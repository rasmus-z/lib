// <copyright file="IStackPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Panels.Interfaces
{
  internal interface IStackPanel : IOrientedPanel
  {
  }

  internal interface IStackPanelAdvanced : IStackPanel
  {
		double Spacing { get; }
  }

  internal interface IVirtualPanel
  {
    #region Properties

    UIElementCollection Children { get; }

    IVirtualItemCollection VirtualSource { get; }

    #endregion
  }

  internal interface IVirtualStackPanel : IStackPanel, IScrollViewPanel, IVirtualPanel
  {
    #region Properties

    int FocusedIndex { get; }

    CompositeTransform Transform { get; } 

    #endregion
  }
}