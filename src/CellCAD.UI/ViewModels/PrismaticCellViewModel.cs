using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CellCAD.viewmodels
{
    /// <summary>
    /// Enumerations for prismatic stack configuration
    /// </summary>
    public enum PrismaticEndElectrodesMode
    {
        BothNegative,
        BothPositive,
        PositiveNegative
    }

    public enum PrismaticEndCoatingsMode
    {
        BothDouble,
        OneSingle,
        BothSingle
    }

    /// <summary>
    /// ViewModel for Prismatic cell type design.
    /// Geometry-focused implementation (no chemistry/materials/compression logic here).
    /// </summary>
    public partial class PrismaticCellViewModel : ObservableObject
    {
        #region Backing Fields

        private double _cathodeHeight_mm = 100.0;
        private double _cathodeWidth_mm = 150.0;
        private double _anodeOffsetTop_mm = 1.0;
        private double _anodeOffsetBottom_mm = 1.0;
        private double _anodeOffsetLeft_mm = 1.0;
        private double _anodeOffsetRight_mm = 1.0;
        private double _separatorOffsetTop_mm = 0.5;
        private double _separatorOffsetBottom_mm = 0.5;
        private double _separatorOffsetLeft_mm = 0.5;
        private double _separatorOffsetRight_mm = 0.5;
        private bool _updateStackDimensions = true;

        #endregion

        #region Constructor

        public PrismaticCellViewModel()
        {
            // Initialize with neutral preset values
            // Cathode: 100mm x 150mm
            // Anode offsets: 1mm on all sides
            // Separator offsets: 0.5mm on all sides

            // Initialize packaging components table with 10 rows (6 pre-filled, 4 empty)
            PackagingComponents = new ObservableCollection<PackagingComponent>
            {
                new PackagingComponent { No = 1, Name = "Housing Case", Material = "Aluminium", Version = "01.10.2035", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 2, Name = "Positive Lid", Material = "Aluminium", Version = "01.10.2036", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 3, Name = "Negative Lid", Material = "Aluminium", Version = "01.10.2037", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 4, Name = "Positive Terminal", Material = "Aluminium", Version = "01.10.2038", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 5, Name = "Negative Terminal", Material = "Copper", Version = "01.10.2039", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 6, Name = "Insulation Parts", Material = "PP", Version = "01.10.2040", Mass_g = 112.05, Volume_cm3 = 12.345, Density_gcm3 = 13.13, Cost_EUR = 6.66 },
                new PackagingComponent { No = 7 },
                new PackagingComponent { No = 8 },
                new PackagingComponent { No = 9 },
                new PackagingComponent { No = 10 }
            };

            // Hook up auto-recalculation for Overwrap and InsShell compositions
            OverwrapComposition.CollectionChanged += (_, __) => RecalcOverwrapFromComposition();
            InsShellComposition.CollectionChanged += (_, __) => RecalcInsShellFromComposition();

            // Populate with sample data (will be replaced by DB load later)
            LoadSampleOverwrapComposition();
            LoadSampleInsShellComposition();
        }

        #endregion

        #region Sheet Design - Cathode

        /// <summary>
        /// Cathode height in mm
        /// </summary>
        public double CathodeHeight_mm
        {
            get => _cathodeHeight_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _cathodeHeight_mm, value))
                {
                    OnPropertyChanged(nameof(CathodeArea_cm2));
                    OnPropertyChanged(nameof(AnodeHeight_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorHeight_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Cathode width in mm
        /// </summary>
        public double CathodeWidth_mm
        {
            get => _cathodeWidth_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _cathodeWidth_mm, value))
                {
                    OnPropertyChanged(nameof(CathodeArea_cm2));
                    OnPropertyChanged(nameof(AnodeWidth_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorWidth_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Cathode area in cm² (calculated)
        /// </summary>
        public double CathodeArea_cm2 => (CathodeWidth_mm * CathodeHeight_mm) / 100.0;

        #endregion

        #region Sheet Design - Anode Offsets

        /// <summary>
        /// Anode offset from cathode top edge in mm
        /// </summary>
        public double AnodeOffsetTop_mm
        {
            get => _anodeOffsetTop_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _anodeOffsetTop_mm, value))
                {
                    OnPropertyChanged(nameof(AnodeHeight_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorHeight_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Anode offset from cathode bottom edge in mm
        /// </summary>
        public double AnodeOffsetBottom_mm
        {
            get => _anodeOffsetBottom_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _anodeOffsetBottom_mm, value))
                {
                    OnPropertyChanged(nameof(AnodeHeight_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorHeight_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Anode offset from cathode left edge in mm
        /// </summary>
        public double AnodeOffsetLeft_mm
        {
            get => _anodeOffsetLeft_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _anodeOffsetLeft_mm, value))
                {
                    OnPropertyChanged(nameof(AnodeWidth_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorWidth_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Anode offset from cathode right edge in mm
        /// </summary>
        public double AnodeOffsetRight_mm
        {
            get => _anodeOffsetRight_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _anodeOffsetRight_mm, value))
                {
                    OnPropertyChanged(nameof(AnodeWidth_mm));
                    OnPropertyChanged(nameof(AnodeArea_cm2));
                    OnPropertyChanged(nameof(SeparatorWidth_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        #endregion

        #region Sheet Design - Anode (Calculated)

        /// <summary>
        /// Anode height in mm (calculated from cathode + offsets)
        /// </summary>
        public double AnodeHeight_mm => CathodeHeight_mm + AnodeOffsetTop_mm + AnodeOffsetBottom_mm;

        /// <summary>
        /// Anode width in mm (calculated from cathode + offsets)
        /// </summary>
        public double AnodeWidth_mm => CathodeWidth_mm + AnodeOffsetLeft_mm + AnodeOffsetRight_mm;

        /// <summary>
        /// Anode area in cm² (calculated)
        /// </summary>
        public double AnodeArea_cm2 => (AnodeWidth_mm * AnodeHeight_mm) / 100.0;

        #endregion

        #region Sheet Design - Separator Offsets

        /// <summary>
        /// Separator offset from anode top edge in mm
        /// </summary>
        public double SeparatorOffsetTop_mm
        {
            get => _separatorOffsetTop_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _separatorOffsetTop_mm, value))
                {
                    OnPropertyChanged(nameof(SeparatorHeight_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Separator offset from anode bottom edge in mm
        /// </summary>
        public double SeparatorOffsetBottom_mm
        {
            get => _separatorOffsetBottom_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _separatorOffsetBottom_mm, value))
                {
                    OnPropertyChanged(nameof(SeparatorHeight_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Separator offset from anode left edge in mm
        /// </summary>
        public double SeparatorOffsetLeft_mm
        {
            get => _separatorOffsetLeft_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _separatorOffsetLeft_mm, value))
                {
                    OnPropertyChanged(nameof(SeparatorWidth_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        /// <summary>
        /// Separator offset from anode right edge in mm
        /// </summary>
        public double SeparatorOffsetRight_mm
        {
            get => _separatorOffsetRight_mm;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _separatorOffsetRight_mm, value))
                {
                    OnPropertyChanged(nameof(SeparatorWidth_mm));
                    OnPropertyChanged(nameof(SeparatorArea_cm2));
                }
            }
        }

        #endregion

        #region Sheet Design - Separator (Calculated)

        /// <summary>
        /// Separator height in mm (calculated from anode + offsets)
        /// </summary>
        public double SeparatorHeight_mm => AnodeHeight_mm + SeparatorOffsetTop_mm + SeparatorOffsetBottom_mm;

        /// <summary>
        /// Separator width in mm (calculated from anode + offsets)
        /// </summary>
        public double SeparatorWidth_mm => AnodeWidth_mm + SeparatorOffsetLeft_mm + SeparatorOffsetRight_mm;

        /// <summary>
        /// Separator area in cm² (calculated)
        /// </summary>
        public double SeparatorArea_cm2 => (SeparatorWidth_mm * SeparatorHeight_mm) / 100.0;

        #endregion

        #region Stack Configuration

        // Stack configuration backing fields
        private int _numberOfStacksInCell = 2;
        private int _electrodePairsPerStack = 50;
        private bool _calculateByMaxCellThickness = false;
        private double _maxCellThickness_SoC100_mm = 15.2;
        private PrismaticEndElectrodesMode _endElectrodes = PrismaticEndElectrodesMode.BothNegative;
        private PrismaticEndCoatingsMode _endCoatings = PrismaticEndCoatingsMode.BothDouble;
        private int _separatorOverwrapsPerStack = 1;
        private int _additionalOverwrapsPerStack = 0;
        private int _insulationShellAroundAllStacks = 1;
        private int _fixingTapesPerStack = 1;
        private bool _fixingTapesOnAllStacks = true;
        private double _targetSeparatorCompression_pct = 3.0;

        /// <summary>
        /// Number of stacks in the cell
        /// </summary>
        public int NumberOfStacksInCell
        {
            get => _numberOfStacksInCell;
            set
            {
                if (SetProperty(ref _numberOfStacksInCell, value))
                {
                    OnPropertyChanged(nameof(ElectrodePairsInCell));
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Number of electrode pairs per stack
        /// </summary>
        public int ElectrodePairsPerStack
        {
            get => _electrodePairsPerStack;
            set
            {
                if (SetProperty(ref _electrodePairsPerStack, value))
                {
                    OnPropertyChanged(nameof(ElectrodePairsInCell));
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Total electrode pairs in cell (computed)
        /// </summary>
        public int ElectrodePairsInCell => ElectrodePairsPerStack * NumberOfStacksInCell;

        /// <summary>
        /// Calculate electrode pairs by maximum cell thickness
        /// </summary>
        public bool CalculateByMaxCellThickness
        {
            get => _calculateByMaxCellThickness;
            set => SetProperty(ref _calculateByMaxCellThickness, value);
        }

        /// <summary>
        /// Maximum cell thickness at 100% SoC in mm
        /// </summary>
        public double MaxCellThickness_SoC100_mm
        {
            get => _maxCellThickness_SoC100_mm;
            set => SetProperty(ref _maxCellThickness_SoC100_mm, value);
        }

        /// <summary>
        /// End electrodes configuration
        /// </summary>
        public PrismaticEndElectrodesMode EndElectrodes
        {
            get => _endElectrodes;
            set
            {
                if (SetProperty(ref _endElectrodes, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// End coatings configuration
        /// </summary>
        public PrismaticEndCoatingsMode EndCoatings
        {
            get => _endCoatings;
            set => SetProperty(ref _endCoatings, value);
        }

        /// <summary>
        /// Number of separator overwraps per stack
        /// </summary>
        public int SeparatorOverwrapsPerStack
        {
            get => _separatorOverwrapsPerStack;
            set
            {
                if (SetProperty(ref _separatorOverwrapsPerStack, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Number of additional overwraps per stack
        /// </summary>
        public int AdditionalOverwrapsPerStack
        {
            get => _additionalOverwrapsPerStack;
            set
            {
                if (SetProperty(ref _additionalOverwrapsPerStack, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Number of insulation shells around all stacks
        /// </summary>
        public int InsulationShellAroundAllStacks
        {
            get => _insulationShellAroundAllStacks;
            set
            {
                if (SetProperty(ref _insulationShellAroundAllStacks, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Number of fixing tapes per stack
        /// </summary>
        public int FixingTapesPerStack
        {
            get => _fixingTapesPerStack;
            set
            {
                if (SetProperty(ref _fixingTapesPerStack, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        /// <summary>
        /// Whether fixing tapes are on all stacks
        /// </summary>
        public bool FixingTapesOnAllStacks
        {
            get => _fixingTapesOnAllStacks;
            set
            {
                if (SetProperty(ref _fixingTapesOnAllStacks, value))
                {
                    NotifyAllStackComputedProperties();
                }
            }
        }

        // Calculated Thickness properties (placeholders from screenshot)
        public double SingleStackThickness_Dry_mm => 6.1;
        public double SingleStackThickness_SoC0_mm => 6.65;
        public double SingleStackThickness_SoC100_mm => 6.85;
        public double AllStacksThickness_Dry_mm => 13.25;
        public double AllStacksThickness_SoC0_mm => 14.45;
        public double AllStacksThickness_SoC100_mm => 14.85;
        public double CellOuterThickness_mm => 13.55;
        public double CellInnerThickness_mm => 14.75;
        public double GapToInnerWall_Dry_mm => 1;
        public double GapToInnerWall_SoC0_mm => 2;
        public double GapToInnerWall_SoC100_mm => -1;
        public double SeparatorCompression_Dry_pct => -60.2;
        public double SeparatorCompression_SoC0_pct => 2.5;
        public double SeparatorCompression_SoC100_pct => 10.0;

        public double TargetSeparatorCompression_pct
        {
            get => _targetSeparatorCompression_pct;
            set => SetProperty(ref _targetSeparatorCompression_pct, value);
        }

        // Number of Sheet Layers (in z) - computed properties
        public int AllElectrodeSheets_Stack => ElectrodePairsPerStack * 2 + (EndElectrodes == PrismaticEndElectrodesMode.BothNegative || EndElectrodes == PrismaticEndElectrodesMode.BothPositive ? 0 : 0);
        public int AllElectrodeSheets_Cell => AllElectrodeSheets_Stack * NumberOfStacksInCell;

        public int CathodeSheets_Stack => ElectrodePairsPerStack;
        public int CathodeSheets_Cell => CathodeSheets_Stack * NumberOfStacksInCell;

        public int AnodeSheets_Stack => ElectrodePairsPerStack + (EndElectrodes == PrismaticEndElectrodesMode.BothNegative ? 1 : 0);
        public int AnodeSheets_Cell => AnodeSheets_Stack * NumberOfStacksInCell;

        public int SeparatorSheets_Stack => ElectrodePairsPerStack * 2;
        public int SeparatorSheets_Cell => SeparatorSheets_Stack * NumberOfStacksInCell;

        public int Overwrap_Stack => SeparatorOverwrapsPerStack + AdditionalOverwrapsPerStack;
        public int Overwrap_Cell => Overwrap_Stack * NumberOfStacksInCell;

        public int InsulationShell_Stack => 0;
        public int InsulationShell_Cell => InsulationShellAroundAllStacks;

        public int FixingTape_Stack => FixingTapesPerStack;
        public int FixingTape_Cell => FixingTapesOnAllStacks ? FixingTapesPerStack * NumberOfStacksInCell : FixingTapesPerStack;

        // Commands (stubs)
        public IRelayCommand RecalculateLoadingsCommand => new RelayCommand(() => { /* TODO: Implement */ });
        public IRelayCommand MatchCompressionCommand => new RelayCommand(() => { /* TODO: Implement */ });

        private void NotifyAllStackComputedProperties()
        {
            OnPropertyChanged(nameof(AllElectrodeSheets_Stack));
            OnPropertyChanged(nameof(AllElectrodeSheets_Cell));
            OnPropertyChanged(nameof(CathodeSheets_Stack));
            OnPropertyChanged(nameof(CathodeSheets_Cell));
            OnPropertyChanged(nameof(AnodeSheets_Stack));
            OnPropertyChanged(nameof(AnodeSheets_Cell));
            OnPropertyChanged(nameof(SeparatorSheets_Stack));
            OnPropertyChanged(nameof(SeparatorSheets_Cell));
            OnPropertyChanged(nameof(Overwrap_Stack));
            OnPropertyChanged(nameof(Overwrap_Cell));
            OnPropertyChanged(nameof(InsulationShell_Cell));
            OnPropertyChanged(nameof(FixingTape_Stack));
            OnPropertyChanged(nameof(FixingTape_Cell));
        }

        #endregion

        #region Sheet Design - Compatibility Properties for SheetDesignView

        // SheetDesignView uses symmetric offset naming from Pouch (AnodeOffsetY_mm, AnodeOffsetX_mm)
        // For Prismatic, we provide these as compatibility properties that update all directional offsets

        /// <summary>
        /// Anode Y offset (compatibility property - sets both Top and Bottom)
        /// </summary>
        public double AnodeOffsetY_mm
        {
            get => (_anodeOffsetTop_mm + _anodeOffsetBottom_mm) / 2.0;
            set
            {
                if (value < 0) value = 0;
                _anodeOffsetTop_mm = value;
                _anodeOffsetBottom_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeOffsetTop_mm));
                OnPropertyChanged(nameof(AnodeOffsetBottom_mm));
                OnPropertyChanged(nameof(AnodeHeight_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorHeight_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
            }
        }

        /// <summary>
        /// Anode X offset (compatibility property - sets both Left and Right)
        /// </summary>
        public double AnodeOffsetX_mm
        {
            get => (_anodeOffsetLeft_mm + _anodeOffsetRight_mm) / 2.0;
            set
            {
                if (value < 0) value = 0;
                _anodeOffsetLeft_mm = value;
                _anodeOffsetRight_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnodeOffsetLeft_mm));
                OnPropertyChanged(nameof(AnodeOffsetRight_mm));
                OnPropertyChanged(nameof(AnodeWidth_mm));
                OnPropertyChanged(nameof(AnodeArea_cm2));
                OnPropertyChanged(nameof(SeparatorWidth_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
            }
        }

        /// <summary>
        /// Separator Y offset (compatibility property - sets both Top and Bottom)
        /// </summary>
        public double SeparatorOffsetY_mm
        {
            get => (_separatorOffsetTop_mm + _separatorOffsetBottom_mm) / 2.0;
            set
            {
                if (value < 0) value = 0;
                _separatorOffsetTop_mm = value;
                _separatorOffsetBottom_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SeparatorOffsetTop_mm));
                OnPropertyChanged(nameof(SeparatorOffsetBottom_mm));
                OnPropertyChanged(nameof(SeparatorHeight_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
            }
        }

        /// <summary>
        /// Separator X offset (compatibility property - sets both Left and Right)
        /// </summary>
        public double SeparatorOffsetX_mm
        {
            get => (_separatorOffsetLeft_mm + _separatorOffsetRight_mm) / 2.0;
            set
            {
                if (value < 0) value = 0;
                _separatorOffsetLeft_mm = value;
                _separatorOffsetRight_mm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SeparatorOffsetLeft_mm));
                OnPropertyChanged(nameof(SeparatorOffsetRight_mm));
                OnPropertyChanged(nameof(SeparatorWidth_mm));
                OnPropertyChanged(nameof(SeparatorArea_cm2));
            }
        }

        #endregion

        #region Sheet Design - Flag Properties (Stubs for SheetDesignView compatibility)

        // SheetDesignView expects flag properties - provide stubs for now
        // TODO: Implement prismatic-specific tab/terminal design later

        private bool _flagsOnOppositeSides = false;
        private bool _flagsOnSameSide = false;

        public bool FlagsOnOppositeSides
        {
            get => _flagsOnOppositeSides;
            set
            {
                if (SetProperty(ref _flagsOnOppositeSides, value) && value)
                {
                    _flagsOnSameSide = false;
                    OnPropertyChanged(nameof(FlagsOnSameSide));
                }
            }
        }

        public bool FlagsOnSameSide
        {
            get => _flagsOnSameSide;
            set
            {
                if (SetProperty(ref _flagsOnSameSide, value) && value)
                {
                    _flagsOnOppositeSides = false;
                    OnPropertyChanged(nameof(FlagsOnOppositeSides));
                }
            }
        }

        // Flag dimension stubs (not used for prismatic, but needed for SheetDesignView binding)
        public double CathodeFlagHeight_mm { get; set; } = 0;
        public double CathodeFlagWidth_mm { get; set; } = 0;
        public double CathodeFlagOffsetX_mm { get; set; } = 0;
        public double AnodeFlagHeight_mm { get; set; } = 0;
        public double AnodeFlagWidth_mm { get; set; } = 0;
        public double AnodeFlagOffsetX_mm { get; set; } = 0;

        #endregion

        #region Sheet Design - Options

        /// <summary>
        /// Whether to automatically update stack dimensions when sheet geometry changes
        /// </summary>
        public bool UpdateStackDimensions
        {
            get => _updateStackDimensions;
            set => SetProperty(ref _updateStackDimensions, value);
        }

        #endregion

        #region Packaging - Case

        // Backing fields for Cell Dimensions
        private double _cellHeightY_mm = 101.2;
        private double _cellWidthX_mm = 177.4;
        private double _cellThicknessZ_mm = 45.41;
        private double _cellThicknessInclCoating_mm = 45.61;

        // Backing fields for Wall Thickness
        private double _wallSideXY_mm = 0.7;
        private double _wallSideZY_mm = 0.7;
        private double _wallTopXZ_mm = 1.5;
        private double _wallBottomXZ_mm = 1.0;

        // Backing fields for Separator Dimension (Packaging - Case specific, independent from Sheet Design calculated values)
        private double _packagingSeparatorHeight_mm = 92.9;
        private double _packagingSeparatorWidth_mm = 176.7;

        // Backing fields for Internal offset to cell dimension (separator)
        private double _separatorOffsetHeight_mm = 5.80;
        private double _separatorOffsetWidth_mm = -0.70;
        private double _separatorOffsetThickness0_mm = -0.10;
        private double _separatorCompression_pct = 3.00;

        // Backing fields for Stack position in cell
        private double _stackOffsetBottom_mm = 1.20;
        private double _stackOffsetLeft_mm = 8.50;

        // Backing fields for Insulation Coating
        private string? _insulationFoilName = "Polyimide";
        private string? _insulationFoilVersion = "2";
        private double _insulationFoilThickness_um = 100.0;

        /// <summary>
        /// Cell Height (Y dimension) in mm
        /// </summary>
        public double CellHeightY_mm
        {
            get => _cellHeightY_mm;
            set
            {
                if (SetProperty(ref _cellHeightY_mm, value))
                {
                    OnPropertyChanged(nameof(CellVolume_cm3));
                }
            }
        }

        /// <summary>
        /// Cell Width (X dimension) in mm
        /// </summary>
        public double CellWidthX_mm
        {
            get => _cellWidthX_mm;
            set
            {
                if (SetProperty(ref _cellWidthX_mm, value))
                {
                    OnPropertyChanged(nameof(CellVolume_cm3));
                }
            }
        }

        /// <summary>
        /// Cell Thickness (Z dimension, excluding coating) in mm
        /// </summary>
        public double CellThicknessZ_mm
        {
            get => _cellThicknessZ_mm;
            set => SetProperty(ref _cellThicknessZ_mm, value);
        }

        /// <summary>
        /// Cell Thickness including coating in mm
        /// </summary>
        public double CellThicknessInclCoating_mm
        {
            get => _cellThicknessInclCoating_mm;
            set
            {
                if (SetProperty(ref _cellThicknessInclCoating_mm, value))
                {
                    OnPropertyChanged(nameof(CellVolume_cm3));
                }
            }
        }

        /// <summary>
        /// Calculated cell volume in cm³
        /// </summary>
        public double CellVolume_cm3 =>
            CellHeightY_mm * CellWidthX_mm * CellThicknessInclCoating_mm / 1000.0;

        /// <summary>
        /// Side wall thickness (X/Y plane) in mm
        /// </summary>
        public double WallSideXY_mm
        {
            get => _wallSideXY_mm;
            set => SetProperty(ref _wallSideXY_mm, value);
        }

        /// <summary>
        /// Side wall thickness (Z/Y plane) in mm
        /// </summary>
        public double WallSideZY_mm
        {
            get => _wallSideZY_mm;
            set => SetProperty(ref _wallSideZY_mm, value);
        }

        /// <summary>
        /// Top lid thickness (X/Z plane) in mm
        /// </summary>
        public double WallTopXZ_mm
        {
            get => _wallTopXZ_mm;
            set => SetProperty(ref _wallTopXZ_mm, value);
        }

        /// <summary>
        /// Bottom lid thickness (X/Z plane) in mm
        /// </summary>
        public double WallBottomXZ_mm
        {
            get => _wallBottomXZ_mm;
            set => SetProperty(ref _wallBottomXZ_mm, value);
        }

        /// <summary>
        /// Separator height in mm (Packaging - Case tab, independent editable property)
        /// Note: Sheet Design has a calculated SeparatorHeight_mm property; this is separate for packaging
        /// </summary>
        public double PackagingSeparatorHeight_mm
        {
            get => _packagingSeparatorHeight_mm;
            set => SetProperty(ref _packagingSeparatorHeight_mm, value);
        }

        /// <summary>
        /// Separator width in mm (Packaging - Case tab, independent editable property)
        /// Note: Sheet Design has a calculated SeparatorWidth_mm property; this is separate for packaging
        /// </summary>
        public double PackagingSeparatorWidth_mm
        {
            get => _packagingSeparatorWidth_mm;
            set => SetProperty(ref _packagingSeparatorWidth_mm, value);
        }

        /// <summary>
        /// Separator offset height in mm
        /// </summary>
        public double SeparatorOffsetHeight_mm
        {
            get => _separatorOffsetHeight_mm;
            set => SetProperty(ref _separatorOffsetHeight_mm, value);
        }

        /// <summary>
        /// Separator offset width in mm
        /// </summary>
        public double SeparatorOffsetWidth_mm
        {
            get => _separatorOffsetWidth_mm;
            set => SetProperty(ref _separatorOffsetWidth_mm, value);
        }

        /// <summary>
        /// Separator offset thickness at 0% SoC in mm
        /// </summary>
        public double SeparatorOffsetThickness0_mm
        {
            get => _separatorOffsetThickness0_mm;
            set => SetProperty(ref _separatorOffsetThickness0_mm, value);
        }

        /// <summary>
        /// Separator compression percentage
        /// </summary>
        public double SeparatorCompression_pct
        {
            get => _separatorCompression_pct;
            set => SetProperty(ref _separatorCompression_pct, value);
        }

        /// <summary>
        /// Stack offset from bottom of cell in mm
        /// </summary>
        public double StackOffsetBottom_mm
        {
            get => _stackOffsetBottom_mm;
            set => SetProperty(ref _stackOffsetBottom_mm, value);
        }

        /// <summary>
        /// Stack offset from left start of cell in mm
        /// </summary>
        public double StackOffsetLeft_mm
        {
            get => _stackOffsetLeft_mm;
            set => SetProperty(ref _stackOffsetLeft_mm, value);
        }

        /// <summary>
        /// Preset list for insulation foil selection
        /// </summary>
        public IReadOnlyList<string> InsulationFoilPresets { get; } = new List<string>
        {
            "Polyimide",
            "PET 50µm",
            "Custom"
        };

        /// <summary>
        /// Preset list for insulation foil version
        /// </summary>
        public IReadOnlyList<string> InsulationFoilVersionPresets { get; } = new List<string>
        {
            "1",
            "2",
            "3"
        };

        /// <summary>
        /// Insulation foil name
        /// </summary>
        public string? InsulationFoilName
        {
            get => _insulationFoilName;
            set => SetProperty(ref _insulationFoilName, value);
        }

        /// <summary>
        /// Insulation foil version
        /// </summary>
        public string? InsulationFoilVersion
        {
            get => _insulationFoilVersion;
            set => SetProperty(ref _insulationFoilVersion, value);
        }

        /// <summary>
        /// Insulation foil thickness in micrometers
        /// </summary>
        public double InsulationFoilThickness_um
        {
            get => _insulationFoilThickness_um;
            set => SetProperty(ref _insulationFoilThickness_um, value);
        }

        /// <summary>
        /// Command to update cell dimensions
        /// </summary>
        public IRelayCommand UpdateCellDimensionsCommand => new RelayCommand(() =>
        {
            // TODO: Implement cell dimension update logic
            // Updates cathode sheet by keeping internal offsets
        });

        /// <summary>
        /// Command to update wall thickness
        /// </summary>
        public IRelayCommand UpdateWallThicknessCommand => new RelayCommand(() =>
        {
            // TODO: Implement wall thickness update logic
            // Updates cathode sheet by keeping internal offsets
        });

        #endregion

        #region Packaging - Table

        /// <summary>
        /// Collection of packaging components for the Table tab
        /// </summary>
        public ObservableCollection<PackagingComponent> PackagingComponents { get; }

        #endregion

        #region Packaging - Add. Foils

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
            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0;
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in OverwrapComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3 += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2 += L.ArealWeight_mgcm2;
            }

            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            _overwrapThicknessSum_um = Math.Max(0, sumThickness_um);
            _overwrapArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _overwrapEffDensity_gcm3 = Math.Max(0, effDensity_gcm3);

            OnPropertyChanged(nameof(OverwrapThicknessSum_um));
            OnPropertyChanged(nameof(OverwrapArealWeight_mgcm2));
            OnPropertyChanged(nameof(OverwrapEffDensity_gcm3));
        }

        /// <summary>
        /// Recalculate totals from InsShellComposition
        /// </summary>
        private void RecalcInsShellFromComposition()
        {
            double sumThickness_um = 0.0;
            double sumTW_um_gcm3 = 0.0;
            double sumAreal_mgcm2 = 0.0;

            foreach (var L in InsShellComposition)
            {
                sumThickness_um += L.Thickness_um;
                sumTW_um_gcm3 += L.Thickness_um * L.EffectiveDensity_gcm3;
                sumAreal_mgcm2 += L.ArealWeight_mgcm2;
            }

            double effDensity_gcm3 = (sumThickness_um > 0) ? (sumTW_um_gcm3 / sumThickness_um) : 0.0;

            _insShellThicknessSum_um = Math.Max(0, sumThickness_um);
            _insShellArealWeight_mgcm2 = Math.Max(0, sumAreal_mgcm2);
            _insShellEffDensity_gcm3 = Math.Max(0, effDensity_gcm3);

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
            set => SetProperty(ref _overwrapThicknessSum_um, value < 0 ? 0 : value);
        }

        private double _overwrapArealWeight_mgcm2 = 0.345;
        public double OverwrapArealWeight_mgcm2
        {
            get => _overwrapArealWeight_mgcm2;
            set => SetProperty(ref _overwrapArealWeight_mgcm2, value < 0 ? 0 : value);
        }

        private double _overwrapEffDensity_gcm3 = 1.38;
        public double OverwrapEffDensity_gcm3
        {
            get => _overwrapEffDensity_gcm3;
            set => SetProperty(ref _overwrapEffDensity_gcm3, value < 0 ? 0 : value);
        }

        private double _overwrapCost_EURm2 = 0.15;
        public double OverwrapCost_EURm2
        {
            get => _overwrapCost_EURm2;
            set => SetProperty(ref _overwrapCost_EURm2, value < 0 ? 0 : value);
        }

        private string _overwrapThicknessNote = "Spec sheet";
        public string OverwrapThicknessNote
        {
            get => _overwrapThicknessNote;
            set => SetProperty(ref _overwrapThicknessNote, value);
        }

        private string _overwrapArealWeightNote = "Calculated";
        public string OverwrapArealWeightNote
        {
            get => _overwrapArealWeightNote;
            set => SetProperty(ref _overwrapArealWeightNote, value);
        }

        private string _overwrapEffDensityNote = "Lit. PET";
        public string OverwrapEffDensityNote
        {
            get => _overwrapEffDensityNote;
            set => SetProperty(ref _overwrapEffDensityNote, value);
        }

        private string _overwrapCostNote = "Supplier quote";
        public string OverwrapCostNote
        {
            get => _overwrapCostNote;
            set => SetProperty(ref _overwrapCostNote, value);
        }

        // Insulation Shell Material Properties
        private double _insShellThicknessSum_um = 50.0;
        public double InsShellThicknessSum_um
        {
            get => _insShellThicknessSum_um;
            set => SetProperty(ref _insShellThicknessSum_um, value < 0 ? 0 : value);
        }

        private double _insShellArealWeight_mgcm2 = 0.69;
        public double InsShellArealWeight_mgcm2
        {
            get => _insShellArealWeight_mgcm2;
            set => SetProperty(ref _insShellArealWeight_mgcm2, value < 0 ? 0 : value);
        }

        private double _insShellEffDensity_gcm3 = 1.38;
        public double InsShellEffDensity_gcm3
        {
            get => _insShellEffDensity_gcm3;
            set => SetProperty(ref _insShellEffDensity_gcm3, value < 0 ? 0 : value);
        }

        private double _insShellCost_EURm2 = 0.25;
        public double InsShellCost_EURm2
        {
            get => _insShellCost_EURm2;
            set => SetProperty(ref _insShellCost_EURm2, value < 0 ? 0 : value);
        }

        private string _insShellThicknessNote = "Spec sheet";
        public string InsShellThicknessNote
        {
            get => _insShellThicknessNote;
            set => SetProperty(ref _insShellThicknessNote, value);
        }

        private string _insShellArealWeightNote = "Calculated";
        public string InsShellArealWeightNote
        {
            get => _insShellArealWeightNote;
            set => SetProperty(ref _insShellArealWeightNote, value);
        }

        private string _insShellEffDensityNote = "Lit. PET";
        public string InsShellEffDensityNote
        {
            get => _insShellEffDensityNote;
            set => SetProperty(ref _insShellEffDensityNote, value);
        }

        private string _insShellCostNote = "Supplier quote";
        public string InsShellCostNote
        {
            get => _insShellCostNote;
            set => SetProperty(ref _insShellCostNote, value);
        }

        #endregion

        // TODO: Hook prismatic sheet geometry into prismatic-specific Packaging/Case and compression later
        // TODO: Add prismatic-specific properties (wall thickness, gaps, etc.) in future iterations
        // TODO: Replace flag stubs with actual prismatic tab/terminal design
    }
}
