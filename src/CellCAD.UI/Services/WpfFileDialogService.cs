using System.Collections.Generic;
using System.Linq;
// Alias the WPF file dialog to avoid clash with System.Windows.Forms.OpenFileDialog
using Win32 = Microsoft.Win32;

namespace CellCAD.services
{
    public sealed class WpfFileDialogService : IFileDialogService
    {
        public string? ShowOpenFile(string title, IEnumerable<(string Label, string Pattern)> filters)
        {
            var dlg = new Win32.OpenFileDialog
            {
                Title = title,
                Filter = string.Join("|", filters.Select(f => $"{f.Label} ({f.Pattern})|{f.Pattern}")),
                CheckFileExists = true,
                Multiselect = false
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
