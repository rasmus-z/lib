﻿// <copyright file="KeyboardUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  public static class KeyboardUtils
  {
    #region Properties

    public static bool IsAltPressed => Keyboard.Modifiers.IsAltPressed();

    public static bool IsControlPressed => Keyboard.Modifiers.IsControlPressed();

    public static bool IsShiftPressed => Keyboard.Modifiers.IsShiftPressed();

    #endregion
  }
}