using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CellCAD.Core.Geometry;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CellCAD.viewmodels
{
    public partial class PouchCellViewModel : ObservableObject, IDataErrorInfo
    {
        private PouchCellParameters _model;

        public PouchCellViewModel() : this(PouchCellParameters.Default()) { }
        public PouchCellViewModel(PouchCellParameters model)
        {
            _model = model ?? PouchCellParameters.Default();
        }

        public PouchCellParameters Model
        {
            get => _model;
            set { if (value == null) return; _model = value;OnPropertyChanged(nameof(Model)); RaiseAll(); }
        }

        // ========== BODY ==========
        public double Length_mm
        {
            get => _model.Length_mm;
            set { _model.Length_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double Width_mm
        {
            get => _model.Width_mm;
            set { _model.Width_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double Thickness_mm
        {
            get => _model.Thickness_mm;
            set { _model.Thickness_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double CornerRadius_mm
        {
            get => _model.CornerRadius_mm;
            set { _model.CornerRadius_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double SealThickness_mm
        {
            get => _model.SealThickness_mm;
            set { _model.SealThickness_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }

        // ========== SHEET DESIGN ==========
        public double CathodeHeight_mm
        {
            get => _model.CathodeHeight_mm;
            set { _model.CathodeHeight_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double CathodeWidth_mm
        {
            get => _model.CathodeWidth_mm;
            set { _model.CathodeWidth_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double CathodeArea_cm2 => _model.CathodeArea_cm2;

        public double AnodeHeight_mm
        {
            get => _model.AnodeHeight_mm;
            set { _model.AnodeHeight_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double AnodeWidth_mm
        {
            get => _model.AnodeWidth_mm;
            set { _model.AnodeWidth_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double AnodeArea_cm2 => _model.AnodeArea_cm2;

        public double SeparatorHeight_mm
        {
            get => _model.SeparatorHeight_mm;
            set { _model.SeparatorHeight_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(SeparatorArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double SeparatorWidth_mm
        {
            get => _model.SeparatorWidth_mm;
            set { _model.SeparatorWidth_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(SeparatorArea_cm2));OnPropertyChanged(nameof(IsValid)); }
        }
        public double SeparatorArea_cm2 => _model.SeparatorArea_cm2;

        // Offsets
        public double AnodeOffsetY_mm
        {
            get => _model.AnodeOffsetY_mm;
            set { _model.AnodeOffsetY_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double AnodeOffsetX_mm
        {
            get => _model.AnodeOffsetX_mm;
            set { _model.AnodeOffsetX_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double SeparatorOffsetY_mm
        {
            get => _model.SeparatorOffsetY_mm;
            set { _model.SeparatorOffsetY_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }
        public double SeparatorOffsetX_mm
        {
            get => _model.SeparatorOffsetX_mm;
            set { _model.SeparatorOffsetX_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(IsValid)); }
        }

        // ========== FLAGS ==========
        public bool FlagsOnOppositeSides
        {
            get => _model.FlagsOnOppositeSides;
            set { _model.FlagsOnOppositeSides = value;OnPropertyChanged(); }
        }
        public bool FlagsOnSameSide
        {
            get => _model.FlagsOnSameSide;
            set { _model.FlagsOnSameSide = value;OnPropertyChanged(); }
        }

        public double CathodeFlagHeight_mm
        {
            get => _model.CathodeFlagHeight_mm;
            set { _model.CathodeFlagHeight_mm = value;OnPropertyChanged(); }
        }
        public double CathodeFlagWidth_mm
        {
            get => _model.CathodeFlagWidth_mm;
            set { _model.CathodeFlagWidth_mm = value;OnPropertyChanged(); }
        }
        public double CathodeFlagOffsetX_mm
        {
            get => _model.CathodeFlagOffsetX_mm;
            set { _model.CathodeFlagOffsetX_mm = value;OnPropertyChanged(); }
        }

        public double AnodeFlagHeight_mm
        {
            get => _model.AnodeFlagHeight_mm;
            set { _model.AnodeFlagHeight_mm = value;OnPropertyChanged(); }
        }
        public double AnodeFlagWidth_mm
        {
            get => _model.AnodeFlagWidth_mm;
            set { _model.AnodeFlagWidth_mm = value;OnPropertyChanged(); }
        }
        public double AnodeFlagOffsetX_mm
        {
            get => _model.AnodeFlagOffsetX_mm;
            set { _model.AnodeFlagOffsetX_mm = value;OnPropertyChanged(); }
        }

        // ========== STACK ==========
        public bool UpdateStackDimensions
        {
            get => _model.UpdateStackDimensions;
            set { _model.UpdateStackDimensions = value;OnPropertyChanged(); }
        }
        public double StackHeight_mm
        {
            get => _model.StackHeight_mm;
            set { _model.StackHeight_mm = value;OnPropertyChanged(); }
        }
        public double StackWidth_mm
        {
            get => _model.StackWidth_mm;
            set { _model.StackWidth_mm = value;OnPropertyChanged(); }
        }

        // ========== VALIDATION ==========
        public bool IsValid => !GetErrors().Any();
        public string Error => string.Join("; ", GetErrors().Select(e => e.ErrorMessage));

        public string this[string columnName]
        {
            get
            {
                var results = new List<ValidationResult>();
                var ctx = new ValidationContext(_model) { MemberName = columnName };

                Validator.TryValidateProperty(
                    value: GetType().GetProperty(columnName)?.GetValue(this, null),
                    validationContext: ctx,
                    validationResults: results);

                results.AddRange(_model.Validate(new ValidationContext(_model))
                    .Where(r => r.MemberNames.Contains(columnName)));

                return results.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }

        public void Normalize()
        {
            if (_model.TryNormalize())
            {
                RaiseAll();
            }
        }

        public IEnumerable<ValidationResult> GetErrors()
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(_model);
            Validator.TryValidateObject(_model, ctx, results, validateAllProperties: true);
            results.AddRange(_model.Validate(ctx));
            return results;
        }

        private void RaiseAll()
        {
            // Body
OnPropertyChanged(nameof(Length_mm));
OnPropertyChanged(nameof(Width_mm));
OnPropertyChanged(nameof(Thickness_mm));
OnPropertyChanged(nameof(CornerRadius_mm));
OnPropertyChanged(nameof(SealThickness_mm));

            // Sheet design
OnPropertyChanged(nameof(CathodeHeight_mm));
OnPropertyChanged(nameof(CathodeWidth_mm));
OnPropertyChanged(nameof(CathodeArea_cm2));
OnPropertyChanged(nameof(AnodeHeight_mm));
OnPropertyChanged(nameof(AnodeWidth_mm));
OnPropertyChanged(nameof(AnodeArea_cm2));
OnPropertyChanged(nameof(SeparatorHeight_mm));
OnPropertyChanged(nameof(SeparatorWidth_mm));
OnPropertyChanged(nameof(SeparatorArea_cm2));

            // Offsets
OnPropertyChanged(nameof(AnodeOffsetY_mm));
OnPropertyChanged(nameof(AnodeOffsetX_mm));
OnPropertyChanged(nameof(SeparatorOffsetY_mm));
OnPropertyChanged(nameof(SeparatorOffsetX_mm));

            // Flags
OnPropertyChanged(nameof(FlagsOnOppositeSides));
OnPropertyChanged(nameof(FlagsOnSameSide));
OnPropertyChanged(nameof(CathodeFlagHeight_mm));
OnPropertyChanged(nameof(CathodeFlagWidth_mm));
OnPropertyChanged(nameof(CathodeFlagOffsetX_mm));
OnPropertyChanged(nameof(AnodeFlagHeight_mm));
OnPropertyChanged(nameof(AnodeFlagWidth_mm));
OnPropertyChanged(nameof(AnodeFlagOffsetX_mm));

            // Stack
OnPropertyChanged(nameof(UpdateStackDimensions));
OnPropertyChanged(nameof(StackHeight_mm));
OnPropertyChanged(nameof(StackWidth_mm));

            // Tab design
OnPropertyChanged(nameof(AnodeTabMaterial));
OnPropertyChanged(nameof(AnodeTabVersion));
OnPropertyChanged(nameof(AnodeTabHeight_mm));
OnPropertyChanged(nameof(AnodeTabWidth_mm));
OnPropertyChanged(nameof(AnodeTabThickness_mm));
OnPropertyChanged(nameof(AnodeTabOverlap_mm));
OnPropertyChanged(nameof(AnodeTabMass_g));

OnPropertyChanged(nameof(CathodeTabMaterial));
OnPropertyChanged(nameof(CathodeTabVersion));
OnPropertyChanged(nameof(CathodeTabHeight_mm));
OnPropertyChanged(nameof(CathodeTabWidth_mm));
OnPropertyChanged(nameof(CathodeTabThickness_mm));
OnPropertyChanged(nameof(CathodeTabOverlap_mm));
OnPropertyChanged(nameof(CathodeTabMass_g));

OnPropertyChanged(nameof(IsValid));
OnPropertyChanged(nameof(Error));
        }
        // Stack Configuration properties
        private int _numberOfStacks = 2;
        public int NumberOfStacks
        {
            get => _numberOfStacks;
            set { _numberOfStacks = value;OnPropertyChanged();OnPropertyChanged(nameof(NumberOfElectrodePairsInCell)); }
        }

        private int _numberOfElectrodePairsInStack = 50;
        public int NumberOfElectrodePairsInStack
        {
            get => _numberOfElectrodePairsInStack;
            set { _numberOfElectrodePairsInStack = value;OnPropertyChanged();OnPropertyChanged(nameof(NumberOfElectrodePairsInCell)); }
        }

        public int NumberOfElectrodePairsInCell => NumberOfStacks * NumberOfElectrodePairsInStack;

        private int _separatorOverwraps = 1;
        public int SeparatorOverwraps
        {
            get => _separatorOverwraps;
            set { _separatorOverwraps = value;OnPropertyChanged(); }
        }

        private int _additionalOverwrap = 0;
        public int AdditionalOverwrap
        {
            get => _additionalOverwrap;
            set { _additionalOverwrap = value;OnPropertyChanged(); }
        }

        private int _insulationShell = 1;
        public int InsulationShell
        {
            get => _insulationShell;
            set { _insulationShell = value;OnPropertyChanged(); }
        }

        private int _fixingTapes = 1;
        public int FixingTapes
        {
            get => _fixingTapes;
            set { _fixingTapes = value;OnPropertyChanged(); }
        }

        // Packaging properties
        private double _pouchFoilOffsetTop = 18;
        public double PouchFoilOffsetTop_mm
        {
            get => _pouchFoilOffsetTop;
            set { _pouchFoilOffsetTop = value;OnPropertyChanged(); }
        }

        private double _pouchFoilOffsetBottom = 18;
        public double PouchFoilOffsetBottom_mm
        {
            get => _pouchFoilOffsetBottom;
            set { _pouchFoilOffsetBottom = value;OnPropertyChanged(); }
        }

        private double _pouchFoilOffsetLeft = 5;
        public double PouchFoilOffsetLeft_mm
        {
            get => _pouchFoilOffsetLeft;
            set { _pouchFoilOffsetLeft = value;OnPropertyChanged(); }
        }

        private double _pouchFoilOffsetRight = 5;
        public double PouchFoilOffsetRight_mm
        {
            get => _pouchFoilOffsetRight;
            set { _pouchFoilOffsetRight = value;OnPropertyChanged(); }
        }

        // ========== TAB DESIGN PROPERTIES ==========

        // Anode Tab
        private string _anodeTabMaterial = "Copper mix";
        public string AnodeTabMaterial
        {
            get => _anodeTabMaterial;
            set { _anodeTabMaterial = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeTabMass_g)); }
        }

        private int _anodeTabVersion = 1;
        public int AnodeTabVersion
        {
            get => _anodeTabVersion;
            set { _anodeTabVersion = value;OnPropertyChanged(); }
        }

        private double _anodeTabHeight_mm = 38;
        public double AnodeTabHeight_mm
        {
            get => _anodeTabHeight_mm;
            set { _anodeTabHeight_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeTabMass_g)); }
        }

        private double _anodeTabWidth_mm = 65;
        public double AnodeTabWidth_mm
        {
            get => _anodeTabWidth_mm;
            set { _anodeTabWidth_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeTabMass_g)); }
        }

        private double _anodeTabThickness_mm = 0.3;
        public double AnodeTabThickness_mm
        {
            get => _anodeTabThickness_mm;
            set { _anodeTabThickness_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(AnodeTabMass_g)); }
        }

        private double _anodeTabOverlap_mm = 5;
        public double AnodeTabOverlap_mm
        {
            get => _anodeTabOverlap_mm;
            set { _anodeTabOverlap_mm = value;OnPropertyChanged(); }
        }

        // Calculated: Anode tab mass (copper density ≈ 8.96 g/cm³)
        public double AnodeTabMass_g
        {
            get
            {
                // Volume in cm³: (height * width * thickness) / 1000
                double volume_cm3 = (AnodeTabHeight_mm * AnodeTabWidth_mm * AnodeTabThickness_mm) / 1000.0;
                // Copper density: 8.96 g/cm³
                double copperDensity = 8.96;
                return volume_cm3 * copperDensity;
            }
        }

        // Cathode Tab
        private string _cathodeTabMaterial = "Aluminum";
        public string CathodeTabMaterial
        {
            get => _cathodeTabMaterial;
            set { _cathodeTabMaterial = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeTabMass_g)); }
        }

        private int _cathodeTabVersion = 2;
        public int CathodeTabVersion
        {
            get => _cathodeTabVersion;
            set { _cathodeTabVersion = value;OnPropertyChanged(); }
        }

        private double _cathodeTabHeight_mm = 38;
        public double CathodeTabHeight_mm
        {
            get => _cathodeTabHeight_mm;
            set { _cathodeTabHeight_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeTabMass_g)); }
        }

        private double _cathodeTabWidth_mm = 65;
        public double CathodeTabWidth_mm
        {
            get => _cathodeTabWidth_mm;
            set { _cathodeTabWidth_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeTabMass_g)); }
        }

        private double _cathodeTabThickness_mm = 0.5;
        public double CathodeTabThickness_mm
        {
            get => _cathodeTabThickness_mm;
            set { _cathodeTabThickness_mm = value;OnPropertyChanged();OnPropertyChanged(nameof(CathodeTabMass_g)); }
        }

        private double _cathodeTabOverlap_mm = 5;
        public double CathodeTabOverlap_mm
        {
            get => _cathodeTabOverlap_mm;
            set { _cathodeTabOverlap_mm = value;OnPropertyChanged(); }
        }

        // Calculated: Cathode tab mass (aluminum density ≈ 2.70 g/cm³)
        public double CathodeTabMass_g
        {
            get
            {
                // Volume in cm³: (height * width * thickness) / 1000
                double volume_cm3 = (CathodeTabHeight_mm * CathodeTabWidth_mm * CathodeTabThickness_mm) / 1000.0;
                // Aluminum density: 2.70 g/cm³ 
                double aluminumDensity = 2.70;
                return volume_cm3 * aluminumDensity;
            }
        }
    }
}
