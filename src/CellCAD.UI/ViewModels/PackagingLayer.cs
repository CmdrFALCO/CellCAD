using System;
using System.ComponentModel;

namespace CellCAD.viewmodels
{
    /// <summary>
    /// Represents a single layer in the package material composition (read-only, DB-driven)
    /// </summary>
    public class PackagingLayer : INotifyPropertyChanged
    {
        public int No { get; init; }              // row index or DB order
        public string Name { get; init; } = "";   // e.g., "Al foil 20µm"
        public string Version { get; init; } = ""; // version from DB
        public double Thickness_um { get; init; } // from DB, non-editable here
        public double Porosity_pct { get; init; } // from DB, 0..100
        public double Density_gcm3 { get; init; } // bulk density from DB

        /// <summary>
        /// Effective density after porosity correction
        /// </summary>
        public double EffectiveDensity_gcm3 => Density_gcm3 * Math.Max(0.0, 1.0 - (Porosity_pct / 100.0));

        /// <summary>
        /// Computed areal weight (not shown as column, used for totals):
        /// thickness[µm] × 1e-4 × effectiveDensity[g/cm³] × 1000 = mg/cm²
        /// </summary>
        public double ArealWeight_mgcm2 => Thickness_um * 1e-4 * EffectiveDensity_gcm3 * 1000.0;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
