﻿// <copyright file="DefaultTemplatedGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal class TemplatedGeneratorImpl<TItem> : IItemGenerator<TItem> where TItem : FrameworkElement, new()
	{
		#region Fields

		private readonly GeneratorDataTemplateHelper<TItem, TItem> _generatorDataTemplateHelper = new GeneratorDataTemplateHelper<TItem, TItem>();
		private DataTemplate _itemTemplate;

		#endregion

		#region Ctors

		public TemplatedGeneratorImpl(ItemGenerator<TItem> generator)
		{
			Generator = generator;
		}

		#endregion

		#region Properties

		public ItemGenerator<TItem> Generator { get; }

		public DataTemplate ItemTemplate
		{
			get => _itemTemplate;
			set
			{
				if (ReferenceEquals(_itemTemplate, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemTemplate = value;
				_generatorDataTemplateHelper.DataTemplate = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		#endregion

		#region Interface Implementations

		#region IItemGenerator<TItem>

		public void AttachItem(TItem item, object itemSource)
		{
			_generatorDataTemplateHelper.AttachDataContext(item, itemSource);
		}

		public TItem CreateItem(object itemSource)
		{
			var itemTemplate = ItemTemplate;

			if (itemTemplate == null)
				return new TItem();

			var item = _generatorDataTemplateHelper.Load(itemSource);

			if (item == null)
				throw new InvalidOperationException();

			return item;
		}

		public void DetachItem(TItem item, object itemSource)
		{
			if (ReferenceEquals(item.DataContext, itemSource))
				item.ClearValue(FrameworkElement.DataContextProperty);
		}

		public void DisposeItem(TItem item, object itemSource)
		{
		}

		#endregion

		#endregion
	}
}