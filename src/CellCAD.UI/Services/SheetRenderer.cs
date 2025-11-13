using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CellCAD.viewmodels;

namespace CellCAD.services
{
    /// <summary>
    /// Renders 2D sheet layout views on WPF Canvas
    /// </summary>
    public static class SheetRenderer
    {
        public static void RenderFullLayout(Canvas target, PouchCellViewModel vm)
        {
            if (target == null || vm == null) return;

            target.Children.Clear();

            // Get dimensions
            double cathodeW = vm.CathodeWidth_mm;
            double cathodeH = vm.CathodeHeight_mm;
            double anodeW = vm.AnodeWidth_mm;
            double anodeH = vm.AnodeHeight_mm;
            double sepW = vm.SeparatorWidth_mm;
            double sepH = vm.SeparatorHeight_mm;

            // Compute world bounds (separator is largest)
            double worldWidth = sepW;
            double worldHeight = sepH;

            // Add space for flags
            double maxFlagHeight = Math.Max(vm.CathodeFlagHeight_mm, vm.AnodeFlagHeight_mm);
            worldHeight += maxFlagHeight * 2; // Add space above and below for flags

            // Compute scale with padding
            double padding = 20;
            double availableWidth = target.ActualWidth > 0 ? target.ActualWidth - 2 * padding : 400;
            double availableHeight = target.ActualHeight > 0 ? target.ActualHeight - 2 * padding : 300;

            double scaleX = availableWidth / worldWidth;
            double scaleY = availableHeight / worldHeight;
            double scale = Math.Min(scaleX, scaleY);

            if (scale <= 0) scale = 1;

            // Center offset (separator bottom-left at origin in world coords)
            double sepOffsetX = (vm.SeparatorWidth_mm - vm.AnodeWidth_mm) / 2.0;
            double sepOffsetY = (vm.SeparatorHeight_mm - vm.AnodeHeight_mm) / 2.0;
            double anodeOffsetX = (vm.AnodeWidth_mm - vm.CathodeWidth_mm) / 2.0;
            double anodeOffsetY = (vm.AnodeHeight_mm - vm.CathodeHeight_mm) / 2.0;

            double centerX = padding + (availableWidth - worldWidth * scale) / 2.0;
            double centerY = padding + (availableHeight - worldHeight * scale) / 2.0 + maxFlagHeight * scale;

            // Draw separator (thin, light gray)
            var sepRect = CreateRectangle(
                centerX,
                centerY,
                sepW * scale,
                sepH * scale,
                Colors.LightGray, 1.0);
            target.Children.Add(sepRect);

            // Draw anode (medium, orange)
            var anodeRect = CreateRectangle(
                centerX + sepOffsetX * scale,
                centerY + sepOffsetY * scale,
                anodeW * scale,
                anodeH * scale,
                Colors.Orange, 2.0);
            target.Children.Add(anodeRect);

            // Draw cathode (bold, red)
            var cathodeRect = CreateRectangle(
                centerX + sepOffsetX * scale + anodeOffsetX * scale,
                centerY + sepOffsetY * scale + anodeOffsetY * scale,
                cathodeW * scale,
                cathodeH * scale,
                Colors.Red, 3.0);
            target.Children.Add(cathodeRect);

            // Draw flags/tabs
            double cathodeX = centerX + sepOffsetX * scale + anodeOffsetX * scale;
            double cathodeY = centerY + sepOffsetY * scale + anodeOffsetY * scale;
            double anodeX = centerX + sepOffsetX * scale;
            double anodeY = centerY + sepOffsetY * scale;

            if (vm.FlagsOnOppositeSides)
            {
                // Cathode flag on top edge
                var cathodeFlag = CreateRectangle(
                    cathodeX + vm.CathodeFlagOffsetX_mm * scale,
                    cathodeY + cathodeH * scale,
                    vm.CathodeFlagWidth_mm * scale,
                    vm.CathodeFlagHeight_mm * scale,
                    Colors.DarkRed, 2.0);
                target.Children.Add(cathodeFlag);

                // Anode flag on bottom edge
                var anodeFlag = CreateRectangle(
                    anodeX + vm.AnodeFlagOffsetX_mm * scale,
                    anodeY - vm.AnodeFlagHeight_mm * scale,
                    vm.AnodeFlagWidth_mm * scale,
                    vm.AnodeFlagHeight_mm * scale,
                    Colors.DarkOrange, 2.0);
                target.Children.Add(anodeFlag);
            }
            else if (vm.FlagsOnSameSide)
            {
                // Both on top edge
                // Cathode flag
                var cathodeFlag = CreateRectangle(
                    cathodeX + vm.CathodeFlagOffsetX_mm * scale,
                    cathodeY + cathodeH * scale,
                    vm.CathodeFlagWidth_mm * scale,
                    vm.CathodeFlagHeight_mm * scale,
                    Colors.DarkRed, 2.0);
                target.Children.Add(cathodeFlag);

                // Anode flag (gap enforcement handled in VM)
                var anodeFlag = CreateRectangle(
                    cathodeX + vm.AnodeFlagOffsetX_mm * scale,
                    cathodeY + cathodeH * scale,
                    vm.AnodeFlagWidth_mm * scale,
                    vm.AnodeFlagHeight_mm * scale,
                    Colors.DarkOrange, 2.0);
                target.Children.Add(anodeFlag);
            }
        }

        public static void RenderCornerZoom(Canvas target, PouchCellViewModel vm)
        {
            if (target == null || vm == null) return;

            target.Children.Clear();

            // Focus on top-right corner showing offsets
            double anodeOffsetX = vm.AnodeOffsetX_mm;
            double anodeOffsetY = vm.AnodeOffsetY_mm;
            double sepOffsetX = vm.SeparatorOffsetX_mm;
            double sepOffsetY = vm.SeparatorOffsetY_mm;

            // World size: show a region that includes all offsets
            double worldWidth = vm.CathodeWidth_mm * 0.3 + anodeOffsetX + sepOffsetX;
            double worldHeight = vm.CathodeHeight_mm * 0.3 + anodeOffsetY + sepOffsetY;

            // Compute scale
            double padding = 20;
            double availableWidth = target.ActualWidth > 0 ? target.ActualWidth - 2 * padding : 400;
            double availableHeight = target.ActualHeight > 0 ? target.ActualHeight - 2 * padding : 200;

            double scaleX = availableWidth / worldWidth;
            double scaleY = availableHeight / worldHeight;
            double scale = Math.Min(scaleX, scaleY);

            if (scale <= 0) scale = 1;

            double centerX = padding;
            double centerY = padding;

            // Separator outer edge (from corner)
            double sepRight = worldWidth;
            double sepTop = worldHeight;

            // Anode edge (inset by separator offset)
            double anodeRight = sepRight - sepOffsetX;
            double anodeTop = sepTop - sepOffsetY;

            // Cathode edge (inset by anode offset from anode)
            double cathodeRight = anodeRight - anodeOffsetX;
            double cathodeTop = anodeTop - anodeOffsetY;

            // Draw separator edge (light gray, thin)
            var sepLine1 = CreateLine(centerX, centerY + sepTop * scale, centerX + sepRight * scale, centerY + sepTop * scale, Colors.LightGray, 1.0);
            var sepLine2 = CreateLine(centerX + sepRight * scale, centerY, centerX + sepRight * scale, centerY + sepTop * scale, Colors.LightGray, 1.0);
            target.Children.Add(sepLine1);
            target.Children.Add(sepLine2);

            // Draw anode edge (orange, medium)
            var anodeLine1 = CreateLine(centerX, centerY + anodeTop * scale, centerX + anodeRight * scale, centerY + anodeTop * scale, Colors.Orange, 2.0);
            var anodeLine2 = CreateLine(centerX + anodeRight * scale, centerY, centerX + anodeRight * scale, centerY + anodeTop * scale, Colors.Orange, 2.0);
            target.Children.Add(anodeLine1);
            target.Children.Add(anodeLine2);

            // Draw cathode edge (red, bold)
            var cathodeLine1 = CreateLine(centerX, centerY + cathodeTop * scale, centerX + cathodeRight * scale, centerY + cathodeTop * scale, Colors.Red, 3.0);
            var cathodeLine2 = CreateLine(centerX + cathodeRight * scale, centerY, centerX + cathodeRight * scale, centerY + cathodeTop * scale, Colors.Red, 3.0);
            target.Children.Add(cathodeLine1);
            target.Children.Add(cathodeLine2);

            // Optional: Add dashed guide lines for offsets
            if (anodeOffsetX > 0)
            {
                var guideX = CreateDashedLine(
                    centerX + cathodeRight * scale,
                    centerY + cathodeTop * scale,
                    centerX + anodeRight * scale,
                    centerY + cathodeTop * scale,
                    Colors.Gray, 1.0);
                target.Children.Add(guideX);
            }

            if (anodeOffsetY > 0)
            {
                var guideY = CreateDashedLine(
                    centerX + cathodeRight * scale,
                    centerY + cathodeTop * scale,
                    centerX + cathodeRight * scale,
                    centerY + anodeTop * scale,
                    Colors.Gray, 1.0);
                target.Children.Add(guideY);
            }

            if (sepOffsetX > 0)
            {
                var guideSepX = CreateDashedLine(
                    centerX + anodeRight * scale,
                    centerY + anodeTop * scale,
                    centerX + sepRight * scale,
                    centerY + anodeTop * scale,
                    Colors.LightGray, 1.0);
                target.Children.Add(guideSepX);
            }

            if (sepOffsetY > 0)
            {
                var guideSepY = CreateDashedLine(
                    centerX + anodeRight * scale,
                    centerY + anodeTop * scale,
                    centerX + anodeRight * scale,
                    centerY + sepTop * scale,
                    Colors.LightGray, 1.0);
                target.Children.Add(guideSepY);
            }
        }

        /// <summary>
        /// Draws the packaging case preview showing pouch foil and separator with offsets
        /// </summary>
        public static void DrawPackagingCase(Canvas target, PouchCellViewModel vm)
        {
            if (target == null || vm == null) return;

            target.Children.Clear();

            // Get separator dimensions (inner rectangle)
            double sepW = vm.SeparatorWidth_mm;
            double sepH = vm.SeparatorHeight_mm;

            // Get pouch foil offsets from ViewModel
            double offsetTop = vm.PouchOffsetTop_mm;
            double offsetBottom = vm.PouchOffsetBottom_mm;
            double offsetLeft = vm.PouchOffsetLeft_mm;
            double offsetRight = vm.PouchOffsetRight_mm;

            // Calculate pouch foil dimensions (outer rectangle)
            double pouchW = sepW + offsetLeft + offsetRight;
            double pouchH = sepH + offsetTop + offsetBottom;

            // Compute scale with padding
            double padding = 20;
            double availableWidth = target.ActualWidth > 0 ? target.ActualWidth - 2 * padding : 380;
            double availableHeight = target.ActualHeight > 0 ? target.ActualHeight - 2 * padding : 500;

            double scaleX = availableWidth / pouchW;
            double scaleY = availableHeight / pouchH;
            double scale = Math.Min(scaleX, scaleY);

            if (scale <= 0) scale = 1;

            // Center the drawing
            double centerX = padding + (availableWidth - pouchW * scale) / 2.0;
            double centerY = padding + (availableHeight - pouchH * scale) / 2.0;

            // Draw pouch foil (outer rectangle - thicker, dark gray)
            var pouchRect = CreateRectangle(
                centerX,
                centerY,
                pouchW * scale,
                pouchH * scale,
                Colors.DarkGray, 2.5);
            target.Children.Add(pouchRect);

            // Draw separator (inner rectangle - thinner, blue)
            var sepRect = CreateRectangle(
                centerX + offsetLeft * scale,
                centerY + offsetBottom * scale,
                sepW * scale,
                sepH * scale,
                Colors.Blue, 1.5);
            target.Children.Add(sepRect);

            // Add dimension labels (optional - for clarity)
            // Top offset label
            var topLabel = new TextBlock
            {
                Text = $"Top: {offsetTop}mm",
                FontSize = 9,
                Foreground = System.Windows.Media.Brushes.DarkGray
            };
            Canvas.SetLeft(topLabel, centerX + pouchW * scale / 2 - 30);
            Canvas.SetTop(topLabel, centerY - 15);
            target.Children.Add(topLabel);

            // Bottom offset label
            var bottomLabel = new TextBlock
            {
                Text = $"Bottom: {offsetBottom}mm",
                FontSize = 9,
                Foreground = System.Windows.Media.Brushes.DarkGray
            };
            Canvas.SetLeft(bottomLabel, centerX + pouchW * scale / 2 - 40);
            Canvas.SetTop(bottomLabel, centerY + pouchH * scale + 5);
            target.Children.Add(bottomLabel);

            // Left offset label
            var leftLabel = new TextBlock
            {
                Text = $"Left: {offsetLeft}mm",
                FontSize = 9,
                Foreground = System.Windows.Media.Brushes.DarkGray
            };
            Canvas.SetLeft(leftLabel, centerX - 45);
            Canvas.SetTop(leftLabel, centerY + pouchH * scale / 2 - 8);
            target.Children.Add(leftLabel);

            // Right offset label
            var rightLabel = new TextBlock
            {
                Text = $"Right: {offsetRight}mm",
                FontSize = 9,
                Foreground = System.Windows.Media.Brushes.DarkGray
            };
            Canvas.SetLeft(rightLabel, centerX + pouchW * scale + 5);
            Canvas.SetTop(rightLabel, centerY + pouchH * scale / 2 - 8);
            target.Children.Add(rightLabel);
        }

        private static System.Windows.Shapes.Rectangle CreateRectangle(double x, double y, double width, double height, System.Windows.Media.Color strokeColor, double strokeThickness)
        {
            return new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness,
                Fill = System.Windows.Media.Brushes.Transparent,
                RenderTransform = new TranslateTransform(x, y)
            };
        }

        private static Line CreateLine(double x1, double y1, double x2, double y2, System.Windows.Media.Color strokeColor, double strokeThickness)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness
            };
        }

        private static Line CreateDashedLine(double x1, double y1, double x2, double y2, System.Windows.Media.Color strokeColor, double strokeThickness)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness,
                StrokeDashArray = new DoubleCollection { 3, 2 }
            };
        }
    }
}
