using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using CellCAD.services;
using CellCAD.viewmodels;
using CellCAD.Core.Geometry;


namespace CellCAD.views
{
    public partial class PouchCellWindow : Window
    {
        private CadSceneHost? _scene;
        private PouchCellViewModel _pouchVm;
        private PrismaticCellViewModel _prismaticVm;
        private CellDesignerShellViewModel _shellVm;
        private StackConfigurationViewModel _stackVm;
        private Canvas? _canvasFullLayout;
        private Canvas? _canvasCornerZoom;
        private Canvas? _canvasCasePreview;


        public PouchCellWindow()
        {
            InitializeComponent();

            _pouchVm = new PouchCellViewModel(PouchCellParameters.NeutralPreset);
            _prismaticVm = new PrismaticCellViewModel();
            _shellVm = new CellDesignerShellViewModel(_pouchVm, _prismaticVm);
            _stackVm = new StackConfigurationViewModel();
            DataContext = _shellVm;

            // Set Stack Configuration panel's DataContext after InitializeComponent
            // (PanelStackConfig is defined in XAML)
            Loaded += (s, e) =>
            {
                PanelStackConfig.DataContext = _stackVm;
            };

            // Initialize 2D CAD viewport with Canvas
            _scene = new CadSceneHost();

            // Create canvases for sheet design views
            _canvasFullLayout = new Canvas { Background = System.Windows.Media.Brushes.White };
            _canvasCornerZoom = new Canvas { Background = System.Windows.Media.Brushes.White };

            ViewportHost.Content = _canvasFullLayout;
            SchematicHost.Child = _canvasCornerZoom;

            Loaded += (_, __) =>
            {
                RenderSheetViews();
                RenderCasePreview();
            };

            SizeChanged += (_, __) =>
            {
                RenderSheetViews();
                RenderCasePreview();
            };

            _pouchVm.PropertyChanged += (_, __) =>
            {
                RenderSheetViews();
                RenderCasePreview();
            };

            // Wire up CasePreviewCanvas for Packaging -> Case tab
            Loaded += (_, __) =>
            {
                if (CasePreviewCanvas != null)
                {
                    _canvasCasePreview = CasePreviewCanvas;
                    CasePreviewCanvas.SizeChanged += (s, e) => RenderCasePreview();
                }
            };
        }

        private void RenderSheetViews()
        {
            if (_canvasFullLayout != null)
            {
                SheetRenderer.RenderFullLayout(_canvasFullLayout, _pouchVm);
            }
            if (_canvasCornerZoom != null)
            {
                SheetRenderer.RenderCornerZoom(_canvasCornerZoom, _pouchVm);
            }
        }

        private void RenderCasePreview()
        {
            if (_canvasCasePreview != null)
            {
                SheetRenderer.DrawPackagingCase(_canvasCasePreview, _pouchVm);
            }
        }

        private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _scene?.BuildPouchCell(_pouchVm.Model);
        }

        private void Normalize_Click(object sender, RoutedEventArgs e)
        {
            _pouchVm.Normalize();
        }


        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            // Canvas auto-fits geometry in BuildPouchCell
            _scene?.BuildPouchCell(_pouchVm.Model);
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
            _pouchVm.PropertyChanged -= ViewModelOnPropertyChanged;
            base.OnClosed(e);
            _scene?.Dispose();
        }
    }
}
