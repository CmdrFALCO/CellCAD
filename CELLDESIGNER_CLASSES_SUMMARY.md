# CellDesigner Analysis for CellCAD Integration

## Key Finding

There is NO explicit 'Pouch' class in CellDesigner. Instead, a generic 'Cell' class represents the electrochemical unit (pouch cells).

## Main Classes

1. Cell - Root container
2. Electrode - Anode/Cathode (identified by type)
3. Separator - Separator layer
4. Foil - Current collectors
5. CoatingProperties - Electrode slurry properties
6. BasicMaterial - Material definitions
7. SlurryMaterials - Material composition
8. ECUproperties - Calculated cell properties
9. ElectrodeSwelling - Dimensional changes
10. ElectrochemicalPropertiesSolid - Material properties

## Namespace
`CellDesigner.models`

## Property Units (SI-based)

- **Thickness**: [m] meters
- **Areal Weight/Mass**: [kg/m²]
- **Density**: [kg/m³]
- **Voltage**: [V] volts
- **Conductivity**: [S/m] Siemens per meter
- **Areal Capacity**: [C/m²] Coulombs per square meter
- **Specific Capacity**: [C/kg] Coulombs per kilogram
- **Energy Density**: [J/kg] gravimetric, [J/m³] volumetric
- **Heat Capacity**: [J/(kg·K)]
- **Heat Conductivity**: [W/(m·K)]
- **Lithiation**: [dimensionless] 0 to 1 range
- **Porosity**: [dimensionless] 0 to 1 range

## File Locations
Base: `D:\_Projects\CellDesigner\CellDesignerClient\CellDesigner\`

Models: `models\` directory (28+ files)
Database: `database\` directory
Enums: `enums\` directory
ViewModels: `viewmodels\` directory

## Key Property Naming Patterns

1. **Naming Style**: camelCase
2. **Input Flags**: `[Property]IsInput: bool`
3. **Source Comments**: `[Property]Comment: string`
4. **Calculation Methods**: 
   - Getters: `PropertyName()`
   - Setters: `SetPropertyName(value)`
   - Calculators: `CalculateProperty()` or `CalulateProperty()`

## Enum Values Pattern
ALL_CAPS_WITH_UNDERSCORES
Common prefix: `IS_[STATE]`
Examples: `IS_NULL`, `IS_ANODE`, `IS_CATHODE`, `IS_VALID`, `IS_NEW`

## Important Notes

1. **No "Pouch" Class** - Architecture is generic, cell-type agnostic
2. **Numeric Type**: Always `decimal` (not double/float)
3. **Nullable Properties**: Use `decimal?`, `string?` for optional values
4. **Misspellings** (keep for compatibility):
   - `systemLihtiumLoss`
   - `CalulateProperties()`
   - `ElectrodeCalulatedProperties`
   - `SitesInvetory()`
5. **JSON Support**: CreateJson() and FromJson() methods
6. **Database**: MSSQL (comsolDB on corporate server)

## Database Pattern
- Each model class has associated *Functions.cs file
- ID fields support persistence
- Header classes manage versions/entries
- Foreign key relationships through ID fields

## Critical Classes for Integration

### Cell
- Container for anode, cathode, separator, electrolyte
- Links to ECUproperties for calculations

### Electrode
- Represents anode or cathode
- Contains CoatingProperties (loading, thickness, porosity)
- Contains ElectrodeSwelling for dimensional changes
- Linked to Foil for current collector
- Has SlurryOCVPoints for lithiation limits

### CoatingProperties
- **Properties**: loading [kg/m²], thickness [m], porosity, coatingDensity [kg/m³]
- **Pattern**: Multiple calculation methods for interdependent parameters
- **Methods**: CalcFromLoadingAndPorosity(), CalcFromLoadingAndThickness(), etc.

### SlurryMaterials
- List of SlurryMaterialElement with mass fractions
- Calculates: lithiumInventory, sitesInventory [C/kg]
- Generates: chargeOCVTable and dischargeOCVTable (SlurryOCVTable)
- Has compositional properties: density, massFractionsSumm

### ECUproperties
- Cell-level calculated properties
- State variables: xAnode0/100, xCathode0/100, u0/u100Charge/Discharge
- Calculated: gravimetricEnergyDensity, volumetricEnergyDensity, nToPRatio
- Methods: SetSoC0FromAnodeX(), SetSoC100FromAnodeU(), etc.

## Component Hierarchy
```
Workspace
  > Group
    > Cell
      > Electrode (anode)
        > CoatingProperties
          > SlurryMaterials (List<SlurryMaterialElement>)
            > BasicMaterial
            > chargeOCVTable (SlurryOCVTable)
            > dischargeOCVTable (SlurryOCVTable)
        > Foil (currentCollector, List<FoilMaterialElement>)
        > ElectrodeSwelling (List<SwellingStep>)
      > Electrode (cathode) [same structure]
      > Separator (has Foil)
      > BasicMaterial (electrolyte)
      > ECUproperties (calculated)
```

## Must-Match Property Names for Integration

**Electrode**:
- coatingProperties, currentCollector, currentCollectorThickness
- electrodeConductivity, contactResistance
- chargeMin, chargeMax, dischargeMin, dischargeMax

**CoatingProperties**:
- loading, thickness, porosity, coatingDensity
- loadingIsInput, thicknessIsInput, porosityIsInput
- slurryMaterials

**BasicMaterial**:
- density, specificElectricConductivity, heatCapacity, heatConductivity
- molarMass, electrochemicalPropertiesSolid

**SlurryMaterials**:
- density, lithiumInventory, lithiumLoss, sitesInventory, sitesLoss
- chargeOCVTable, dischargeOCVTable, massFractionsSumm

## Enumerations
- `CellComponentDisplayState`: IS_NULL, IS_ANODE, IS_CATHODE, IS_ELECTROLYTE, IS_SEPARATOR
- `CellComponentEditModeState`: IS_NULL, IS_NEW, IS_BROWSER, IS_REFERENCE, IS_CLONE
- `ValidationState`: IS_NULL, IS_VALID, IS_NOT_VALID
- `UpdateValidationState`: IS_SAME_AND_VALID, IS_NEW_AND_VALID, IS_NOT_VALID

## OCV Curves
Three related classes for handling open circuit voltage data:
- `OCVCurve` (List<OCVPoint>) - Single material
- `SlurryOCVTable` (List<SlurryOCVPoint>) - Mixed materials
- Methods: InterpolateU(x), InterpolateX(voltage), AverageVoltage(min, max)

## Database Connection
Server: smtcac2746.rd.corpintra.net\SQLEXPRESS,1433
Database: comsolDB
Auth: Integrated Security
Files: *Functions.cs and *Procedures.cs for data operations

