// <copyright file="ArtboardPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardPanel : Panel
	{
		private protected readonly CompositeTransform ScrollViewTransform = new CompositeTransform
		{
			ScaleX = 1.0,
			ScaleY = 1.0
		};

		private ArtboardControl _artboard;
		private double _designHeight;
		private double _designWidth;

		internal event EventHandler DesignMatrixChanged;

		public ArtboardControl Artboard
		{
			get => _artboard;
			internal set
			{
				if (ReferenceEquals(_artboard, value))
					return;

				if (_artboard != null)
					DetachArtboard(_artboard);

				_artboard = value;

				if (_artboard != null)
					AttachArtboard(_artboard);
			}
		}

		internal double DesignHeight
		{
			get => _designHeight;
			set
			{
				if (_designHeight.IsCloseTo(value))
					return;

				_designHeight = value;

				OnDesignHeightChanged();
			}
		}

		internal double DesignWidth
		{
			get => _designWidth;
			set
			{
				if (_designWidth.IsCloseTo(value))
					return;

				_designWidth = value;

				OnDesignWidthChanged();
			}
		}

		protected Matrix FromDesignMatrix => ScrollViewTransform.Transform.Value;

		internal double OffsetX
		{
			get => -ScrollViewTransform.TranslateX;
			set
			{
				if (ScrollViewTransform.TranslateX.IsCloseTo(-value))
					return;

				ScrollViewTransform.TranslateX = -value;

				OnOffsetXChanged();
				OnDesignMatrixChanged();
			}
		}

		internal double OffsetY
		{
			get => -ScrollViewTransform.TranslateY;
			set
			{
				if (ScrollViewTransform.TranslateY.IsCloseTo(-value))
					return;

				ScrollViewTransform.TranslateY = -value;

				OnOffsetYChanged();
				OnDesignMatrixChanged();
			}
		}

		protected Matrix ToDesignMatrix
		{
			get
			{
				var matrix = ScrollViewTransform.Transform.Value;

				matrix.Invert();

				return matrix;
			}
		}

		internal double Zoom
		{
			get => ScrollViewTransform.ScaleX;
			set
			{
				if (Zoom.IsCloseTo(value))
					return;

				ScrollViewTransform.ScaleX = ScrollViewTransform.ScaleY = value;

				OnZoomChanged();
				OnDesignMatrixChanged();
			}
		}

		private protected virtual void AttachArtboard(ArtboardControl artboard)
		{
		}

		private protected virtual void DetachArtboard(ArtboardControl artboard)
		{
		}

		protected virtual void OnDesignHeightChanged()
		{
		}

		private void OnDesignMatrixChanged()
		{
			DesignMatrixChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnDesignWidthChanged()
		{
		}

		protected virtual void OnOffsetXChanged()
		{
			InvalidateArrange();
		}

		protected virtual void OnOffsetYChanged()
		{
			InvalidateArrange();
		}

		protected virtual void OnZoomChanged()
		{
			InvalidateMeasure();
		}

		internal Point TransformFromDesignCoordinates(Point point)
		{
			return FromDesignMatrix.Transform(point);
		}

		internal Rect TransformFromDesignCoordinates(Rect rect)
		{
			var matrix = FromDesignMatrix;

			return new Rect(matrix.Transform(rect.TopLeft), matrix.Transform(rect.BottomRight));
		}

		internal Point TransformToDesignCoordinates(Point point)
		{
			return ToDesignMatrix.Transform(point);
		}

		internal Rect TransformToDesignCoordinates(Rect rect)
		{
			var matrix = ToDesignMatrix;

			return new Rect(matrix.Transform(rect.TopLeft), matrix.Transform(rect.BottomRight));
		}
	}
}