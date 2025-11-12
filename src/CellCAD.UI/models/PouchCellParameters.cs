using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CellCAD.models
{
    /// <summary>
    /// Core dimensional parameters for a prismatic pouch cell (millimetres).
    /// Expanded to support full sheet design, stack configuration, overwrap, and tabs.
    /// </summary>
    public class PouchCellParameters : IValidatableObject
    {
        // ========== BODY (unchanged) ==========
        [Range(1, 2_000, ErrorMessage = "Length must be between 1 and 2000 mm.")]
        public double Length_mm { get; set; } = 120;

        [Range(1, 2_000, ErrorMessage = "Width must be between 1 and 2000 mm.")]
        public double Width_mm { get; set; } = 90;

        [Range(0.1, 100, ErrorMessage = "Thickness must be between 0.1 and 100 mm.")]
        public double Thickness_mm { get; set; } = 12;

        [Range(0, 500, ErrorMessage = "Corner radius must be non-negative and reasonable.")]
        public double CornerRadius_mm { get; set; } = 8;

        [Range(0, 50, ErrorMessage = "Seal thickness must be between 0 and 50 mm.")]
        public double SealThickness_mm { get; set; } = 0.5;

        // ========== SHEET DESIGN (electrode dimensions) ==========

        // Cathode
        [Range(1, 1000)] public double CathodeHeight_mm { get; set; } = 375.5;
        [Range(1, 1000)] public double CathodeWidth_mm { get; set; } = 79.0;
        public double CathodeArea_cm2 => (CathodeHeight_mm * CathodeWidth_mm) / 100.0;

        // Anode
        [Range(1, 1000)] public double AnodeHeight_mm { get; set; } = 375.5;
        [Range(1, 1000)] public double AnodeWidth_mm { get; set; } = 84.0;
        public double AnodeArea_cm2 => (AnodeHeight_mm * AnodeWidth_mm) / 100.0;

        // Separator
        [Range(1, 1000)] public double SeparatorHeight_mm { get; set; } = 386.0;
        [Range(1, 1000)] public double SeparatorWidth_mm { get; set; } = 86.0;
        public double SeparatorArea_cm2 => (SeparatorHeight_mm * SeparatorWidth_mm) / 100.0;

        // Offsets to Cathode (anode and separator positioning)
        [Range(0, 100)] public double AnodeOffsetY_mm { get; set; } = 1.0;
        [Range(0, 100)] public double AnodeOffsetX_mm { get; set; } = 1.0;
        [Range(0, 100)] public double SeparatorOffsetY_mm { get; set; } = 2.85;
        [Range(0, 100)] public double SeparatorOffsetX_mm { get; set; } = 2.0;

        // ========== FLAG / TAB POSITIONS ==========
        public bool FlagsOnOppositeSides { get; set; } = true;
        public bool FlagsOnSameSide { get; set; } = false;

        [Range(1, 200)] public double CathodeFlagHeight_mm { get; set; } = 12;
        [Range(1, 200)] public double CathodeFlagWidth_mm { get; set; } = 64;
        [Range(0, 500)] public double CathodeFlagOffsetX_mm { get; set; } = 8.5;

        [Range(1, 200)] public double AnodeFlagHeight_mm { get; set; } = 12;
        [Range(1, 200)] public double AnodeFlagWidth_mm { get; set; } = 64;
        [Range(0, 500)] public double AnodeFlagOffsetX_mm { get; set; } = 8.5;

        // ========== STACK CONFIGURATION ==========
        public bool UpdateStackDimensions { get; set; } = true;
        [Range(1, 1000)] public double StackHeight_mm { get; set; } = 380;
        [Range(1, 1000)] public double StackWidth_mm { get; set; } = 83;

        // ========== OVERWRAP & FIXING TAPE (placeholders for future tabs) ==========
        [Range(0, 1000)] public double TabWidth_mm { get; set; } = 12;
        [Range(0, 1000)] public double TabLength_mm { get; set; } = 20;
        [Range(0, 500)] public double TabOffsetFromEdge_mm { get; set; } = 15;

        // ========== VALIDATION & HELPERS ==========

        /// <summary>
        /// Normalizes numeric fields (clamps negatives to zero where appropriate).
        /// </summary>
        public bool TryNormalize()
        {
            bool changed = false;
            if (Length_mm < 0) { Length_mm = Math.Abs(Length_mm); changed = true; }
            if (Width_mm < 0) { Width_mm = Math.Abs(Width_mm); changed = true; }
            if (Thickness_mm < 0) { Thickness_mm = Math.Abs(Thickness_mm); changed = true; }
            if (CornerRadius_mm < 0) { CornerRadius_mm = 0; changed = true; }
            if (SealThickness_mm < 0) { SealThickness_mm = 0; changed = true; }

            if (CathodeHeight_mm < 0) { CathodeHeight_mm = Math.Abs(CathodeHeight_mm); changed = true; }
            if (CathodeWidth_mm < 0) { CathodeWidth_mm = Math.Abs(CathodeWidth_mm); changed = true; }
            if (AnodeHeight_mm < 0) { AnodeHeight_mm = Math.Abs(AnodeHeight_mm); changed = true; }
            if (AnodeWidth_mm < 0) { AnodeWidth_mm = Math.Abs(AnodeWidth_mm); changed = true; }
            if (SeparatorHeight_mm < 0) { SeparatorHeight_mm = Math.Abs(SeparatorHeight_mm); changed = true; }
            if (SeparatorWidth_mm < 0) { SeparatorWidth_mm = Math.Abs(SeparatorWidth_mm); changed = true; }

            return changed;
        }

        /// <summary>
        /// Default, realistic parameters matching the target layout.
        /// </summary>
        public static PouchCellParameters Default() => new PouchCellParameters
        {
            // Body
            Length_mm = 120,
            Width_mm = 90,
            Thickness_mm = 12,
            CornerRadius_mm = 8,
            SealThickness_mm = 0.5,

            // Sheet dimensions
            CathodeHeight_mm = 375.5,
            CathodeWidth_mm = 79.0,
            AnodeHeight_mm = 375.5,
            AnodeWidth_mm = 84.0,
            SeparatorHeight_mm = 386.0,
            SeparatorWidth_mm = 86.0,

            // Offsets
            AnodeOffsetY_mm = 1.0,
            AnodeOffsetX_mm = 1.0,
            SeparatorOffsetY_mm = 2.85,
            SeparatorOffsetX_mm = 2.0,

            // Flags
            FlagsOnOppositeSides = true,
            CathodeFlagHeight_mm = 12,
            CathodeFlagWidth_mm = 64,
            CathodeFlagOffsetX_mm = 8.5,
            AnodeFlagHeight_mm = 12,
            AnodeFlagWidth_mm = 64,
            AnodeFlagOffsetX_mm = 8.5,

            // Stack
            UpdateStackDimensions = true,
            StackHeight_mm = 380,
            StackWidth_mm = 83,

            // Tabs (future)
            TabWidth_mm = 12,
            TabLength_mm = 20,
            TabOffsetFromEdge_mm = 15
        };

        /// <summary>
        /// Cross-parameter validation rules.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Corner radius check
            var maxR = 0.5 * Math.Min(Length_mm, Width_mm);
            if (CornerRadius_mm > maxR)
            {
                yield return new ValidationResult(
                    $"Corner radius ({CornerRadius_mm:F2} mm) must be ≤ {maxR:F2} mm.",
                    new[] { nameof(CornerRadius_mm) });
            }

            // Seal thickness check
            if (SealThickness_mm > 0.5 * Thickness_mm)
            {
                yield return new ValidationResult(
                    $"Seal thickness ({SealThickness_mm:F2} mm) must be ≤ {0.5 * Thickness_mm:F2} mm.",
                    new[] { nameof(SealThickness_mm) });
            }

            // Anode should be larger than cathode (typical design rule)
            if (AnodeWidth_mm < CathodeWidth_mm || AnodeHeight_mm < CathodeHeight_mm)
            {
                yield return new ValidationResult(
                    "Anode dimensions should be ≥ cathode dimensions for proper coverage.",
                    new[] { nameof(AnodeWidth_mm), nameof(AnodeHeight_mm) });
            }

            // Separator should be largest
            if (SeparatorWidth_mm < AnodeWidth_mm || SeparatorHeight_mm < AnodeHeight_mm)
            {
                yield return new ValidationResult(
                    "Separator dimensions should be ≥ anode dimensions.",
                    new[] { nameof(SeparatorWidth_mm), nameof(SeparatorHeight_mm) });
            }
        }

        public static PouchCellParameters NeutralPreset => Default();
    }
}
