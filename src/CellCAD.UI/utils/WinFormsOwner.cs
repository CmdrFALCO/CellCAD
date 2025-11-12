using WinForms = System.Windows.Forms;
using System.Drawing;

namespace CellCAD.utils
{
    public static class WinFormsOwner
    {
        private static WinForms.Form? _owner;

        /// <summary>Ensures a hidden WinForms owner form exists so CADability dialogs don't crash.</summary>
        public static WinForms.IWin32Window Ensure()
        {
            if (_owner != null && !_owner.IsDisposed) return _owner;

            _owner = new WinForms.Form
            {
                ShowInTaskbar = false,
                FormBorderStyle = WinForms.FormBorderStyle.FixedToolWindow,
                StartPosition = WinForms.FormStartPosition.Manual,
                Location = new Point(-32000, -32000),
                Size = new Size(1, 1),
                Opacity = 0.01,
                TopMost = false
            };

            // Make sure it becomes part of Application.OpenForms, then hide it.
            _owner.Load += (_, __) => _owner.Hide();
            _owner.Shown += (_, __) => _owner.Hide();
            _owner.Show();  // important: this registers it in OpenForms

            return _owner;
        }
    }
}
