﻿// <copyright file="RotateTransformAnimator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public sealed class RotateTransformAnimator : AnimatorBase<RotateTransform>
  {
    #region Static Fields and Constants

    private static readonly RotateTransform NullTransform = new RotateTransform();

    #endregion

    #region Fields

    private readonly RotateTransform _transform = new RotateTransform();

    #endregion

    #region  Methods

    protected override RotateTransform EvaluateCurrent()
    {
      var interpolator = DoubleInterpolator.Instance;

      var start = Start ?? NullTransform;
      var end = End ?? NullTransform;

      _transform.Angle = interpolator.Evaluate(start.Angle, end.Angle, Time, EasingFunction);
      _transform.CenterX = interpolator.Evaluate(start.CenterX, end.CenterX, Time, EasingFunction);
      _transform.CenterY = interpolator.Evaluate(start.CenterY, end.CenterY, Time, EasingFunction);

      return _transform;
    }

    #endregion
  }
}