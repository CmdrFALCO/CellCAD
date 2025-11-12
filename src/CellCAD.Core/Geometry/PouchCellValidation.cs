using System.Collections.Generic;

namespace CellCAD.Core.Geometry
{
    public static class PouchCellValidation
    {
        public static IReadOnlyList<string> Validate(PouchCellParameters p)
        {
            var errors = new List<string>();

            // Basic ranges
            if (p.BodyWidth_mm <= 0 || p.BodyHeight_mm <= 0 || p.BodyThickness_mm <= 0)
                errors.Add("Body dimensions must be positive.");

            if (p.SealLeft_mm < PouchCellParameters.MinSeal_mm ||
                p.SealRight_mm < PouchCellParameters.MinSeal_mm ||
                p.SealTop_mm < PouchCellParameters.MinSeal_mm ||
                p.SealBottom_mm < PouchCellParameters.MinSeal_mm)
                errors.Add($"All seals must be ≥ {PouchCellParameters.MinSeal_mm:0.##} mm.");

            var maxCorner = 0.5 * System.Math.Min(p.BodyWidth_mm, p.BodyHeight_mm);
            if (p.CornerRadius_mm < 0 || p.CornerRadius_mm > maxCorner)
                errors.Add("Corner radius must be between 0 and min(width,height)/2.");

            // Stack vs thickness
            var requiredThickness = p.Stack.SealedStackThickness_mm + PouchCellParameters.LaminateAllowance_mm;
            if (requiredThickness > p.BodyThickness_mm)
                errors.Add($"BodyThickness ({p.BodyThickness_mm:0.###} mm) is less than stack requirement ({requiredThickness:0.###} mm).");

            // Tabs fit checks (span depends on edge)
            double spanTop = p.BodyWidth_mm;
            double spanSide = p.BodyHeight_mm;

            bool NeedsFit(double width, int count, double spacing, double span)
            {
                var needed = count * width + (count - 1) * spacing + 2 * PouchCellParameters.TabEdgeClearance_mm;
                return needed <= span;
            }

            var a = p.AnodeTab; var c = p.CathodeTab;

            switch (p.TabPlacement)
            {
                case TabPlacement.Top:
                    if (!NeedsFit(a.Width_mm, a.Count, a.InterTabSpacing_mm, spanTop))
                        errors.Add("Anode tabs do not fit on top edge.");
                    if (!NeedsFit(c.Width_mm, c.Count, c.InterTabSpacing_mm, spanTop))
                        errors.Add("Cathode tabs do not fit on top edge.");
                    break;
                case TabPlacement.BothLeft:
                case TabPlacement.BothRight:
                    if (!NeedsFit(a.Width_mm, a.Count, a.InterTabSpacing_mm, spanSide))
                        errors.Add("Anode tabs do not fit on side edge.");
                    if (!NeedsFit(c.Width_mm, c.Count, c.InterTabSpacing_mm, spanSide))
                        errors.Add("Cathode tabs do not fit on side edge.");
                    break;
                case TabPlacement.SplitLeftRight:
                    if (!NeedsFit(a.Width_mm, a.Count, a.InterTabSpacing_mm, spanSide))
                        errors.Add("Anode tabs do not fit on side edge.");
                    if (!NeedsFit(c.Width_mm, c.Count, c.InterTabSpacing_mm, spanSide))
                        errors.Add("Cathode tabs do not fit on side edge.");
                    break;
            }

            // Overlap must be ≤ the seal on that edge (simple safety)
            double minSideSeal = System.Math.Min(p.SealLeft_mm, p.SealRight_mm);
            if (a.InternalOverlap_mm > minSideSeal || c.InternalOverlap_mm > minSideSeal ||
                a.InternalOverlap_mm > p.SealTop_mm || c.InternalOverlap_mm > p.SealTop_mm)
                errors.Add("Tab InternalOverlap exceeds available seal margin.");

            return errors;
        }
    }
}
