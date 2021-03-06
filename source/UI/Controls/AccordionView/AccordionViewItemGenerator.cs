// <copyright file="AccordionViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.AccordionView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class AccordionViewItemGenerator : AccordionViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<AccordionViewItemTemplate, AccordionViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public AccordionViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImpl<AccordionViewItem>(this);
		}

		private TemplatedGeneratorImpl<AccordionViewItem> Implementation { get; }

		public AccordionViewItemTemplate ItemTemplate
		{
			get => (AccordionViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(AccordionViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override AccordionViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(AccordionViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(AccordionViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}