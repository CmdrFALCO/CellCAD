using System.Collections.Generic;


namespace CellCAD.services
{
    public interface IFileDialogService
    {
        /// <summary>
        /// Shows an Open File dialog and returns the selected file path or null if cancelled.
        /// </summary>
        string? ShowOpenFile(string title, IEnumerable<(string Label, string Pattern)> filters);
    }
}
