// <copyright file="SkinResourceManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	internal sealed class SkinResourceManager
	{
		#region Fields

		private readonly SkinnedTheme _theme;
		private readonly Dictionary<string, ThemeResource> _themeResources = new Dictionary<string, ThemeResource>(StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Ctors

		public SkinResourceManager()
		{
		}

		public SkinResourceManager(SkinnedTheme theme) : this()
		{
			_theme = theme;
		}

		#endregion

		#region Properties

		private SkinDictionary Root { get; } = new SkinDictionary();

		public HashSet<string> UnresolvedDependencies
		{
			get
			{
				var dependencies = new HashSet<string>();

				TreeEnumerator.Visit(Root, SkinDictionary.SkinDictionaryTreeAdvisor, s => dependencies.AddRange(s.BasedOn.Where(d => d.IsDeferred).Select(d => d.DeferredKey)));

				return dependencies;
			}
		}

		#endregion

		#region  Methods

		public void AddThemeResource(ThemeResource themeResource)
		{
			AddThemeResource(new KeyValuePair<string, object>(themeResource.ActualKey, themeResource.Value));
		}

		private void AddThemeResource(KeyValuePair<string, object> themeResourceKeyValuePair)
		{
			var key = themeResourceKeyValuePair.Key;
			var value = themeResourceKeyValuePair.Value;
			var themeResourceValue = value as ThemeResource;

			if (themeResourceValue != null)
				value = themeResourceValue.Value;

			var themeKeywordGroup = value as ThemeKeywordGroup;

			if (themeKeywordGroup != null)
				foreach (var themeKeyword in themeKeywordGroup)
					AddThemeResource(new KeyValuePair<string, object>(themeKeyword.ActualKey, themeKeyword.Value));

			var themeResource = EnsureThemeResource(key);

			themeResource.BindValue(value);

			_themeResources[themeResource.ActualKey] = themeResource;

			_theme?.BindThemeResourceInternal(themeResource);
		}

		private ThemeResource EnsureThemeResource(string key)
		{
			return new ThemeResource(this) { Key = key };
		}

		public IEnumerable<ThemeResource> EnumerateResources()
		{
			return _themeResources.Values;
		}

		private IEnumerable<KeyValuePair<string, object>> EnumerateResources(ResourceDictionary resourceDictionary)
		{
			foreach (var rd in resourceDictionary.EnumerateDictionaries())
			{
				foreach (var keyObj in rd.Keys)
				{
					var key = keyObj as string;

					if (key == null)
						continue;

					var value = rd[keyObj];

					yield return new KeyValuePair<string, object>(key, value);
				}
			}
		}

		public ThemeResource GetResource(string key)
		{
			return _themeResources.GetValueOrDefault(key);
		}

		public void LoadResources(ThemeSkinResourceDictionary resourceDictionary)
		{
			LoadResourcesImpl(EnumerateResources(resourceDictionary));
		}

		private void LoadResourcesImpl(IEnumerable<KeyValuePair<string, object>> themeResources)
		{
			foreach (var themeResource in themeResources)
			{
				var skinResource = themeResource.Value as SkinDictionary;

				if (skinResource == null)
				{
					Root.Merge(themeResource, SkinDictionaryMergeFlags.Override);

					AddThemeResource(themeResource);
				}
				else
					Root.Merge(themeResource, SkinDictionaryMergeFlags.Default);
			}

			if (ResolveDependencies() == false)
				return;

			var frozenRoot = Root.AsFrozen();

			foreach (var themeResourceKeyValuePair in frozenRoot.Flatten().Select(UnwrapValue))
				AddThemeResource(themeResourceKeyValuePair);
		}

		private bool ResolveDependencies()
		{
			var result = true;

			TreeEnumerator.Visit(Root, SkinDictionary.SkinDictionaryTreeAdvisor, s =>
			{
				if (s.BasedOnInternal == null || s.BasedOnInternal.Count == 0)
					return;

				for (var index = 0; index < s.BasedOn.Count; index++)
				{
					var basedOn = s.BasedOn[index];

					if (basedOn.IsDeferred == false || basedOn.IsAbsoluteKey == false)
						continue;

					object resolved;

					if (Root.TryGetValue(basedOn.DeferredKey, out resolved) == false)
					{
						result = false;

						continue;
					}

					var resolvedSkin = resolved as SkinDictionary;

					if (resolvedSkin != null)
						s.BasedOn[index] = resolvedSkin;
					else
						result = false;
				}
			});

			return result;
		}

		private static KeyValuePair<string, object> UnwrapValue(KeyValuePair<string, object> keyValuePair)
		{
			var themeResourceValue = keyValuePair.Value as ThemeResource;

			return themeResourceValue != null ? keyValuePair.WithValue(themeResourceValue.Value) : keyValuePair;
		}

		#endregion
	}
}