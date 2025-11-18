namespace CellCAD.views
{
    /// <summary>
    /// Interaction logic for SheetDesignView.xaml
    /// </summary>
    public partial class SheetDesignView : System.Windows.Controls.UserControl
    {
        public SheetDesignView()
        {
            InitializeComponent();
        }

        private void NumericOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.' && c != ',' && c != '-') { e.Handled = true; return; }
            }
        }
    }
}
