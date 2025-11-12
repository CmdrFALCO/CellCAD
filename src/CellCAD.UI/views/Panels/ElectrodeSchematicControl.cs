using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using CellCAD.models;

namespace CellCAD.views
{
    /// <summary>
    /// Simple 2D schematic showing cathode/anode/separator overlay with offsets.
    /// </summary>
    public class ElectrodeSchematicControl : Canvas
    {
        private PouchCellParameters? _params;

        public void UpdateSchematic(PouchCellParameters parameters)
        {
            _params = parameters;
            Children.Clear();

            if (parameters == null) return;

            // Calculate scaling to fit the canvas
            double margin = 40;
            double availableWidth = ActualWidth - 2 * margin;
            double availableHeight = ActualHeight - 2 * margin;

            if (availableWidth <= 0 || availableHeight <= 0) return;

            // Find the maximum dimensions to scale everything
            double maxWidth = Math.Max(parameters.CathodeWidth_mm,
                              Math.Max(parameters.AnodeWidth_mm, parameters.SeparatorWidth_mm));
            double maxHeight = Math.Max(parameters.CathodeHeight_mm,
                               Math.Max(parameters.AnodeHeight_mm, parameters.SeparatorHeight_mm));

            // Add offsets to max dimensions
            maxWidth += Math.Max(parameters.AnodeOffsetX_mm, parameters.SeparatorOffsetX_mm) * 2;
            maxHeight += Math.Max(parameters.AnodeOffsetY_mm, parameters.SeparatorOffsetY_mm) * 2;

            double scaleX = availableWidth / maxWidth;
            double scaleY = availableHeight / maxHeight;
            double scale = Math.Min(scaleX, scaleY);

            // Center point
            double centerX = ActualWidth / 2;
            double centerY = ActualHeight / 2;

            // Create brushes once - explicitly use WPF types
            var separatorBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
            var anodeBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            var cathodeBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightCoral);

            // Draw in order: Separator (back) → Anode → Cathode (front)
            DrawRectangle(
                centerX, centerY,
                parameters.SeparatorWidth_mm * scale,
                parameters.SeparatorHeight_mm * scale,
                parameters.SeparatorOffsetX_mm * scale,
                parameters.SeparatorOffsetY_mm * scale,
                separatorBrush,
                "Separator"
            );

            DrawRectangle(
                centerX, centerY,
                parameters.AnodeWidth_mm * scale,
                parameters.AnodeHeight_mm * scale,
                parameters.AnodeOffsetX_mm * scale,
                parameters.AnodeOffsetY_mm * scale,
                anodeBrush,
                "Anode"
            );

            DrawRectangle(
                centerX, centerY,
                parameters.CathodeWidth_mm * scale,
                parameters.CathodeHeight_mm * scale,
                0, 0, // Cathode is the reference (no offset)
                cathodeBrush,
                "Cathode"
            );

            // Draw coordinate system
            DrawCoordinateSystem(margin, ActualHeight - margin);

            // Draw title
            var grayBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            var title = new TextBlock
            {
                Text = "(Simple 2D plot from full sheet layout)",
                FontSize = 10,
                Foreground = grayBrush,
                FontStyle = FontStyles.Italic
            };
            SetLeft(title, margin);
            SetTop(title, 10);
            Children.Add(title);
        }

        private void DrawRectangle(double centerX, double centerY, double width, double height,
                                   double offsetX, double offsetY, System.Windows.Media.Brush fill, string label)
        {
            // Apply offset (anode/separator are offset relative to cathode)
            double x = centerX - width / 2 + offsetX;
            double y = centerY - height / 2 + offsetY;

            var blackBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill,
                Stroke = blackBrush,
                StrokeThickness = 1.5,
                Opacity = 0.7
            };

            SetLeft(rect, x);
            SetTop(rect, y);
            Children.Add(rect);

            // Add label
            var text = new TextBlock
            {
                Text = label,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = blackBrush
            };
            SetLeft(text, x + 5);
            SetTop(text, y + 5);
            Children.Add(text);
        }

        private void DrawCoordinateSystem(double x, double y)
        {
            double arrowLen = 30;

            var redBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            var greenBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            var blueBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);

            // X axis (red)
            var xLine = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = x + arrowLen,
                Y2 = y,
                Stroke = redBrush,
                StrokeThickness = 2
            };
            Children.Add(xLine);

            var xLabel = new TextBlock
            {
                Text = "X",
                FontWeight = FontWeights.Bold,
                Foreground = redBrush
            };
            SetLeft(xLabel, x + arrowLen + 5);
            SetTop(xLabel, y - 10);
            Children.Add(xLabel);

            // Y axis (green)
            var yLine = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = x,
                Y2 = y - arrowLen,
                Stroke = greenBrush,
                StrokeThickness = 2
            };
            Children.Add(yLine);

            var yLabel = new TextBlock
            {
                Text = "Y",
                FontWeight = FontWeights.Bold,
                Foreground = greenBrush
            };
            SetLeft(yLabel, x - 15);
            SetTop(yLabel, y - arrowLen - 5);
            Children.Add(yLabel);

            // Z label (pointing out of screen)
            var zLabel = new TextBlock
            {
                Text = "⊙ Z",
                FontWeight = FontWeights.Bold,
                Foreground = blueBrush,
                ToolTip = "Z axis points out of the screen"
            };
            SetLeft(zLabel, x - 25);
            SetTop(zLabel, y + 5);
            Children.Add(zLabel);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_params != null)
                UpdateSchematic(_params);
        }
    }
}
