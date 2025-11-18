namespace CellCAD.viewmodels
{
    /// <summary>
    /// Represents a packaging component in the Table tab (editable, manual entry)
    /// </summary>
    public class PackagingComponent
    {
        public int No { get; set; }
        public string? Name { get; set; }
        public string? Material { get; set; }
        public string? Version { get; set; }
        public double? Mass_g { get; set; }
        public double? Volume_cm3 { get; set; }
        public double? Density_gcm3 { get; set; }
        public double? Cost_EUR { get; set; }
    }
}
