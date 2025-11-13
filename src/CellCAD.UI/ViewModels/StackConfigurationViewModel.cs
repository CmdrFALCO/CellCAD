using System.ComponentModel;
using CellCAD.viewmodels;

namespace CellCAD.viewmodels
{
    /// <summary>
    /// Enumerations for stack configuration options
    /// </summary>
    public enum EndElectrodesMode
    {
        BothNegative,
        BothPositive,
        PositiveNegative
    }

    public enum EndCoatingsMode
    {
        BothDoubleSided,
        OneSingleSided,
        BothSingleSided
    }

    /// <summary>
    /// ViewModel for Stack Configuration tab
    /// </summary>
    public class StackConfigurationViewModel : NotifyBase
    {
        #region Input Properties (TwoWay - Black Fields)

        private int _numberOfStacksInCell = 2;
        public int NumberOfStacksInCell
        {
            get => _numberOfStacksInCell;
            set
            {
                _numberOfStacksInCell = value;
                Raise();
                Raise(nameof(ElectrodePairsInCell));
                // Notify all computed properties
                NotifyAllComputedProperties();
            }
        }

        private int _electrodePairsPerStack = 50;
        public int ElectrodePairsPerStack
        {
            get => _electrodePairsPerStack;
            set
            {
                _electrodePairsPerStack = value;
                Raise();
                Raise(nameof(ElectrodePairsInCell));
                NotifyAllComputedProperties();
            }
        }

        private int _separatorOverwrapsPerStack = 1;
        public int SeparatorOverwrapsPerStack
        {
            get => _separatorOverwrapsPerStack;
            set
            {
                _separatorOverwrapsPerStack = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        private int _additionalOverwrapsPerStack = 0;
        public int AdditionalOverwrapsPerStack
        {
            get => _additionalOverwrapsPerStack;
            set
            {
                _additionalOverwrapsPerStack = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        private int _insulationShellAroundAllStacks = 1;
        public int InsulationShellAroundAllStacks
        {
            get => _insulationShellAroundAllStacks;
            set
            {
                _insulationShellAroundAllStacks = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        private int _fixingTapesPerStack = 1;
        public int FixingTapesPerStack
        {
            get => _fixingTapesPerStack;
            set
            {
                _fixingTapesPerStack = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        private bool _fixingTapesOnAllStacks = true;
        public bool FixingTapesOnAllStacks
        {
            get => _fixingTapesOnAllStacks;
            set
            {
                _fixingTapesOnAllStacks = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        #endregion

        #region Radio Button Enum Properties

        private EndElectrodesMode _endElectrodes = EndElectrodesMode.BothNegative;
        public EndElectrodesMode EndElectrodes
        {
            get => _endElectrodes;
            set
            {
                _endElectrodes = value;
                Raise();
                NotifyAllComputedProperties();
            }
        }

        private EndCoatingsMode _endCoatings = EndCoatingsMode.BothDoubleSided;
        public EndCoatingsMode EndCoatings
        {
            get => _endCoatings;
            set
            {
                _endCoatings = value;
                Raise();
                // Coatings don't affect geometry, but might affect thickness calculations later
            }
        }

        #endregion

        #region Computed Properties (OneWay - Gray Fields)

        /// <summary>
        /// Total electrode pairs in cell = stacks × pairs per stack
        /// </summary>
        public int ElectrodePairsInCell => NumberOfStacksInCell * ElectrodePairsPerStack;

        #endregion

        #region Number of Sheet Layers (in z) - Computed Geometry

        /// <summary>
        /// Number of cathode sheets per stack (depends on end electrode configuration)
        /// </summary>
        public int CathodeSheets_Stack
        {
            get
            {
                int N = ElectrodePairsPerStack;
                return EndElectrodes switch
                {
                    EndElectrodesMode.BothNegative => N,           // N cathodes
                    EndElectrodesMode.BothPositive => N + 1,       // N+1 cathodes
                    EndElectrodesMode.PositiveNegative => N,       // N cathodes
                    _ => N
                };
            }
        }

        /// <summary>
        /// Number of anode sheets per stack (depends on end electrode configuration)
        /// </summary>
        public int AnodeSheets_Stack
        {
            get
            {
                int N = ElectrodePairsPerStack;
                return EndElectrodes switch
                {
                    EndElectrodesMode.BothNegative => N + 1,       // N+1 anodes
                    EndElectrodesMode.BothPositive => N,           // N anodes
                    EndElectrodesMode.PositiveNegative => N,       // N anodes
                    _ => N + 1
                };
            }
        }

        /// <summary>
        /// Total electrode sheets per stack = cathodes + anodes
        /// </summary>
        public int AllElectrodeSheets_Stack => CathodeSheets_Stack + AnodeSheets_Stack;

        /// <summary>
        /// Number of separator sheets per stack
        /// Formula: cathodes + anodes + 1
        /// </summary>
        public int SeparatorSheets_Stack => CathodeSheets_Stack + AnodeSheets_Stack + 1;

        /// <summary>
        /// Overwrap sheets per stack (separator + additional overwraps)
        /// </summary>
        public int Overwrap_Stack => SeparatorOverwrapsPerStack + AdditionalOverwrapsPerStack;

        /// <summary>
        /// Insulation shell per stack (fractional, for display)
        /// </summary>
        public double InsulationShell_Stack
        {
            get
            {
                if (NumberOfStacksInCell > 0)
                    return (double)InsulationShellAroundAllStacks / NumberOfStacksInCell;
                return 0.0;
            }
        }

        /// <summary>
        /// Fixing tape per stack
        /// </summary>
        public int FixingTape_Stack => FixingTapesPerStack;

        // Cell totals (multiply by number of stacks)

        public int AllElectrodeSheets_Cell => AllElectrodeSheets_Stack * NumberOfStacksInCell;
        public int CathodeSheets_Cell => CathodeSheets_Stack * NumberOfStacksInCell;
        public int AnodeSheets_Cell => AnodeSheets_Stack * NumberOfStacksInCell;
        public int SeparatorSheets_Cell => SeparatorSheets_Stack * NumberOfStacksInCell;
        public int Overwrap_Cell => Overwrap_Stack * NumberOfStacksInCell;
        public int InsulationShell_Cell => InsulationShellAroundAllStacks;

        /// <summary>
        /// Fixing tape total for cell - depends on FixingTapesOnAllStacks setting
        /// </summary>
        public int FixingTape_Cell => FixingTapesOnAllStacks
            ? FixingTape_Stack * NumberOfStacksInCell
            : FixingTapesPerStack;

        #endregion

        #region Calculated Thickness - Placeholder (DB-driven later)

        // TODO: Values populated from materials DB in a later phase
        public double? SingleStackThickness_Dry => null;
        public double? SingleStackThickness_SoC0 => null;
        public double? SingleStackThickness_SoC100 => null;

        public double? AllStacksThickness_Dry => null;
        public double? AllStacksThickness_SoC0 => null;
        public double? AllStacksThickness_SoC100 => null;

        public double? CellOuterThickness_Dry => null;
        public double? CellOuterThickness_SoC0 => null;
        public double? CellOuterThickness_SoC100 => null;

        public double? SwellingStack_SoC0 => null;
        public double? SwellingStack_SoC100 => null;

        public double? SwellingCell_SoC0 => null;
        public double? SwellingCell_SoC100 => null;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Notify all computed properties when inputs change
        /// </summary>
        private void NotifyAllComputedProperties()
        {
            // Sheet layer counts
            Raise(nameof(CathodeSheets_Stack));
            Raise(nameof(AnodeSheets_Stack));
            Raise(nameof(AllElectrodeSheets_Stack));
            Raise(nameof(SeparatorSheets_Stack));
            Raise(nameof(Overwrap_Stack));
            Raise(nameof(InsulationShell_Stack));
            Raise(nameof(FixingTape_Stack));

            Raise(nameof(AllElectrodeSheets_Cell));
            Raise(nameof(CathodeSheets_Cell));
            Raise(nameof(AnodeSheets_Cell));
            Raise(nameof(SeparatorSheets_Cell));
            Raise(nameof(Overwrap_Cell));
            Raise(nameof(InsulationShell_Cell));
            Raise(nameof(FixingTape_Cell));
        }

        #endregion
    }
}
