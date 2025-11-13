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
            set
            {
                if (value < 0) value = 0;
                _model.CathodeHeight_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeArea_cm2));
                OnPropertyChanged(nameof(AnodeHeight_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorHeight_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                // Re-clamp stack if enabled
                if (UpdateStackDimensions) StackHeight_mm = _model.StackHeight_mm;
            }
        }
        public double CathodeWidth_mm
        {
            get => _model.CathodeWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _model.CathodeWidth_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeArea_cm2));
                OnPropertyChanged(nameof(AnodeWidth_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorWidth_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                // Re-clamp stack if enabled
                if (UpdateStackDimensions) StackWidth_mm = _model.StackWidth_mm;
            }
        }
        public double CathodeArea_cm2 => _model.CathodeArea_cm2;

        // Calculated properties based on cathode + offsets
        public double AnodeHeight_mm => CathodeHeight_mm + 2 * AnodeOffsetY_mm;
        public double AnodeWidth_mm => CathodeWidth_mm + 2 * AnodeOffsetX_mm;
        public double AnodeArea_cm2 => (AnodeWidth_mm * AnodeHeight_mm) / 100.0;

        public double SeparatorHeight_mm => AnodeHeight_mm + 2 * SeparatorOffsetY_mm;
        public double SeparatorWidth_mm => AnodeWidth_mm + 2 * SeparatorOffsetX_mm;
        public double SeparatorArea_cm2 => (SeparatorWidth_mm * SeparatorHeight_mm) / 100.0;

        // Offsets
        public double AnodeOffsetY_mm
        {
            get => _model.AnodeOffsetY_mm;
            set
            {
                if (value < 0) value = 0;
                _model.AnodeOffsetY_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeHeight_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorHeight_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                if (UpdateStackDimensions) StackHeight_mm = _model.StackHeight_mm;
            }
        }
        public double AnodeOffsetX_mm
        {
            get => _model.AnodeOffsetX_mm;
            set
            {
                if (value < 0) value = 0;
                _model.AnodeOffsetX_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeWidth_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorWidth_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                if (UpdateStackDimensions) StackWidth_mm = _model.StackWidth_mm;
            }
        }
        public double SeparatorOffsetY_mm
        {
            get => _model.SeparatorOffsetY_mm;
            set
            {
                if (value < 0) value = 0;
                _model.SeparatorOffsetY_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SeparatorHeight_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                if (UpdateStackDimensions) StackHeight_mm = _model.StackHeight_mm;
            }
        }
        public double SeparatorOffsetX_mm
        {
            get => _model.SeparatorOffsetX_mm;
            set
            {
                if (value < 0) value = 0;
                _model.SeparatorOffsetX_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SeparatorWidth_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
                OnPropertyChanged(nameof(IsValid));
                if (UpdateStackDimensions) StackWidth_mm = _model.StackWidth_mm;
            }
        }

        // ========== FLAGS ==========
        public bool FlagsOnOppositeSides
        {
            get => _model.FlagsOnOppositeSides;
            set
            {
                if (_model.FlagsOnOppositeSides != value)
                {
                    _model.FlagsOnOppositeSides = value;
                    if (value) _model.FlagsOnSameSide = false; // Mutual exclusion
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FlagsOnSameSide));
                }
            }
        }
        public bool FlagsOnSameSide
        {
            get => _model.FlagsOnSameSide;
            set
            {
                if (_model.FlagsOnSameSide != value)
                {
                    _model.FlagsOnSameSide = value;
                    if (value) _model.FlagsOnOppositeSides = false; // Mutual exclusion
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FlagsOnOppositeSides));
                }
            }
        }

        public double CathodeFlagHeight_mm
        {
            get => _model.CathodeFlagHeight_mm;
            set
            {
                if (value < 0) value = 0;
                _model.CathodeFlagHeight_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }
        public double CathodeFlagWidth_mm
        {
            get => _model.CathodeFlagWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _model.CathodeFlagWidth_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }
        public double CathodeFlagOffsetX_mm
        {
            get => _model.CathodeFlagOffsetX_mm;
            set
            {
                if (value < 0) value = 0;
                _model.CathodeFlagOffsetX_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }

        public double AnodeFlagHeight_mm
        {
            get => _model.AnodeFlagHeight_mm;
            set
            {
                if (value < 0) value = 0;
                _model.AnodeFlagHeight_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }
        public double AnodeFlagWidth_mm
        {
            get => _model.AnodeFlagWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _model.AnodeFlagWidth_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }
        public double AnodeFlagOffsetX_mm
        {
            get => _model.AnodeFlagOffsetX_mm;
            set
            {
                if (value < 0) value = 0;
                _model.AnodeFlagOffsetX_mm = value;
                OnPropertyChanged();
                ValidateFlagOverlap();
            }
        }

        // Validate and fix flag overlap when on same side
        private void ValidateFlagOverlap()
        {
            if (!FlagsOnSameSide) return;

            const double minGap = 2.0; // mm
            double cathodeEnd = CathodeFlagOffsetX_mm + CathodeFlagWidth_mm;
            double anodeStart = AnodeFlagOffsetX_mm;

            // If anode flag would overlap, shift it to maintain 2mm gap
            if (anodeStart < cathodeEnd + minGap)
            {
                _model.AnodeFlagOffsetX_mm = cathodeEnd + minGap;
                OnPropertyChanged(nameof(AnodeFlagOffsetX_mm));
            }
        }

        // ========== STACK ==========
        public bool UpdateStackDimensions
        {
            get => _model.UpdateStackDimensions;
            set
            {
                _model.UpdateStackDimensions = value;
                OnPropertyChanged();
                if (value)
                {
                    // Re-clamp when enabling
                    StackHeight_mm = _model.StackHeight_mm;
                    StackWidth_mm = _model.StackWidth_mm;
                }
            }
        }
        public double StackHeight_mm
        {
            get => _model.StackHeight_mm;
            set
            {
                if (UpdateStackDimensions)
                {
                    // Clamp between anode and separator
                    double min = AnodeHeight_mm;
                    double max = SeparatorHeight_mm;
                    if (value < min) value = min;
                    if (value > max) value = max;
                }
                _model.StackHeight_mm = value;
                OnPropertyChanged();
            }
        }
        public double StackWidth_mm
        {
            get => _model.StackWidth_mm;
            set
            {
                if (UpdateStackDimensions)
                {
                    // Clamp between anode and separator
                    double min = AnodeWidth_mm;
                    double max = SeparatorWidth_mm;
                    if (value < min) value = min;
                    if (value > max) value = max;
                }
                _model.StackWidth_mm = value;
                OnPropertyChanged();
            }
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

        // ========== PACKAGING / CASE ==========

        // Pouch foil offsets (mm) - drive preview
        private double _pouchOffsetTop_mm = 18.0;
        public double PouchOffsetTop_mm
        {
            get => _pouchOffsetTop_mm;
            set
            {
                if (value < 0) value = 0;
                _pouchOffsetTop_mm = value;
                OnPropertyChanged();
            }
        }

        private double _pouchOffsetBottom_mm = 18.0;
        public double PouchOffsetBottom_mm
        {
            get => _pouchOffsetBottom_mm;
            set
            {
                if (value < 0) value = 0;
                _pouchOffsetBottom_mm = value;
                OnPropertyChanged();
            }
        }

        private double _pouchOffsetLeft_mm = 5.0;
        public double PouchOffsetLeft_mm
        {
            get => _pouchOffsetLeft_mm;
            set
            {
                if (value < 0) value = 0;
                _pouchOffsetLeft_mm = value;
                OnPropertyChanged();
            }
        }

        private double _pouchOffsetRight_mm = 5.0;
        public double PouchOffsetRight_mm
        {
            get => _pouchOffsetRight_mm;
            set
            {
                if (value < 0) value = 0;
                _pouchOffsetRight_mm = value;
                OnPropertyChanged();
            }
        }

        // Package Material Properties (TwoWay, editable)
        private double _packageThicknessSum_um = 13.0;
        public double PackageThicknessSum_um
        {
            get => _packageThicknessSum_um;
            set
            {
                if (value < 0) value = 0;
                _packageThicknessSum_um = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        private double _packageArealWeight_mgcm2 = 0.725;
        public double PackageArealWeight_mgcm2
        {
            get => _packageArealWeight_mgcm2;
            set
            {
                if (value < 0) value = 0;
                _packageArealWeight_mgcm2 = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        private double _packageEffDensity_gcm3 = 500.0;
        public double PackageEffDensity_gcm3
        {
            get => _packageEffDensity_gcm3;
            set
            {
                if (value < 0) value = 0;
                _packageEffDensity_gcm3 = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        private double _packageCost_EURm2 = 0.4;
        public double PackageCost_EURm2
        {
            get => _packageCost_EURm2;
            set
            {
                if (value < 0) value = 0;
                _packageCost_EURm2 = value;
                OnPropertyChanged();
            }
        }

        // Right-hand notes (editable strings)
        private string _packageThicknessNote = "CoA xyz";
        public string PackageThicknessNote
        {
            get => _packageThicknessNote;
            set { _packageThicknessNote = value; OnPropertyChanged(); }
        }

        private string _packageArealWeightNote = "Measured @CL 10.10.25";
        public string PackageArealWeightNote
        {
            get => _packageArealWeightNote;
            set { _packageArealWeightNote = value; OnPropertyChanged(); }
        }

        private string _packageEffDensityNote = "Lit. from xyz";
        public string PackageEffDensityNote
        {
            get => _packageEffDensityNote;
            set { _packageEffDensityNote = value; OnPropertyChanged(); }
        }

        private string _packageCostNote = "REC Value from 05.10.25";
        public string PackageCostNote
        {
            get => _packageCostNote;
            set { _packageCostNote = value; OnPropertyChanged(); }
        }

        // Cell Dimensions - Calculated (from separator + offsets)
        private double _calculatedWidth_mm = 100.0;
        public double CalculatedWidth_mm
        {
            get => _calculatedWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _calculatedWidth_mm = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        private double _calculatedHeight_mm = 150.0;
        public double CalculatedHeight_mm
        {
            get => _calculatedHeight_mm;
            set
            {
                if (value < 0) value = 0;
                _calculatedHeight_mm = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        // Cell Dimensions - Measured (user input)
        private double _measuredWidth_mm = 0.0;
        public double MeasuredWidth_mm
        {
            get => _measuredWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _measuredWidth_mm = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        private double _measuredHeight_mm = 0.0;
        public double MeasuredHeight_mm
        {
            get => _measuredHeight_mm;
            set
            {
                if (value < 0) value = 0;
                _measuredHeight_mm = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        // Toggle for using Calculated vs Measured dimensions
        private bool _useMeasuredDims = false;
        public bool UseMeasuredDims
        {
            get => _useMeasuredDims;
            set
            {
                _useMeasuredDims = value;
                OnPropertyChanged();
                RaiseCaseMassChain();
            }
        }

        // Active outer dimensions (based on toggle)
        public double CaseOuterWidth_mm => UseMeasuredDims ? MeasuredWidth_mm : CalculatedWidth_mm;
        public double CaseOuterHeight_mm => UseMeasuredDims ? MeasuredHeight_mm : CalculatedHeight_mm;

        // Package area in cm²
        public double PackageArea_cm2 => Math.Max(0, (CaseOuterWidth_mm * CaseOuterHeight_mm) / 100.0);

        // Computed areal weight from thickness & density (mg/cm²)
        // thickness[µm] → cm (1 µm = 1e-4 cm)
        // g/cm³ × cm = g/cm² → mg/cm² (*1000)
        public double PackageArealWeight_mgcm2_calc =>
            Math.Max(0, PackageThicknessSum_um * 1e-4 * PackageEffDensity_gcm3 * 1000.0);

        // Effective areal weight: prefer user input if > 0, else computed
        public double PackageArealWeight_mgcm2_effective =>
            (PackageArealWeight_mgcm2 > 0.0) ? PackageArealWeight_mgcm2 : PackageArealWeight_mgcm2_calc;

        // Calculated package mass [g]
        // mg/cm² × cm² = mg → g (/1000)
        public double PackageMass_g => Math.Max(0, PackageArea_cm2 * PackageArealWeight_mgcm2_effective / 1000.0);

        // Helper to raise all dependent properties for mass calculation
        private void RaiseCaseMassChain()
        {
            OnPropertyChanged(nameof(CaseOuterWidth_mm));
            OnPropertyChanged(nameof(CaseOuterHeight_mm));
            OnPropertyChanged(nameof(PackageArea_cm2));
            OnPropertyChanged(nameof(PackageArealWeight_mgcm2_calc));
            OnPropertyChanged(nameof(PackageArealWeight_mgcm2_effective));
            OnPropertyChanged(nameof(PackageMass_g));
        }
    }
}
