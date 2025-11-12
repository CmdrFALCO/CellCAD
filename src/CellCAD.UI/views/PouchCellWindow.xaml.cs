using System.Windows;
using System.ComponentModel;
using CellCAD.services;
using CellCAD.viewmodels;
using CellCAD.models;


namespace CellCAD.views
{
    public partial class PouchCellWindow : Window
    {
        private CadSceneHost? _scene;
        private PouchCellViewModel _vm;
        private ElectrodeSchematicControl? _schematic;


        public PouchCellWindow()
        {
            InitializeComponent();

            _vm = new PouchCellViewModel(PouchCellParameters.NeutralPreset);
            DataContext = _vm;

            // Initialize 2D CAD viewport with Canvas
            _scene = new CadSceneHost();
            ViewportHost.Content = _scene.Canvas;

            // Initialize 2D schematic
            _schematic = new ElectrodeSchematicControl();
            SchematicHost.Child = _schematic;

            Loaded += (_, __) =>
            {
                _scene.BuildPouchCell(_vm.Model);
                _schematic.UpdateSchematic(_vm.Model);
            };

            _vm.PropertyChanged += (_, __) =>
            {
                _scene.BuildPouchCell(_vm.Model);
                _schematic?.UpdateSchematic(_vm.Model);
            };
        }

        private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _scene?.BuildPouchCell(_vm.Model);
        }

        private void Normalize_Click(object sender, RoutedEventArgs e)
        {
            _vm.Normalize();
        }


        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            // Canvas auto-fits geometry in BuildPouchCell
            _scene?.BuildPouchCell(_vm.Model);
        }


        // Optional: restrict key input to numeric for our TextBoxes
        private void NumericOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.' && c != ',' && c != '-') { e.Handled = true; return; }
            }
        }


        protected override void OnClosed(System.EventArgs e)
        {
            _vm.PropertyChanged -= ViewModelOnPropertyChanged;
            base.OnClosed(e);
            _scene?.Dispose();
        }
    }
}
