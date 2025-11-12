using System.Windows.Controls;

namespace CellCAD.services
{
    /// <summary>
    /// Exposes the Canvas control for 2D wireframe rendering to ViewModels/Commands.
    /// </summary>
    public interface ICadHost
    {
        Canvas Canvas { get; }
    }
}
