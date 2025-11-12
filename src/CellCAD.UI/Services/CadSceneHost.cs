using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CellCAD.geometry;
using CellCAD.Core.Geometry;
using Brushes = System.Windows.Media.Brushes;

namespace CellCAD.services
{
    /// <summary>
    /// Manages a WPF Canvas for rendering 2D wireframe geometry of pouch cells
    /// </summary>
    public sealed class CadSceneHost : ICadHost, IDisposable
    {
        // Tracks last geometry we drew so it can be removed on the next rebuild
        private UIElement? _lastPouchPath;

        public Canvas Canvas { get; }

        public CadSceneHost()
        {
            Canvas = new Canvas
            {
                Background = Brushes.White,
                ClipToBounds = true
            };
        }

        public void Dispose()
        {
            // Canvas cleanup if needed
            Canvas.Children.Clear();
        }

        public void Clear()
        {
            Canvas.Children.Clear();
            _lastPouchPath = null;
        }

        /// <summary>
        /// Build and display pouch cell footprint (2D wireframe).
        /// </summary>
        public void BuildPouchCell(PouchCellParameters p)
        {
            // Remove previous geometry
            if (_lastPouchPath != null)
            {
                Canvas.Children.Remove(_lastPouchPath);
                _lastPouchPath = null;
            }

            // Build current geometry
            var path = PouchCellBuilder.CreatePouchCellPath(
                Math.Max(1, p.Length_mm),
                Math.Max(1, p.Width_mm),
                Math.Max(0, p.CornerRadius_mm)
            );

            // Add to canvas
            Canvas.Children.Add(path);
            _lastPouchPath = path;

            // Center and fit the geometry to canvas
            FitToCanvas();
        }

        /// <summary>
        /// Centers and scales the geometry to fit the canvas viewport
        /// </summary>
        private void FitToCanvas()
        {
            if (Canvas.Children.Count == 0 || Canvas.ActualWidth <= 0 || Canvas.ActualHeight <= 0)
                return;

            // Calculate bounds of all children
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (UIElement child in Canvas.Children)
            {
                if (child is Path path && path.Data is PathGeometry geometry)
                {
                    var bounds = geometry.Bounds;
                    minX = Math.Min(minX, bounds.Left);
                    minY = Math.Min(minY, bounds.Top);
                    maxX = Math.Max(maxX, bounds.Right);
                    maxY = Math.Max(maxY, bounds.Bottom);
                }
            }

            if (double.IsInfinity(minX) || double.IsInfinity(maxX))
                return;

            double contentWidth = maxX - minX;
            double contentHeight = maxY - minY;

            if (contentWidth <= 0 || contentHeight <= 0)
                return;

            // Calculate scale to fit with margins
            double marginRatio = 0.9; // 90% of canvas (10% margin)
            double scaleX = (Canvas.ActualWidth * marginRatio) / contentWidth;
            double scaleY = (Canvas.ActualHeight * marginRatio) / contentHeight;
            double scale = Math.Min(scaleX, scaleY);

            // Calculate center offset
            double centerX = Canvas.ActualWidth / 2.0;
            double centerY = Canvas.ActualHeight / 2.0;
            double contentCenterX = (minX + maxX) / 2.0;
            double contentCenterY = (minY + maxY) / 2.0;

            // Apply transform to each child
            foreach (UIElement child in Canvas.Children)
            {
                var transformGroup = new TransformGroup();

                // Translate to origin
                transformGroup.Children.Add(new TranslateTransform(-contentCenterX, -contentCenterY));

                // Scale
                transformGroup.Children.Add(new ScaleTransform(scale, scale));

                // Translate to canvas center
                transformGroup.Children.Add(new TranslateTransform(centerX, centerY));

                child.RenderTransform = transformGroup;
            }
        }

        /// <summary>
        /// Temporary no-op placeholder; safe to keep if you still call it anywhere.
        /// </summary>
        public void ShowPlaceholder(PouchCellParameters _) => Clear();
    }
}
