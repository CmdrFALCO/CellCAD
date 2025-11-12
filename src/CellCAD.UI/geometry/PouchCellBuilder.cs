using System;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Brushes = System.Windows.Media.Brushes;

namespace CellCAD.geometry
{
    /// <summary>
    /// Builds 2D wireframe geometry for pouch cells using WPF shapes
    /// </summary>
    public static class PouchCellBuilder
    {
        /// <summary>
        /// Builds a rounded-rectangle path geometry for the pouch cell outline
        /// </summary>
        public static PathGeometry Build2DFootprint(double lengthMm, double widthMm, double cornerRadiusMm)
        {
            lengthMm = Math.Max(1, lengthMm);
            widthMm = Math.Max(1, widthMm);
            double r = Math.Max(0, Math.Min(cornerRadiusMm, 0.5 * Math.Min(lengthMm, widthMm)));

            double hx = 0.5 * lengthMm;
            double hy = 0.5 * widthMm;

            var geometry = new PathGeometry();
            var figure = new PathFigure { IsClosed = true };

            // Start at top-right (before corner)
            figure.StartPoint = new Point(hx - r, hy);

            // Top edge (moving left)
            figure.Segments.Add(new LineSegment(new Point(-hx + r, hy), true));

            // Top-left corner arc
            figure.Segments.Add(new ArcSegment(
                new Point(-hx, hy - r),
                new Size(r, r),
                0,
                false,
                SweepDirection.Clockwise,
                true));

            // Left edge (moving down)
            figure.Segments.Add(new LineSegment(new Point(-hx, -hy + r), true));

            // Bottom-left corner arc
            figure.Segments.Add(new ArcSegment(
                new Point(-hx + r, -hy),
                new Size(r, r),
                0,
                false,
                SweepDirection.Clockwise,
                true));

            // Bottom edge (moving right)
            figure.Segments.Add(new LineSegment(new Point(hx - r, -hy), true));

            // Bottom-right corner arc
            figure.Segments.Add(new ArcSegment(
                new Point(hx, -hy + r),
                new Size(r, r),
                0,
                false,
                SweepDirection.Clockwise,
                true));

            // Right edge (moving up)
            figure.Segments.Add(new LineSegment(new Point(hx, hy - r), true));

            // Top-right corner arc
            figure.Segments.Add(new ArcSegment(
                new Point(hx - r, hy),
                new Size(r, r),
                0,
                false,
                SweepDirection.Clockwise,
                true));

            geometry.Figures.Add(figure);
            return geometry;
        }

        /// <summary>
        /// Creates a Path shape for rendering the pouch cell
        /// </summary>
        public static Path CreatePouchCellPath(double lengthMm, double widthMm, double cornerRadiusMm)
        {
            var path = new Path
            {
                Data = Build2DFootprint(lengthMm, widthMm, cornerRadiusMm),
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            return path;
        }
    }
}
