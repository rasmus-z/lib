// <copyright file="TabViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Menu;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Utils;
using SortDescriptionCollection = Zaaml.UI.Data.SortDescriptionCollection;
using Style = System.Windows.Style;

namespace Zaaml.UI.Controls.TabView
{
	[TemplateContractType(typeof(TabViewControlTemplateContract))]
	public class TabViewControl : IndexedSelectorBase<TabViewControl, TabViewItem, TabViewItemCollection, TabViewItemsPresenter, TabViewItemsPanel>, IHeaderedContentItemsControl
	{
		private static readonly DependencyPropertyKey ActualCreateTabButtonVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TabViewControl>
			("ActualCreateTabButtonVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualCreateTabButtonVisibilityProperty = ActualCreateTabButtonVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualIsBackContentVisiblePropertyKey = DPM.RegisterReadOnly<bool, TabViewControl>
			("ActualIsBackContentVisible", true);

		public static readonly DependencyProperty ActualIsBackContentVisibleProperty = ActualIsBackContentVisiblePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualItemsMenuButtonVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TabViewControl>
			("ActualItemsMenuButtonVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualItemsMenuButtonVisibilityProperty = ActualItemsMenuButtonVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualItemsPresenterVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TabViewControl>
			("ActualItemsPresenterVisibility");

		public static readonly DependencyProperty ActualItemsPresenterVisibilityProperty = ActualItemsPresenterVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualMenuItemGeneratorPropertyKey = DPM.RegisterReadOnly<MenuItemGeneratorBase, TabViewControl>
			("ActualMenuItemGenerator");

		public static readonly DependencyProperty ActualMenuItemGeneratorProperty = ActualMenuItemGeneratorPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualMenuItemsSourcePropertyKey = DPM.RegisterReadOnly<IEnumerable, TabViewControl>
			("ActualMenuItemsSource");

		public static readonly DependencyProperty ActualMenuItemsSourceProperty = ActualMenuItemsSourcePropertyKey.DependencyProperty;

		public static readonly DependencyProperty BackContentProperty = DPM.Register<object, TabViewControl>
			("BackContent");

		public static readonly DependencyProperty BackContentTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("BackContentTemplate");

		public static readonly DependencyProperty ItemsPinButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewControl>
			("ItemsPinButtonVisibility", ElementVisibility.Collapsed, t => t.UpdateItemsButtonsVisibility);

		public static readonly DependencyProperty ItemsCloseButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewControl>
			("ItemsCloseButtonVisibility", ElementVisibility.Collapsed, t => t.UpdateItemsButtonsVisibility);

		public static readonly DependencyProperty ContentPresenterStyleProperty = DPM.Register<Style, TabViewControl>
			("ContentPresenterStyle");

		public static readonly DependencyProperty CreateTabButtonContentProperty = DPM.Register<object, TabViewControl>
			("CreateTabButtonContent");

		public static readonly DependencyProperty CreateTabButtonContentTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("CreateTabButtonContentTemplate");

		public static readonly DependencyProperty CreateTabButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewControl>
			("CreateTabButtonVisibility", ElementVisibility.Collapsed, t => t.UpdateCreateTabButtonVisibility);

		public static readonly DependencyProperty DragOutDistanceProperty = DPM.Register<double, TabViewControl>
			("DragOutDistance", 100.0);

		private static readonly DependencyProperty InternalMenuItemsSourceProperty = DPM.Register<IEnumerable, TabViewControl>
			("InternalMenuItemsSource", t => t.OnInternalMenuItemsSourceChanged);

		public static readonly DependencyProperty IsBackContentVisibleProperty = DPM.Register<bool, TabViewControl>
			("IsBackContentVisible", false, t => t.OnIsBackContentVisibleChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, TabViewControl>
			("ItemContentStringFormat", a => a.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("ItemContentTemplate", a => a.DefaultGeneratorImpl.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, TabViewControl>
			("ItemContentTemplateSelector", a => a.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<TabViewItemGeneratorBase, TabViewControl>
			("ItemGenerator", t => t.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemHeaderStringFormatProperty = DPM.Register<string, TabViewControl>
			("ItemHeaderStringFormat", a => a.DefaultGeneratorImpl.OnItemHeaderStringFormatChanged);

		public static readonly DependencyProperty ItemHeaderTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("ItemHeaderTemplate", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateChanged);

		public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, TabViewControl>
			("ItemHeaderTemplateSelector", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateSelectorChanged);

		public static readonly DependencyProperty ItemsMenuButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewControl>
			("ItemsMenuButtonVisibility", ElementVisibility.Auto, t => t.UpdateMenuButtonVisibility);

		public static readonly DependencyProperty ItemsPresenterFooterProperty = DPM.Register<object, TabViewControl>
			("ItemsPresenterFooter");

		public static readonly DependencyProperty ItemsPresenterFooterTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("ItemsPresenterFooterTemplate");

		public static readonly DependencyProperty ItemsPresenterHeaderProperty = DPM.Register<object, TabViewControl>
			("ItemsPresenterHeader");

		public static readonly DependencyProperty ItemsPresenterHeaderTemplateProperty = DPM.Register<DataTemplate, TabViewControl>
			("ItemsPresenterHeaderTemplate");

		public static readonly DependencyProperty ItemsPresenterStyleProperty = DPM.Register<Style, TabViewControl>
			("ItemsPresenterStyle");

		public static readonly DependencyProperty ItemsPresenterVisibilityProperty = DPM.Register<ElementVisibility, TabViewControl>
			("ItemsPresenterVisibility", ElementVisibility.Auto, t => t.OnTabContainerVisibilityChanged);

		public static readonly DependencyProperty ItemsFlexDefinitionProperty = DPM.Register<FlexDefinition, TabViewControl>
			("ItemsFlexDefinition", new FlexDefinition { StretchDirection = FlexStretchDirection.Shrink, Length = FlexLength.Auto });

		public static readonly DependencyProperty MenuItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, TabViewControl>
			("MenuItemGenerator", t => t.OnMenuItemGeneratorChanged);

		private static readonly DependencyPropertyKey MenuItemsSortDescriptionsPropertyKey = DPM.RegisterReadOnly<SortDescriptionCollection, TabViewControl>
			("MenuItemsSortDescriptionsInt");

		public static readonly DependencyProperty MenuItemsSortDescriptionsProperty = MenuItemsSortDescriptionsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TabStripPlacementProperty = DPM.Register<Dock, TabViewControl>
			("TabStripPlacement", Dock.Top);

		public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, TabViewControl>
			("ItemsSource", i => i.OnItemsSourceChangedPrivate);

		private readonly CollectionViewSource _menuItemsCollectionViewSource = new CollectionViewSource { IsLiveSortingRequested = true };
		private DelegateHeaderedContentItemGeneratorImpl<TabViewItem, DefaultTabViewItemGenerator> _defaultGeneratorImpl;

		public event EventHandler<CanCloseTabViewItemEventArgs> QueryCanCloseTab;
		public event EventHandler<CanCreateTabViewItemEventArgs> QueryCanCreateTab;
		public event EventHandler<TabViewItemEventArgs> QueryCloseTab;
		public event EventHandler QueryCreateTab;
		public event EventHandler<TabViewItemEventArgs> QueryDragOutTab;

		static TabViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabViewControl>();
		}

		public TabViewControl()
		{
			this.OverrideStyleKey<TabViewControl>();

			SetBinding(InternalMenuItemsSourceProperty, new Binding { Source = _menuItemsCollectionViewSource });

			DefaultMenuItemGenerator = new TabViewControlMenuItemGenerator(this);
			ActualMenuItemGenerator = DefaultMenuItemGenerator;

			CloseTabCommand = new RelayCommand(p => OnQueryCloseTab((TabViewItem) p), p => OnQueryCanCloseTab((TabViewItem) p));
			CreateTabCommand = new RelayCommand(OnQueryCreateTab, OnQueryCanCreateTab);
		}

		public ICommand CloseTabCommand { get; }

		public ICommand CreateTabCommand { get; }

		public Visibility ActualCreateTabButtonVisibility
		{
			get => (Visibility) GetValue(ActualCreateTabButtonVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualCreateTabButtonVisibilityPropertyKey, value);
		}

		public bool ActualIsBackContentVisible
		{
			get => (bool) GetValue(ActualIsBackContentVisibleProperty);
			private set => this.SetReadOnlyValue(ActualIsBackContentVisiblePropertyKey, value);
		}

		private TabViewItemGeneratorBase ActualItemGenerator => ItemGenerator ?? DefaultItemGenerator;

		public Visibility ActualItemsMenuButtonVisibility
		{
			get => (Visibility) GetValue(ActualItemsMenuButtonVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualItemsMenuButtonVisibilityPropertyKey, value);
		}

		public Visibility ActualItemsPresenterVisibility
		{
			get => (Visibility) GetValue(ActualItemsPresenterVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualItemsPresenterVisibilityPropertyKey, value);
		}

		public MenuItemGeneratorBase ActualMenuItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(ActualMenuItemGeneratorProperty);
			private set => this.SetReadOnlyValue(ActualMenuItemGeneratorPropertyKey, value);
		}

		public IEnumerable ActualMenuItemsSource
		{
			get => (IEnumerable) GetValue(ActualMenuItemsSourceProperty);
			private set => this.SetReadOnlyValue(ActualMenuItemsSourcePropertyKey, value);
		}

		private IEnumerable<TabViewItem> ActualTabViewItems => Items.ActualItemsInternal;

		public object BackContent
		{
			get => GetValue(BackContentProperty);
			set => SetValue(BackContentProperty, value);
		}

		public DataTemplate BackContentTemplate
		{
			get => (DataTemplate) GetValue(BackContentTemplateProperty);
			set => SetValue(BackContentTemplateProperty, value);
		}

		private TabViewContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		public Style ContentPresenterStyle
		{
			get => (Style) GetValue(ContentPresenterStyleProperty);
			set => SetValue(ContentPresenterStyleProperty, value);
		}

		public object CreateTabButtonContent
		{
			get => GetValue(CreateTabButtonContentProperty);
			set => SetValue(CreateTabButtonContentProperty, value);
		}

		public DataTemplate CreateTabButtonContentTemplate
		{
			get => (DataTemplate) GetValue(CreateTabButtonContentTemplateProperty);
			set => SetValue(CreateTabButtonContentTemplateProperty, value);
		}

		public ElementVisibility CreateTabButtonVisibility
		{
			get => (ElementVisibility) GetValue(CreateTabButtonVisibilityProperty);
			set => SetValue(CreateTabButtonVisibilityProperty, value);
		}

		private protected override bool DefaultAllowNullSelection => false;

		private DelegateHeaderedContentItemGeneratorImpl<TabViewItem, DefaultTabViewItemGenerator> DefaultGeneratorImpl =>
			_defaultGeneratorImpl ??= new DelegateHeaderedContentItemGeneratorImpl<TabViewItem, DefaultTabViewItemGenerator>(this);

		private TabViewItemGeneratorBase DefaultItemGenerator => DefaultGeneratorImpl.Generator;

		private MenuItemGeneratorBase DefaultMenuItemGenerator { get; }

		private protected override bool DefaultPreferSelection => true;

		public double DragOutDistance
		{
			get => (double) GetValue(DragOutDistanceProperty);
			set => SetValue(DragOutDistanceProperty, value);
		}

		public bool IsBackContentVisible
		{
			get => (bool) GetValue(IsBackContentVisibleProperty);
			set => SetValue(IsBackContentVisibleProperty, value);
		}
		
		public TabViewItemGeneratorBase ItemGenerator
		{
			get => (TabViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public ElementVisibility ItemsCloseButtonVisibility
		{
			get => (ElementVisibility) GetValue(ItemsCloseButtonVisibilityProperty);
			set => SetValue(ItemsCloseButtonVisibilityProperty, value);
		}

		public FlexDefinition ItemsFlexDefinition
		{
			get => (FlexDefinition) GetValue(ItemsFlexDefinitionProperty);
			set => SetValue(ItemsFlexDefinitionProperty, value);
		}

		public ElementVisibility ItemsMenuButtonVisibility
		{
			get => (ElementVisibility) GetValue(ItemsMenuButtonVisibilityProperty);
			set => SetValue(ItemsMenuButtonVisibilityProperty, value);
		}

		public ElementVisibility ItemsPinButtonVisibility
		{
			get => (ElementVisibility) GetValue(ItemsPinButtonVisibilityProperty);
			set => SetValue(ItemsPinButtonVisibilityProperty, value);
		}

		public object ItemsPresenterFooter
		{
			get => GetValue(ItemsPresenterFooterProperty);
			set => SetValue(ItemsPresenterFooterProperty, value);
		}

		public DataTemplate ItemsPresenterFooterTemplate
		{
			get => (DataTemplate) GetValue(ItemsPresenterFooterTemplateProperty);
			set => SetValue(ItemsPresenterFooterTemplateProperty, value);
		}

		public object ItemsPresenterHeader
		{
			get => GetValue(ItemsPresenterHeaderProperty);
			set => SetValue(ItemsPresenterHeaderProperty, value);
		}

		public DataTemplate ItemsPresenterHeaderTemplate
		{
			get => (DataTemplate) GetValue(ItemsPresenterHeaderTemplateProperty);
			set => SetValue(ItemsPresenterHeaderTemplateProperty, value);
		}

		public Style ItemsPresenterStyle
		{
			get => (Style) GetValue(ItemsPresenterStyleProperty);
			set => SetValue(ItemsPresenterStyleProperty, value);
		}

		public ElementVisibility ItemsPresenterVisibility
		{
			get => (ElementVisibility) GetValue(ItemsPresenterVisibilityProperty);
			set => SetValue(ItemsPresenterVisibilityProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public MenuItemGeneratorBase MenuItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(MenuItemGeneratorProperty);
			set => SetValue(MenuItemGeneratorProperty, value);
		}

		public SortDescriptionCollection MenuItemsSortDescriptions => this.GetValueOrCreate(MenuItemsSortDescriptionsPropertyKey, CreateSortDescriptionCollection);

		public object SelectedContent => SelectedTabViewItem?.Content;

		internal TabViewItem SelectedTabViewItem => SelectedItem;

		public Dock TabStripPlacement
		{
			get => (Dock) GetValue(TabStripPlacementProperty);
			set => SetValue(TabStripPlacementProperty, value);
		}

		private TabViewControlTemplateContract TemplateContract => (TabViewControlTemplateContract) TemplateContractInternal;

		internal void Activate(TabViewItem tabViewItem)
		{
			if (ReferenceEquals(SelectedItem, tabViewItem) == false)
				SelectorController.SelectedItem = tabViewItem;

			HideBackContent();
		}

		protected override TabViewItemCollection CreateItemCollection()
		{
			var itemCollection = new TabViewItemCollection(this)
			{
				Generator = ActualItemGenerator
			};

			_menuItemsCollectionViewSource.SetBinding(CollectionViewSource.SourceProperty, new Binding
			{
				Path = new PropertyPath("ItemsSource"),
				Source = this,
				TargetNullValue = itemCollection
			});

			return itemCollection;
		}

		internal override SelectorController<TabViewControl, TabViewItem> CreateSelectorController()
		{
			return new TabViewSelectorController(this);
		}

		private SortDescriptionCollection CreateSortDescriptionCollection()
		{
			var sortDescriptionCollection = new SortDescriptionCollection();

			sortDescriptionCollection.CollectionChanged += OnSortDescriptionCollectionChanged;

			return sortDescriptionCollection;
		}

		private void EnsureSelectedItemVisible()
		{
			var item = SelectedTabViewItem;

			if (item == null)
				return;

			if (FlexPanel.GetIsHidden(item))
				ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();
		}

		private void EnsureSelection()
		{
			if (IsInitializing == false)
				SelectorController.EnsureSelection();
		}

		private void HideBackContent()
		{
			this.SetCurrentValueInternal(IsBackContentVisibleProperty, KnownBoxes.BoolFalse);
		}

		private void OnDragMove(object sender, DragMoveRoutedEventArgs e)
		{
			var tabItem = (TabViewItem) sender;
			var dragOutDistance = DragOutDistance;
			var box = tabItem.GetScreenBox().GetInflated(dragOutDistance, dragOutDistance);

			if (box.Contains(MouseInternal.ScreenPosition) == false)
				OnDragOutTabItem(tabItem);
		}

		private void OnDragOutTabItem(TabViewItem tabViewItem)
		{
			QueryDragOutTab?.Invoke(this, new TabViewItemEventArgs(tabViewItem));
		}

		protected override void OnHasItemsChanged()
		{
			base.OnHasItemsChanged();

			UpdateActualIsBackContentVisible();
			UpdateMenuButtonVisibility();
			UpdateItemsPresenterVisibility();
		}

		internal void OnHasOverflowedChildrenChanged()
		{
			UpdateMenuButtonVisibility();
		}

		private void OnInternalMenuItemsSourceChanged()
		{
			ActualMenuItemsSource = (IEnumerable) GetValue(InternalMenuItemsSourceProperty);
		}

		private void OnIsBackContentVisibleChanged()
		{
			UpdateActualIsBackContentVisible();
			UpdatePreferSelection();
		}

		protected override void OnItemAttached(TabViewItem tabViewItem)
		{
			base.OnItemAttached(tabViewItem);

			EnsureSelection();
		}

		internal override void OnItemAttachedInternal(TabViewItem item)
		{
			item.TabViewControl = this;
			DraggableBehavior.AddDragMoveHandler(item, OnDragMove);

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(TabViewItem tabViewItem)
		{
			base.OnItemDetachedInternal(tabViewItem);

			DraggableBehavior.RemoveDragMoveHandler(tabViewItem, OnDragMove);
			tabViewItem.TabViewControl = null;

			EnsureSelection();
		}

		internal override void OnItemDetachingInternal(TabViewItem item)
		{
			if (ReferenceEquals(item, SelectedTabViewItem) == false)
				return;

			var orderedChildren = ItemsPresenter?.ItemsHostInternal?.OrderedChildren;

			if (orderedChildren == null || orderedChildren.Count == 0)
				return;

			var index = orderedChildren.IndexOf(item);
			var nextIndex = SelectNextHelper.SelectNext(index, orderedChildren.Count, SelectDirection.Next, false).Clamp(0, orderedChildren.Count);
			var nextItem = orderedChildren[nextIndex];

			if (nextItem == null)
				return;

			var preferredIndex = Items.GetIndexFromItemInternal(nextItem);

			if (preferredIndex != -1)
				SelectorController.PreferredIndex = preferredIndex;

			base.OnItemDetachingInternal(item);
		}

		internal virtual void OnItemGeneratorChanged(TabViewItemGeneratorBase oldGenerator, TabViewItemGeneratorBase newGenerator)
		{
			Items.Generator = ActualItemGenerator;
		}

		private void OnItemsSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsSourceCore = newSource;
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			SelectorController.EnsureSelection();
		}

		private void OnMenuItemGeneratorChanged()
		{
			ActualMenuItemGenerator = MenuItemGenerator ?? DefaultMenuItemGenerator;
		}

		internal void OnMenuItemSelect(object parameter)
		{
			if (ItemsSource == null)
			{
				if (parameter is TabViewItem tabViewItem)
					Activate(tabViewItem);

				return;
			}

			this.SetCurrentValueInternal(SelectedItemSourceProperty, parameter);

			if (ReferenceEquals(SelectedItemSource, parameter))
				HideBackContent();
		}

		protected virtual bool OnQueryCanCloseTab(TabViewItem tabViewItem)
		{
			var canCloseTabItemEventArgs = new CanCloseTabViewItemEventArgs(tabViewItem) { CanClose = true };

			QueryCanCloseTab?.Invoke(this, canCloseTabItemEventArgs);

			return canCloseTabItemEventArgs.CanClose;
		}

		protected virtual bool OnQueryCanCreateTab()
		{
			var canCreateTabItemEventArgs = new CanCreateTabViewItemEventArgs { CanCreate = true };

			QueryCanCreateTab?.Invoke(this, canCreateTabItemEventArgs);

			return canCreateTabItemEventArgs.CanCreate;
		}

		protected virtual void OnQueryCloseTab(TabViewItem tabViewItem)
		{
			QueryCloseTab?.Invoke(this, new TabViewItemEventArgs(tabViewItem));
		}

		protected virtual void OnQueryCreateTab()
		{
			QueryCreateTab?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnSelectionChanged(Selection<TabViewItem> oldSelection, Selection<TabViewItem> newSelection)
		{
			base.OnSelectionChanged(oldSelection, newSelection);

			EnsureSelectedItemVisible();
			UpdateSelectedContent();
		}

		private void OnSortDescriptionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			using (_menuItemsCollectionViewSource.DeferRefresh())
			{
				_menuItemsCollectionViewSource.SortDescriptions.Clear();

				foreach (var menuItemsSortDescription in MenuItemsSortDescriptions)
					_menuItemsCollectionViewSource.SortDescriptions.Add(menuItemsSortDescription.ToComponentModel());
			}
		}

		private void OnTabContainerVisibilityChanged()
		{
			UpdateItemsPresenterVisibility();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.TabViewControl = this;
			ContentPresenter.TabViewControl = this;

			UpdateSelectedContent();
			UpdateItemsPresenterVisibility();

			UpdateCreateTabButtonVisibility();
			UpdateItemsButtonsVisibility();
			UpdateMenuButtonVisibility();
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.TabViewControl = null;
			ContentPresenter.TabViewControl = null;

			UpdateCreateTabButtonVisibility();
			UpdateItemsButtonsVisibility();
			UpdateMenuButtonVisibility();

			base.OnTemplateContractDetaching();
		}

		internal void SelectItemInternal(TabViewItem tabViewItem)
		{
			SelectorController.SelectedItem = tabViewItem;
		}

		private void UpdateActualIsBackContentVisible()
		{
			ActualIsBackContentVisible = IsBackContentVisible || HasItems == false;
		}

		private void UpdateCreateTabButtonVisibility()
		{
			ActualCreateTabButtonVisibility = VisibilityUtils.EvaluateElementVisibility(CreateTabButtonVisibility, Visibility.Collapsed);
		}

		private void UpdateItemsButtonsVisibility()
		{
			foreach (var tabViewItem in ActualTabViewItems)
				tabViewItem.UpdateButtonsVisibility();
		}

		private void UpdateItemsPresenterVisibility()
		{
			ActualItemsPresenterVisibility = VisibilityUtils.EvaluateElementVisibility(ItemsPresenterVisibility, HasItems ? Visibility.Visible : Visibility.Collapsed);
		}

		private void UpdateMenuButtonVisibility()
		{
			var autoVisibility = HasItems == false ? Visibility.Collapsed : (ItemsPresenter?.HasHiddenChildren == true ? Visibility.Visible : Visibility.Collapsed);
			ActualItemsMenuButtonVisibility = VisibilityUtils.EvaluateElementVisibility(ItemsMenuButtonVisibility, autoVisibility);
		}

		private void UpdatePreferSelection()
		{
			PreferSelection = IsBackContentVisible == false;
		}

		protected virtual void UpdateSelectedContent()
		{
			if (ContentPresenter == null)
				return;

			ContentPresenter.Content = SelectedTabViewItem?.ContentHost;
		}

		public string ItemContentStringFormat
		{
			get => (string) GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ItemContentTemplateSelectorProperty);
			set => SetValue(ItemContentTemplateSelectorProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate) GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public string ItemHeaderStringFormat
		{
			get => (string) GetValue(ItemHeaderStringFormatProperty);
			set => SetValue(ItemHeaderStringFormatProperty, value);
		}

		public DataTemplateSelector ItemHeaderTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ItemHeaderTemplateSelectorProperty);
			set => SetValue(ItemHeaderTemplateSelectorProperty, value);
		}

		public DataTemplate ItemHeaderTemplate
		{
			get => (DataTemplate) GetValue(ItemHeaderTemplateProperty);
			set => SetValue(ItemHeaderTemplateProperty, value);
		}

		public override void EndInit()
		{
			UpdatePreferSelection();

			base.EndInit();
		}
	}
}