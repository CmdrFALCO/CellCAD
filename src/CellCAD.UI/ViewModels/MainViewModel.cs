using CellCAD.UI.Commands;
using CellCAD.services;


namespace CellCAD.viewmodels
{
    public sealed class MainViewModel
    {
        public OpenStepCommand OpenStep { get; }


        public MainViewModel(ICadHost cadHost, IFileDialogService dialogs)
        {
            OpenStep = new OpenStepCommand(cadHost, dialogs);
        }
    }
}
