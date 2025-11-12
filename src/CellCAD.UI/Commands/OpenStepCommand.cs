using System;
using System.Reflection;
using CellCAD.UI.Infrastructure;
using CellCAD.services;

namespace CellCAD.commands
{
    /// <summary>
    /// Open a STEP/STP file into the embedded CADability viewer and fit the view.
    /// </summary>
    public sealed class OpenStepCommand : RelayCommand
    {
        public OpenStepCommand(ICadHost cadHost, IFileDialogService dialogs)
            : base(_ => Execute(cadHost, dialogs), _ => cadHost != null && cadHost.CadControl != null)
        { }

        private static void Execute(ICadHost cadHost, IFileDialogService dialogs)
        {
            var path = dialogs.ShowOpenFile(
                title: "Open STEP File",
                filters: new[]
                {
                    ("STEP files","*.step;*.stp"),
                    ("All supported","*.step;*.stp;*.stl;*.dxf"),
                    ("All files","*.*")
                });

            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                var cad = cadHost.CadControl;

                // Prefer CadControl.ShowFile(string) if present.
                var showFile = cad.GetType().GetMethod("ShowFile", new[] { typeof(string) });
                if (showFile != null)
                {
                    showFile.Invoke(cad, new object[] { path });
                    FitActiveView(cadHost);
                    return;
                }

                // Fallback: find STEP importer via reflection.
                var frame = cad.CadFrame ?? throw new InvalidOperationException("CadFrame not initialized.");
                var project = frame.Project ?? throw new InvalidOperationException("CAD Project not available.");

                var cadAsm = typeof(CADability.Project).Assembly;
                MethodInfo? importer = null;
                object? importerInstance = null;

                foreach (var t in cadAsm.GetTypes())
                {
                    // net48: no string.Contains(string, StringComparison)
                    if (t.Name.IndexOf("Step", StringComparison.OrdinalIgnoreCase) < 0) continue;

                    importer = t.GetMethod("Import", new[] { typeof(CADability.Project), typeof(string) })
                             ?? t.GetMethod("Read", new[] { typeof(CADability.Project), typeof(string) });

                    if (importer != null)
                    {
                        importerInstance = importer.IsStatic ? null : Activator.CreateInstance(t);
                        break;
                    }
                }

                if (importer == null)
                    throw new NotSupportedException("STEP importer not found in CADability assembly. Update integration.");

                importer.Invoke(importerInstance, new object[] { project, path });

                FitActiveView(cadHost);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to open STEP file.\n\n{ex.Message}",
                    "Open STEP",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private static void FitActiveView(ICadHost host)
        {
            var frame = host.CadControl?.CadFrame;
            var view = frame?.ActiveView;
            if (view == null) return;

            var t = view.GetType();
            t.GetMethod("ZoomToExtent")?.Invoke(view, null);
            t.GetMethod("FitAll")?.Invoke(view, null);
            t.GetMethod("ZoomAll")?.Invoke(view, null);
            t.GetMethod("ZoomAllObjects")?.Invoke(view, null);

            host.CadControl?.Invalidate();
        }
    }
}
