// <copyright file="Trigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class Trigger : SourceTriggerBase, IPropertySubject
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(OnValueChanged);
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);

		private static readonly uint DefaultPackedValue;

		#endregion

		#region Fields

		private ITriggerValueComparer _comparer;
		private object _sourceValue;
		private object _targetValue;

		#endregion

		#region Ctors

		static Trigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.PropertyKind.SetValue(ref DefaultPackedValue, PropertyKind.Unspecified);
		}

		public Trigger()
		{
			PackedValue |= DefaultPackedValue;
		}

		#endregion

		#region Properties

		private DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(this);

		public ITriggerValueComparer Comparer
		{
			get => _comparer;
			set
			{
				if (ReferenceEquals(_comparer, value))
					return;

				_comparer = value;
				UpdateTriggerState();
			}
		}

		public string ExpandoProperty
		{
			get => PropertyResolver.GetExpandoProperty(this);
			set => PropertyResolver.SetExpandoProperty(this, value);
		}

		public object Property
		{
			get => PropertyResolver.GetProperty(this);
			set => PropertyResolver.SetProperty(this, value);
		}

		private PropertyKind PropertyKind
		{
			get => PackedDefinition.PropertyKind.GetValue(PackedValue);
			set => PackedDefinition.PropertyKind.SetValue(ref PackedValue, value);
		}

		private object SourceValue
		{
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		public object Value
		{
			get => GetOriginalValue(TargetValueProperty, _targetValue);
			set => SetValue(TargetValueProperty, ref _targetValue, value);
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (Trigger) source;
			PropertyResolver.CopyFrom(this, triggerSource);

			Value = triggerSource.Value;
			Comparer = triggerSource.Comparer;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new Trigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			Unload(TargetValueProperty, ref _targetValue);
			Unload(SourceValueProperty, ref _sourceValue);

			PropertyResolver.UnresolveProperty(this);

			base.DeinitializeTrigger(root);
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			Load(TargetValueProperty, ref _targetValue);
			UpdateSourceBinding();

			base.InitializeTrigger(root);
		}

		private void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			UpdateSourceBinding();
		}

		protected override void OnActualSourceChanged(DependencyObject oldSource)
		{
			base.OnActualSourceChanged(oldSource);
			UpdateSourceBinding();
		}

		private static void OnValueChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
			((Trigger) interactivityobject).UpdateTriggerState();
		}

		private void UpdateSourceBinding()
		{
			var source = ActualSource;
			var property = ActualProperty;

			SourceValue = property != null && source != null ? new Binding {Path = new PropertyPath(property), Source = source} : null;
		}

		protected override TriggerState UpdateTriggerStateCore()
		{
			return TriggerCompareUtil.UpdateState(this, SourceValueProperty, ref _sourceValue, TargetValueProperty, ref _targetValue, Comparer);
		}

		#endregion

		#region Interface Implementations

		#region IPropertySubject

		DependencyObject IPropertySubject.ActualSubject => ActualSource;

		PropertyKind IPropertySubject.PropertyKind
		{
			get => PropertyKind;
			set => PropertyKind = value;
		}

		object IPropertySubject.PropertyStore { get; set; }

		void IPropertySubject.OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			OnActualPropertyChanged(oldProperty, newProperty);
		}

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<Trigger>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
			}

			#endregion
		}

		#endregion
	}
}