using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            // Hook up auto-sum for Case composition
            CaseComposition.CollectionChanged += (_, __) => RecalcFromComposition();

            // Hook up auto-sum for Overwrap and InsShell compositions
            OverwrapComposition.CollectionChanged += (_, __) => RecalcOverwrapFromComposition();
            InsShellComposition.CollectionChanged += (_, __) => RecalcInsShellFromComposition();

            // Hook up auto-sum for FixingTape composition
            FixingTapeComposition.CollectionChanged += (_, __) => RecalcFixingTapeFromComposition();

            // Populate with sample data (will be replaced by DB load later)
            LoadSampleComposition();
            LoadSampleOverwrapComposition();
            LoadSampleInsShellComposition();
            LoadSampleFixingTapeComposition();
        }

        public PouchCellParameters Model
        {
            get => _model;
            set { if (value == null) return; _model = value;OnPropertyChanged(nameof(Model)); RaiseAll(); }
        }

        // Cell type selection (Pouch / Prismatic / Cylindrical)
        private CellType _selectedCellType = CellType.Pouch;
        public CellType SelectedCellType
        {
            get => _selectedCellType;
            set
            {
                _selectedCellType = value;
                OnPropertyChanged();
            }
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

OnPropertyChanged(nameof(TotalTabMassPerCell_g));

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

        // ========== TAB DESIGN HELPERS ==========

        /// <summary>
        /// Returns material density in g/cm³ based on material name
        /// </summary>
        private static double GetTabMaterialDensity(string material)
        {
            if (string.IsNullOrWhiteSpace(material))
                return 8.0; // generic fallback

            var m = material.Trim();
            if (m.Contains("Copper", StringComparison.OrdinalIgnoreCase))
                return 8.96;  // Cu

            if (m.Contains("Al", StringComparison.OrdinalIgnoreCase))
                return 2.70;  // Al

            // fallback for other metals (e.g., Ni-plated Cu)
            return 8.0;
        }

        // ========== ANODE TAB ==========

        private string _anodeTabMaterial = "Copper mix";
        public string AnodeTabMaterial
        {
            get => _anodeTabMaterial;
            set
            {
                _anodeTabMaterial = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
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
            set
            {
                _anodeTabHeight_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _anodeTabWidth_mm = 65;
        public double AnodeTabWidth_mm
        {
            get => _anodeTabWidth_mm;
            set
            {
                _anodeTabWidth_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _anodeTabThickness_mm = 0.3;
        public double AnodeTabThickness_mm
        {
            get => _anodeTabThickness_mm;
            set
            {
                _anodeTabThickness_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _anodeTabOverlap_mm = 5;
        public double AnodeTabOverlap_mm
        {
            get => _anodeTabOverlap_mm;
            set { _anodeTabOverlap_mm = value;OnPropertyChanged(); }
        }

        /// <summary>
        /// Calculated: Anode tab mass using material-dependent density
        /// </summary>
        public double AnodeTabMass_g
        {
            get
            {
                var density = GetTabMaterialDensity(AnodeTabMaterial);
                var volume_mm3 = AnodeTabHeight_mm * AnodeTabWidth_mm * AnodeTabThickness_mm;
                var volume_cm3 = volume_mm3 / 1000.0; // mm³ → cm³
                var mass_g = volume_cm3 * density;
                return Math.Max(0.0, mass_g);
            }
        }

        // ========== CATHODE TAB ==========

        private string _cathodeTabMaterial = "Aluminum";
        public string CathodeTabMaterial
        {
            get => _cathodeTabMaterial;
            set
            {
                _cathodeTabMaterial = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
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
            set
            {
                _cathodeTabHeight_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _cathodeTabWidth_mm = 65;
        public double CathodeTabWidth_mm
        {
            get => _cathodeTabWidth_mm;
            set
            {
                _cathodeTabWidth_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _cathodeTabThickness_mm = 0.5;
        public double CathodeTabThickness_mm
        {
            get => _cathodeTabThickness_mm;
            set
            {
                _cathodeTabThickness_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CathodeTabMass_g));
                OnPropertyChanged(nameof(TotalTabMassPerCell_g));
            }
        }

        private double _cathodeTabOverlap_mm = 5;
        public double CathodeTabOverlap_mm
        {
            get => _cathodeTabOverlap_mm;
            set { _cathodeTabOverlap_mm = value;OnPropertyChanged(); }
        }

        /// <summary>
        /// Calculated: Cathode tab mass using material-dependent density
        /// </summary>
        public double CathodeTabMass_g
        {
            get
            {
                var density = GetTabMaterialDensity(CathodeTabMaterial);
                var volume_mm3 = CathodeTabHeight_mm * CathodeTabWidth_mm * CathodeTabThickness_mm;
                var volume_cm3 = volume_mm3 / 1000.0; // mm³ → cm³
                var mass_g = volume_cm3 * density;
                return Math.Max(0.0, mass_g);
            }
        }

        /// <summary>
        /// Total tab mass per cell (one anode + one cathode tab)
        /// </summary>
        public double TotalTabMassPerCell_g => AnodeTabMass_g + CathodeTabMass_g;

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

        // ========== PACKAGING → CASE: Composition ==========

        /// <summary>
        /// Read-only composition backing the Case tab (layers loaded from DB)
        /// </summary>
        public ObservableCollection<PackagingLayer> CaseComposition { get; } = new();

        /// <summary>
        /// Optional: allow turning auto-sync on/off later if needed
        /// </summary>
        private bool _autoSyncFromComposition = true;
        public bool AutoSyncFromComposition
        {
            get => _autoSyncFromComposition;
            set { _autoSyncFromComposition = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Recalculate totals from CaseComposition and push into the three properties
        /// Uses porosity-adjusted effective density for each layer
        /// </summary>
        private void RecalcFromComposition()
        {
            if (!AutoSyncFromComposition) return;

            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0; // Σ(t_i * ρ_eff_i) with t in µm, ρ_eff in g/cm³
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in CaseComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3   += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2  += L.ArealWeight_mgcm2;
            }

            // Effective density of the laminate:
            // ρ_eff,laminate = (Σ t_i ρ_eff_i) / (Σ t_i), with t in consistent length units (µm cancels out in ratio)
            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            // Push into the three properties (these are user-editable, but overwritten on composition change)
            // Set backing fields directly to avoid triggering RaiseCaseMassChain multiple times
            _packageThicknessSum_um   = Math.Max(0, sumThickness_um);
            _packageArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _packageEffDensity_gcm3   = Math.Max(0, effDensity_gcm3);

            // Raise property changed notifications
            OnPropertyChanged(nameof(PackageThicknessSum_um));
            OnPropertyChanged(nameof(PackageArealWeight_mgcm2));
            OnPropertyChanged(nameof(PackageEffDensity_gcm3));

            // Raise dependent chains
            RaiseCaseMassChain();
        }

        /// <summary>
        /// Load sample composition data (will be replaced by DB load later)
        /// </summary>
        private void LoadSampleComposition()
        {
            CaseComposition.Clear();
            CaseComposition.Add(new PackagingLayer
            {
                No = 1,
                Name = "PA 15µm",
                Version = "1 – 01.10.25",
                Thickness_um = 15.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 1.15
            });
            CaseComposition.Add(new PackagingLayer
            {
                No = 2,
                Name = "Al foil 40µm",
                Version = "2 – 15.09.25",
                Thickness_um = 40.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 2.70
            });
            CaseComposition.Add(new PackagingLayer
            {
                No = 3,
                Name = "PP 60µm",
                Version = "1 – 20.08.25",
                Thickness_um = 60.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 0.90
            });
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

        #region Additional Foils (Overwrap & Insulation Shell)

        /// <summary>
        /// Overwrap composition (layers loaded from DB)
        /// </summary>
        public ObservableCollection<PackagingLayer> OverwrapComposition { get; } = new();

        /// <summary>
        /// Insulation Shell composition (layers loaded from DB)
        /// </summary>
        public ObservableCollection<PackagingLayer> InsShellComposition { get; } = new();

        /// <summary>
        /// Material presets for Overwrap dropdown
        /// </summary>
        public IReadOnlyList<string> OverwrapMaterialPresets { get; } = new[] { "PET 25µm", "PET 50µm", "Custom" };

        /// <summary>
        /// Material presets for Insulation Shell dropdown
        /// </summary>
        public IReadOnlyList<string> InsShellMaterialPresets { get; } = new[] { "PET 50µm", "PET 75µm", "Custom" };

        /// <summary>
        /// Recalculate totals from OverwrapComposition
        /// </summary>
        private void RecalcOverwrapFromComposition()
        {
            if (!AutoSyncFromComposition) return;

            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0;
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in OverwrapComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3   += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2  += L.ArealWeight_mgcm2;
            }

            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            _overwrapThicknessSum_um   = Math.Max(0, sumThickness_um);
            _overwrapArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _overwrapEffDensity_gcm3   = Math.Max(0, effDensity_gcm3);

            OnPropertyChanged(nameof(OverwrapThicknessSum_um));
            OnPropertyChanged(nameof(OverwrapArealWeight_mgcm2));
            OnPropertyChanged(nameof(OverwrapEffDensity_gcm3));
        }

        /// <summary>
        /// Recalculate totals from InsShellComposition
        /// </summary>
        private void RecalcInsShellFromComposition()
        {
            if (!AutoSyncFromComposition) return;

            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0;
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in InsShellComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3   += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2  += L.ArealWeight_mgcm2;
            }

            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            _insShellThicknessSum_um   = Math.Max(0, sumThickness_um);
            _insShellArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _insShellEffDensity_gcm3   = Math.Max(0, effDensity_gcm3);

            OnPropertyChanged(nameof(InsShellThicknessSum_um));
            OnPropertyChanged(nameof(InsShellArealWeight_mgcm2));
            OnPropertyChanged(nameof(InsShellEffDensity_gcm3));
        }

        /// <summary>
        /// Load sample Overwrap composition data
        /// </summary>
        private void LoadSampleOverwrapComposition()
        {
            OverwrapComposition.Clear();
            OverwrapComposition.Add(new PackagingLayer
            {
                No = 1,
                Name = "PET 25µm",
                Version = "1 – 01.11.25",
                Thickness_um = 25.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 1.38
            });
        }

        /// <summary>
        /// Load sample Insulation Shell composition data
        /// </summary>
        private void LoadSampleInsShellComposition()
        {
            InsShellComposition.Clear();
            InsShellComposition.Add(new PackagingLayer
            {
                No = 1,
                Name = "PET 50µm",
                Version = "1 – 01.11.25",
                Thickness_um = 50.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 1.38
            });
        }

        // Overwrap Material Properties
        private double _overwrapThicknessSum_um = 25.0;
        public double OverwrapThicknessSum_um
        {
            get => _overwrapThicknessSum_um;
            set
            {
                if (value < 0) value = 0;
                _overwrapThicknessSum_um = value;
                OnPropertyChanged();
            }
        }

        private double _overwrapArealWeight_mgcm2 = 0.345;
        public double OverwrapArealWeight_mgcm2
        {
            get => _overwrapArealWeight_mgcm2;
            set
            {
                if (value < 0) value = 0;
                _overwrapArealWeight_mgcm2 = value;
                OnPropertyChanged();
            }
        }

        private double _overwrapEffDensity_gcm3 = 1.38;
        public double OverwrapEffDensity_gcm3
        {
            get => _overwrapEffDensity_gcm3;
            set
            {
                if (value < 0) value = 0;
                _overwrapEffDensity_gcm3 = value;
                OnPropertyChanged();
            }
        }

        private double _overwrapCost_EURm2 = 0.15;
        public double OverwrapCost_EURm2
        {
            get => _overwrapCost_EURm2;
            set
            {
                if (value < 0) value = 0;
                _overwrapCost_EURm2 = value;
                OnPropertyChanged();
            }
        }

        private string _overwrapThicknessNote = "Spec sheet";
        public string OverwrapThicknessNote
        {
            get => _overwrapThicknessNote;
            set { _overwrapThicknessNote = value; OnPropertyChanged(); }
        }

        private string _overwrapArealWeightNote = "Calculated";
        public string OverwrapArealWeightNote
        {
            get => _overwrapArealWeightNote;
            set { _overwrapArealWeightNote = value; OnPropertyChanged(); }
        }

        private string _overwrapEffDensityNote = "Lit. PET";
        public string OverwrapEffDensityNote
        {
            get => _overwrapEffDensityNote;
            set { _overwrapEffDensityNote = value; OnPropertyChanged(); }
        }

        private string _overwrapCostNote = "Supplier quote";
        public string OverwrapCostNote
        {
            get => _overwrapCostNote;
            set { _overwrapCostNote = value; OnPropertyChanged(); }
        }

        // Insulation Shell Material Properties
        private double _insShellThicknessSum_um = 50.0;
        public double InsShellThicknessSum_um
        {
            get => _insShellThicknessSum_um;
            set
            {
                if (value < 0) value = 0;
                _insShellThicknessSum_um = value;
                OnPropertyChanged();
            }
        }

        private double _insShellArealWeight_mgcm2 = 0.69;
        public double InsShellArealWeight_mgcm2
        {
            get => _insShellArealWeight_mgcm2;
            set
            {
                if (value < 0) value = 0;
                _insShellArealWeight_mgcm2 = value;
                OnPropertyChanged();
            }
        }

        private double _insShellEffDensity_gcm3 = 1.38;
        public double InsShellEffDensity_gcm3
        {
            get => _insShellEffDensity_gcm3;
            set
            {
                if (value < 0) value = 0;
                _insShellEffDensity_gcm3 = value;
                OnPropertyChanged();
            }
        }

        private double _insShellCost_EURm2 = 0.25;
        public double InsShellCost_EURm2
        {
            get => _insShellCost_EURm2;
            set
            {
                if (value < 0) value = 0;
                _insShellCost_EURm2 = value;
                OnPropertyChanged();
            }
        }

        private string _insShellThicknessNote = "Spec sheet";
        public string InsShellThicknessNote
        {
            get => _insShellThicknessNote;
            set { _insShellThicknessNote = value; OnPropertyChanged(); }
        }

        private string _insShellArealWeightNote = "Calculated";
        public string InsShellArealWeightNote
        {
            get => _insShellArealWeightNote;
            set { _insShellArealWeightNote = value; OnPropertyChanged(); }
        }

        private string _insShellEffDensityNote = "Lit. PET";
        public string InsShellEffDensityNote
        {
            get => _insShellEffDensityNote;
            set { _insShellEffDensityNote = value; OnPropertyChanged(); }
        }

        private string _insShellCostNote = "Supplier quote";
        public string InsShellCostNote
        {
            get => _insShellCostNote;
            set { _insShellCostNote = value; OnPropertyChanged(); }
        }

        #endregion

        #region Fixing Tape

        /// <summary>
        /// Fixing Tape composition (layers loaded from DB)
        /// </summary>
        public ObservableCollection<PackagingLayer> FixingTapeComposition { get; } = new();

        /// <summary>
        /// Material presets for Fixing Tape dropdown
        /// </summary>
        public IReadOnlyList<string> FixingTapeMaterialPresets { get; } = new[] { "PET 50µm", "PET 75µm", "Woven glass tape", "Custom" };

        /// <summary>
        /// Version presets for Fixing Tape dropdown
        /// </summary>
        public IReadOnlyList<string> FixingTapeVersionPresets { get; } = new[] { "1 – 01.10.25", "2 – 15.11.25", "Custom" };

        /// <summary>
        /// Selected Fixing Tape Material
        /// </summary>
        private string _fixingTapeMaterial = "PET 50µm";
        public string FixingTapeMaterial
        {
            get => _fixingTapeMaterial;
            set { _fixingTapeMaterial = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Selected Fixing Tape Version
        /// </summary>
        private string _fixingTapeVersion = "1 – 01.10.25";
        public string FixingTapeVersion
        {
            get => _fixingTapeVersion;
            set { _fixingTapeVersion = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Recalculate totals from FixingTapeComposition
        /// </summary>
        private void RecalcFixingTapeFromComposition()
        {
            if (!AutoSyncFromComposition) return;

            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0;
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in FixingTapeComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3   += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2  += L.ArealWeight_mgcm2;
            }

            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            _fixingTapeThicknessSum_um   = Math.Max(0, sumThickness_um);
            _fixingTapeArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _fixingTapeEffDensity_gcm3   = Math.Max(0, effDensity_gcm3);

            OnPropertyChanged(nameof(FixingTapeThicknessSum_um));
            OnPropertyChanged(nameof(FixingTapeArealWeight_mgcm2));
            OnPropertyChanged(nameof(FixingTapeEffDensity_gcm3));
        }

        /// <summary>
        /// Load sample Fixing Tape composition data
        /// </summary>
        private void LoadSampleFixingTapeComposition()
        {
            FixingTapeComposition.Clear();
            FixingTapeComposition.Add(new PackagingLayer
            {
                No = 1,
                Name = "PET",
                Version = "1 – 01.10.25",
                Thickness_um = 50.0,
                Porosity_pct = 0.0,
                Density_gcm3 = 1.38
            });

            // Wire collection changed
            FixingTapeComposition.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null)
                    foreach (PackagingLayer layer in e.OldItems)
                        layer.PropertyChanged -= OnFixingTapeLayerPropertyChanged;

                if (e.NewItems != null)
                    foreach (PackagingLayer layer in e.NewItems)
                        layer.PropertyChanged += OnFixingTapeLayerPropertyChanged;

                RecalcFixingTapeFromComposition();
            };

            // Wire existing items
            foreach (var layer in FixingTapeComposition)
                layer.PropertyChanged += OnFixingTapeLayerPropertyChanged;

            RecalcFixingTapeFromComposition();
        }

        private void OnFixingTapeLayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecalcFixingTapeFromComposition();
        }

        // Fixing Tape Material Properties

        private double _fixingTapeThicknessSum_um = 50.0;
        public double FixingTapeThicknessSum_um
        {
            get => _fixingTapeThicknessSum_um;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeThicknessSum_um = value;
                OnPropertyChanged();
            }
        }

        private double _fixingTapeArealWeight_mgcm2 = 0.725;
        public double FixingTapeArealWeight_mgcm2
        {
            get => _fixingTapeArealWeight_mgcm2;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeArealWeight_mgcm2 = value;
                OnPropertyChanged();
            }
        }

        private double _fixingTapeEffDensity_gcm3 = 1.45;
        public double FixingTapeEffDensity_gcm3
        {
            get => _fixingTapeEffDensity_gcm3;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeEffDensity_gcm3 = value;
                OnPropertyChanged();
            }
        }

        private double _fixingTapeCost_EURm2 = 0.4;
        public double FixingTapeCost_EURm2
        {
            get => _fixingTapeCost_EURm2;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeCost_EURm2 = value;
                OnPropertyChanged();
            }
        }

        // Fixing Tape Material Properties Notes

        private string _fixingTapeThicknessNote = "CoA xyz";
        public string FixingTapeThicknessNote
        {
            get => _fixingTapeThicknessNote;
            set { _fixingTapeThicknessNote = value; OnPropertyChanged(); }
        }

        private string _fixingTapeArealWeightNote = "Measured @CL 10.10.25";
        public string FixingTapeArealWeightNote
        {
            get => _fixingTapeArealWeightNote;
            set { _fixingTapeArealWeightNote = value; OnPropertyChanged(); }
        }

        private string _fixingTapeEffDensityNote = "Lit. from xyz";
        public string FixingTapeEffDensityNote
        {
            get => _fixingTapeEffDensityNote;
            set { _fixingTapeEffDensityNote = value; OnPropertyChanged(); }
        }

        private string _fixingTapeCostNote = "REC Value from 05.10.25";
        public string FixingTapeCostNote
        {
            get => _fixingTapeCostNote;
            set { _fixingTapeCostNote = value; OnPropertyChanged(); }
        }

        // Fixing Tape Geometry

        private double _fixingTapeWidth_mm = 10.0;
        public double FixingTapeWidth_mm
        {
            get => _fixingTapeWidth_mm;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeWidth_mm = value;
                OnPropertyChanged();
            }
        }

        private double _fixingTapeTotalLength_cm = 40.0;
        public double FixingTapeTotalLength_cm
        {
            get => _fixingTapeTotalLength_cm;
            set
            {
                if (value < 0) value = 0;
                _fixingTapeTotalLength_cm = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Report - Core

        // TODO: replace all Report values with real DB-backed calculations once materials database is available.

        // Energy density properties
        public double GravEnergyDensity_Whkg => 380.0;  // TODO: calculate from cell mass and energy
        public double VolEnergyDensity_Cell_Whl => 970.5;  // TODO: calculate from cell volume and energy
        public double VolEnergyDensity_Stack_Whl => 930.2;  // TODO: calculate from stack volume and energy
        public double VolEnergyDensity_ECU_Whl => 1050.0;  // TODO: calculate from ECU volume and energy

        // Volume properties
        public double Volume_ECU_cm3 => 433.1;  // TODO: calculate from ECU dimensions
        public double Volume_Stack_cm3 => 468.4;  // TODO: calculate from stack dimensions
        public double Volume_CellBox_cm3 => 480.6;  // TODO: calculate from cell box dimensions (Height * Width * Thickness)

        // Construction efficiency properties
        public double Efficiency_Stack_pct => 86.65;  // TODO: calculate stack efficiency
        public double Efficiency_ECU_pct => 80.03;  // TODO: calculate ECU efficiency

        // Electrical characteristics properties
        public double CellCapacity_Ah => 9.348;  // TODO: calculate from cathode active material and chemistry
        public double CellEnergy_Wh => 34588.0;  // TODO: calculate from capacity * nominal voltage
        public double NominalVoltage_V => 3.70;  // TODO: get from chemistry database
        public double AverageOverpotential_mV => 60.2;  // TODO: calculate from chemistry/stack config
        public double ArealCapacity_mAhcm2 => 3.674;  // TODO: calculate from capacity / active area
        public double ArealEnergy_mWhcm2 => 13592.0;  // TODO: calculate from areal capacity * voltage
        public double Umin_V => 2.50;  // TODO: get from chemistry database
        public double Umax_V => 4.25;  // TODO: get from chemistry database

        // Number of sheets (placeholders - TODO: expose from StackConfigurationViewModel)
        public int CathodeSheets_Cell => 100;  // TODO: get from StackConfigurationViewModel
        public int AnodeSheets_Cell => 102;  // TODO: get from StackConfigurationViewModel
        public int AllElectrodeSheets_Cell => 202;  // TODO: get from StackConfigurationViewModel
        public int SeparatorSheets_Cell => 204;  // TODO: get from StackConfigurationViewModel

        // Cell dimensions properties (using existing geometry where possible)
        public double CellHeight_mm => CalculatedHeight_mm;  // From Packaging → Case
        public double CellWidth_mm => CalculatedWidth_mm;  // From Packaging → Case
        public double CellThickness_Dry_mm => 13.54;  // TODO: calculate from stack dry thickness + case
        public double CellThickness_SoC0_mm => 14.77;  // TODO: calculate with 0% SoC swelling
        public double CellThickness_SoC100_mm => 15.16;  // TODO: calculate with 100% SoC swelling
        public double FormationSwelling_pct => 9.11;  // TODO: get from chemistry/materials
        public double SoCBreathing_pct => 2.61;  // TODO: calculate from SoC0 to SoC100 thickness change

        // Areal characteristics (calculated from existing Sheet Design areas)
        // TODO: multiply by actual sheet counts from StackConfigurationViewModel when exposed
        public double CathodeActiveArea_m2 => CathodeArea_cm2 * 100 / 10000.0;  // Assumes 100 cathode sheets
        public double AnodeActiveArea_m2 => AnodeArea_cm2 * 102 / 10000.0;  // Assumes 102 anode sheets
        public double SeparatorArea_m2 => SeparatorArea_cm2 * 204 / 10000.0;  // Assumes 204 separator sheets

        // Mass summary properties
        public double CellMass_g => 1250.1;  // TODO: sum of all component masses
        public double CathodeMass_g => 620.5;  // TODO: calculate from cathode material density * volume
        public double AnodeMass_g => 330.2;  // TODO: calculate from anode material density * volume
        public double ElectrolyteMass_g => 180.2;  // TODO: calculate from electrolyte volume/porosity
        public double SeparatorMass_g => 40.6;  // TODO: calculate from separator material * area
        public double HousingMass_g => 20.1;  // TODO: calculate from packaging materials

        // Material name properties (placeholders)
        public string CellNameReport => "PRIMA FCL";  // From cell name
        public string CathodeMaterialName => "N00B Gen. 1 Cathode";  // TODO: get from materials DB
        public string AnodeMaterialName => "N00B Gen. 2 Anode";  // TODO: get from materials DB
        public string ElectrolyteName => "CL – E007";  // TODO: get from materials DB
        public string SeparatorName => "Cellgard xyz";  // TODO: get from materials DB
        public string HousingName => "PRIMA FCL";  // From packaging case material name

        #endregion

        #region Report - Comp

        // Material name properties for Comp tab headers
        public string ReportCathodeName => "N00B Gen. 1 Cathode";  // TODO: get from materials DB
        public string ReportAnodeName => "N00B Gen. 2 Anode";  // TODO: get from materials DB
        public string ReportElectrolyteName => "CL – E007";  // TODO: get from materials DB
        public string ReportSeparatorName => "Cellgard xyz";  // TODO: get from materials DB

        // Cathode characteristics
        public double ReportCathodeLoading_mgcm2 => 18.02;  // TODO: get from materials DB
        public double ReportCathodeRevSpecCapacity_mAhg => 210.54;  // TODO: get from materials DB
        public double ReportCathodeC3SpecCapacity_mAhg => 215.11;  // TODO: get from materials DB
        public double ReportCathodeCoatingThk_0pct_um => 59.11;  // TODO: calculate from loading/density
        public double ReportCathodeCoatingThk_100pct_um => 58.54;  // TODO: calculate from loading/density
        public double ReportCathodeCoatingDensity_0pct_gcm3 => 3.451;  // TODO: get from materials DB
        public double ReportCathodeCoatingDensity_100pct_gcm3 => 3.472;  // TODO: get from materials DB

        // Anode characteristics
        public double ReportAnodeLoading_mgcm2 => 9.45;  // TODO: get from materials DB
        public double ReportAnodeRevSpecCapacity_mAhg => 372.15;  // TODO: get from materials DB
        public double ReportAnodeC3SpecCapacity_mAhg => 365.02;  // TODO: get from materials DB
        public double ReportAnodeCoatingThk_0pct_um => 62.78;  // TODO: calculate from loading/density
        public double ReportAnodeCoatingThk_100pct_um => 68.91;  // TODO: calculate from loading/density
        public double ReportAnodeCoatingDensity_0pct_gcm3 => 1.505;  // TODO: get from materials DB
        public double ReportAnodeCoatingDensity_100pct_gcm3 => 1.371;  // TODO: get from materials DB

        // Separator characteristics
        public double ReportSeparatorThickness_um => 12.0;  // TODO: get from materials DB
        public double ReportSeparatorPorosity_pct => 39.0;  // TODO: get from materials DB
        public double ReportSeparatorArealWeight_mgcm2 => 0.73;  // TODO: get from materials DB

        // Electrolyte characteristics
        public double ReportElectrolytePoresAnode_0pctSoC_ml => 142.7;  // TODO: calculate from anode porosity
        public double ReportElectrolytePoresCathode_0pctSoC_ml => 86.3;  // TODO: calculate from cathode porosity
        public double ReportElectrolytePoresSeparator_ml => 195.4;  // TODO: calculate from separator porosity
        public double ReportElectrolyteCalcVolume_ml => 424.4;  // TODO: sum of pore volumes
        public double ReportElectrolyteUsedVolume_ml => 466.8;  // TODO: calculated volume * excess factor
        public double ReportElectrolyteExcessFactor => 1.10;  // TODO: get from configuration/DB
        public double ReportElectrolyteCalcVolumeCapacity_mlAh => 10.61;  // TODO: calc volume / cell capacity
        public double ReportElectrolyteUsedVolumeCapacity_mlAh => 11.67;  // TODO: used volume / cell capacity

        // Heat capacity characteristics
        public double ReportHeatCapacityCell_JgK => 0.951;  // TODO: calculate weighted average
        public double ReportHeatCapacityCathode_JgK => 0.832;  // TODO: get from materials DB
        public double ReportHeatCapacityAnode_JgK => 1.115;  // TODO: get from materials DB
        public double ReportHeatCapacityElectrolyte_JgK => 1.850;  // TODO: get from materials DB
        public double ReportHeatCapacitySeparator_JgK => 1.200;  // TODO: get from materials DB
        public double ReportHeatCapacityCase_JgK => 0.903;  // TODO: get from materials DB

        // Thermal conductivity characteristics
        public double ReportThermalCondPosCollectorShareZ_pct => 15.8;  // TODO: calculate from geometry
        public double ReportThermalCondNegCollectorShareZ_pct => 18.2;  // TODO: calculate from geometry
        public double ReportThermalCondStackXY_WmK => 0.412;  // TODO: calculate from materials

        #endregion
    }
}
