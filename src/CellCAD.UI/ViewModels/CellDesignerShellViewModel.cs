using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CellCAD.viewmodels
{
    /// <summary>
    /// Shell ViewModel that orchestrates the cell designer UI.
    /// Holds references to per-cell-type ViewModels and manages cell type selection.
    /// </summary>
    public partial class CellDesignerShellViewModel : ObservableObject
    {
        private CellType _selectedCellType;

        public CellDesignerShellViewModel(PouchCellViewModel pouchVm, PrismaticCellViewModel prismaticVm)
        {
            PouchVm = pouchVm ?? throw new ArgumentNullException(nameof(pouchVm));
            PrismaticVm = prismaticVm ?? throw new ArgumentNullException(nameof(prismaticVm));

            _selectedCellType = CellType.Pouch; // default to Pouch
        }

        /// <summary>
        /// ViewModel for Pouch cell type
        /// </summary>
        public PouchCellViewModel PouchVm { get; }

        /// <summary>
        /// ViewModel for Prismatic cell type
        /// </summary>
        public PrismaticCellViewModel PrismaticVm { get; }

        /// <summary>
        /// Currently selected cell type (Pouch, Prismatic, or Cylindrical)
        /// </summary>
        public CellType SelectedCellType
        {
            get => _selectedCellType;
            set
            {
                if (_selectedCellType != value)
                {
                    _selectedCellType = value;
                    OnPropertyChanged(nameof(SelectedCellType));
                }
            }
        }
    }
}
