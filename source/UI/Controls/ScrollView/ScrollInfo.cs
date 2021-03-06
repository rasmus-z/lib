﻿// <copyright file="ScrollInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.ScrollView
{
  public struct ScrollInfo
  {
    //private Vector _offset;
    private Size _extent;
    private Size _viewport;

    public Size Extent
    {
      get => _extent;
      set => _extent = value;
    }

    //public Vector Offset
    //{
    //  get { return _offset; }
    //  set { _offset = value; }
    //}

    public Size Viewport
    {
      get => _viewport;
      set => _viewport = value;
    }

    public Size ScrollableSize => new Size
    {
      Height = Math.Max(_extent.Height - _viewport.Height, 0),
      Width = Math.Max(_extent.Width - _viewport.Width, 0),
    };

    internal Vector ClampOffset(Vector offset)
    {
      var scrollableSize = ScrollableSize;

      offset.X = offset.X.Clamp(0, scrollableSize.Width);
      offset.Y = offset.Y.Clamp(0, scrollableSize.Height);

      return offset;
    }
  }
}