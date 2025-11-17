# CellCAD – Named Objects Reference

**Document Version:** 1.1
**Last Updated:** 2025-11-17
**Application:** CellCAD.UI – Pouch Cell Stack Designer
**Target Framework:** .NET 10.0 (Windows)
**File:** `PouchCellWindow.xaml` + `PouchCellWindow.xaml.cs`

---

## Table of Contents

1. [Overview](#overview)
2. [Navigation Elements](#navigation-elements)
3. [Main Tab Panels](#main-tab-panels)
4. [Packaging Sub-Panels](#packaging-sub-panels)
5. [Preview and Visualization Elements](#preview-and-visualization-elements)
6. [Data Context and ViewModels](#data-context-and-viewmodels)
7. [Key Binding Properties](#key-binding-properties)
8. [Quick Reference Table](#quick-reference-table)

---

## Overview

This document provides a complete reference of all named XAML elements and their relationships in the CellCAD Pouch Cell Window. Named elements are UI components with `x:Name` attributes that can be referenced in code-behind or bindings.

**File Locations:**
- XAML: `src\CellCAD.UI\views\PouchCellWindow.xaml`
- Code-behind: `src\CellCAD.UI\views\PouchCellWindow.xaml.cs`
- Main ViewModel: `src\CellCAD.UI\ViewModels\PouchCellViewModel.cs`
- Stack ViewModel: `src\CellCAD.UI\ViewModels\StackConfigurationViewModel.cs`

---

## Navigation Elements

### Main Section Tabs (RadioButtons)

| Name | Type | Content | Purpose | Group | Location |
|------|------|---------|---------|-------|----------|
| `TabSheetDesign` | RadioButton | "Sheet Design" | Selects Sheet Design panel | MainTabs | Left sidebar, line ~190 |
| `TabStackConfig` | RadioButton | "Stack Config." | Selects Stack Configuration panel | MainTabs | Left sidebar, line ~191 |
| `TabPackaging` | RadioButton | "Packaging" | Selects Packaging panel + shows sub-tabs | MainTabs | Left sidebar, line ~192 |

**Default State:** `TabSheetDesign` is checked on startup (`IsChecked="True"`)

**Style:** `MainTabStyle` – Blue background when active (#0078D4), gray when inactive (#E8E8E8)

**Behavior:**
- Only one main tab can be active at a time
- Activating a tab shows its corresponding panel via `BoolToVisibilityConverter`
- `TabPackaging` also reveals the sub-tab stack panel when checked

---

### Packaging Sub-Tabs (RadioButtons)

| Name | Type | Content | Purpose | Group | Location |
|------|------|---------|---------|-------|----------|
| `SubTabCase` | RadioButton | "Case" | Shows Package Material, Composition, Offsets | PackagingSubTabs | Left sidebar, line ~196 |
| `SubTabTabs` | RadioButton | "Tabs" | Shows Tab Design section | PackagingSubTabs | Left sidebar, line ~197 |
| `SubTabAddFoils` | RadioButton | "Add. Foils" | Shows Additional Foils section (placeholder) | PackagingSubTabs | Left sidebar, line ~198 |
| `SubTabFixingTape` | RadioButton | "Fixing Tape" | Shows Fixing Tape section (placeholder) | PackagingSubTabs | Left sidebar, line ~199 |

**Default State:** `SubTabCase` is checked when Packaging tab is opened

**Style:** `SubTabStyle` – Indented sub-tab style, blue when active

**Visibility:** Entire sub-tab stack is visible only when `TabPackaging.IsChecked == true`

---

## Main Tab Panels

### Sheet Design Panel

**Name:** `PanelSheetDesign`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=TabSheetDesign, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~219-505
**DataContext:** `PouchCellViewModel` (inherited from Window)

**Sections:**
1. **Sheet Dimensions** (Lines ~224-339)
   - Cathode: Height, Width, Area (calculated)
   - Anode Offset to Cathode: Top, Bottom, Left, Right
   - Anode: Height, Width, Area (calculated)
   - Separator Offset to Anode: Top, Bottom, Left, Right
   - Separator: Height, Width, Area (calculated)

2. **Coating Thickness** (Lines ~341-456)
   - Cathode coating: active mass thickness (1-sided, 2-sided)
   - Cathode current collector thickness
   - Anode coating: active mass thickness (1-sided, 2-sided)
   - Anode current collector thickness

3. **Options** (Lines ~458-503)
   - CheckBox: "Update Stack Dimensions based on Separator" → `UpdateStackDimensions`
   - CheckBox: "Show Dimension Lines" → `ShowDimensionLines`
   - Button: "Normalize" → `Normalize_Click` handler
   - Button: "Fit" → `Fit_Click` handler

**Key Bindings:**
- `CathodeHeight_mm`, `CathodeWidth_mm`, `CathodeArea_cm2`
- `AnodeOffsetTop_mm`, `AnodeOffsetBottom_mm`, `AnodeOffsetLeft_mm`, `AnodeOffsetRight_mm`
- `AnodeHeight_mm`, `AnodeWidth_mm`, `AnodeArea_cm2`
- `SeparatorOffsetTop_mm`, `SeparatorOffsetBottom_mm`, `SeparatorOffsetLeft_mm`, `SeparatorOffsetRight_mm`
- `SeparatorHeight_mm`, `SeparatorWidth_mm`, `SeparatorArea_cm2`

---

### Stack Configuration Panel

**Name:** `PanelStackConfig`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=TabStackConfig, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~508-607
**DataContext:** `StackConfigurationViewModel` (set in code-behind via `PanelStackConfig.DataContext = _stackVm`)

**Sections:**
1. **Stack Inputs** (Lines ~513-607)
   - Number of Stacks in Cell
   - Electrode Pairs per Stack
   - Electrode Pairs in Cell (calculated)
   - Separator Overwraps per Stack
   - Additional Overwraps per Stack
   - Insulation Shell around all Stacks
   - Fixing Tapes per Stack

2. **End Electrodes Radio Buttons** (Lines ~580-589)
   - Both Negative (default)
   - Both Positive
   - Asymmetric (Positive + Negative)
   - Bound to: `EndElectrodes` enum property

3. **End Coatings Radio Buttons** (Lines ~591-600)
   - Both Double-sided (default)
   - Both Single-sided
   - Asymmetric (Double + Single)
   - Bound to: `EndCoatings` enum property

4. **Fixing Tapes Option** (Line ~611)
   - CheckBox: `x:Name="FixingTapesOnAllStacksCheckBox"`
   - Content: "Fixing Tapes on all stacks?"
   - Binding: `FixingTapesOnAllStacks`

**Sheet Layer Counts (Calculated - Gray Fields):**
- Per Stack: `CathodeSheets_Stack`, `AnodeSheets_Stack`, `AllElectrodeSheets_Stack`, `SeparatorSheets_Stack`, `Overwrap_Stack`, `InsulationShell_Stack`, `FixingTape_Stack`
- Per Cell: `CathodeSheets_Cell`, `AnodeSheets_Cell`, `AllElectrodeSheets_Cell`, `SeparatorSheets_Cell`, `Overwrap_Cell`, `InsulationShell_Cell`, `FixingTape_Cell`

**Calculated Thickness (Placeholder):**
- `SingleStackThickness_Dry` (null)
- `SingleStackThickness_SoC0` (null)
- `SingleStackThickness_SoC100` (null)
- `AllStacksThickness_Dry` (null)
- `AllStacksThickness_SoC0` (null)
- `AllStacksThickness_SoC100` (null)

**Key ViewModel:** `StackConfigurationViewModel` (separate from main VM)

---

### Packaging Panel (Container)

**Name:** `PanelPackaging`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=TabPackaging, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~875-1311
**DataContext:** `PouchCellViewModel` (inherited)

**Contains 4 Sub-Panels:**
- `SubPanelCase` (Case tab content)
- `SubPanelTabs` (Tabs tab content)
- `SubPanelAddFoils` (Add. Foils tab content)
- `SubPanelFixingTape` (Fixing Tape tab content)

---

## Packaging Sub-Panels

### Case Sub-Panel

**Name:** `SubPanelCase`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=SubTabCase, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~879-1117
**DataContext:** `PouchCellViewModel`

**Sections:**

#### 1. Package Material (Selection) – Lines ~884-903
- ComboBox for material selection (placeholder)
- Currently shows: "Al/PE/PP 115 µm, Version 2 – 15.09.2024"

#### 2. Package Material Composition – Lines ~905-983
- **DataGrid** bound to `CaseComposition` (ObservableCollection<PackagingLayer>)
- **Columns:**
  - `#` → `No` (row number)
  - `Name` → `Name` (e.g., "PA 15µm")
  - `Version` → `Version` (e.g., "1 – 01.10.25")
  - `Thickness [µm]` → `Thickness_um`
  - `Porosity [%]` → `Porosity_pct`
  - `Density [g/cm³]` → `Density_gcm3`
- **Read-only:** `IsReadOnly="True"`
- **Auto-computed totals:** Changes to composition trigger `RecalcFromComposition()`

#### 3. Package Material Properties – Lines ~985-1031
- **Thickness (sum)** → `PackageThicknessSum_um` + note field
- **Areal Weight** → `PackageArealWeight_mgcm2` + note field
- **Effective Density** → `PackageEffDensity_gcm3` + note field
- **Cost** → `PackageCost_EURm2` + note field
- All have TwoWay bindings (user can override auto-calculated values)

#### 4. Pouch foil Offset to Separator – Lines ~1033-1067
- **Top** → `PouchOffsetTop_mm`
- **Bottom** → `PouchOffsetBottom_mm`
- **Left Side** → `PouchOffsetLeft_mm`
- **Right Side** → `PouchOffsetRight_mm`

#### 5. Cell Dimensions (Calculated) – Lines ~1069-1115
- **Calculated Cell height** → `CalculatedHeight_mm` (gray, read-only)
- **Calculated Cell width** → `CalculatedWidth_mm` (gray, read-only)
- CheckBox: "Update Cell Dimension based on measurement" → `UseMeasuredDims`
- **Measured Cell height** → `MeasuredHeight_mm` (enabled when checkbox checked)
- **Measured Cell width** → `MeasuredWidth_mm` (enabled when checkbox checked)

**Removed Sections (as of previous tasks):**
- ❌ Calculated Mass (removed)
- ❌ Tab Design (moved to Tabs sub-tab)

---

### Tabs Sub-Panel

**Name:** `SubPanelTabs`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=SubTabTabs, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~1121-1287
**DataContext:** `PouchCellViewModel`

**Sections:**

#### 1. Tab Design – Lines ~1127-1287
- **Tab Positive:**
  - Position Horizontal → `TabPosHorizontal_mm`
  - Position Vertical → `TabPosVertical_mm`
  - Width → `TabPosWidth_mm`
  - Length → `TabPosLength_mm`
  - Length outside case → `TabPosLengthOutside_mm`

- **Tab Negative:**
  - Position Horizontal → `TabNegHorizontal_mm`
  - Position Vertical → `TabNegVertical_mm`
  - Width → `TabNegWidth_mm`
  - Length → `TabNegLength_mm`
  - Length outside case → `TabNegLengthOutside_mm`

- **Calculated Mass (Placeholder)**
  - "Mass of all tabs per cell" → null (TODO: implement)

---

### Additional Foils Sub-Panel

**Name:** `SubPanelAddFoils`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=SubTabAddFoils, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~1291-1296
**Status:** Placeholder (empty StackPanel)

---

### Fixing Tape Sub-Panel

**Name:** `SubPanelFixingTape`
**Type:** Grid
**Visibility Binding:** `{Binding IsChecked, ElementName=SubTabFixingTape, Converter={StaticResource BoolToVis}}`
**Location:** Lines ~1306-1311
**Status:** Placeholder (empty StackPanel)

---

## Preview and Visualization Elements

### 3D Viewport (Center Column)

**Name:** `ViewportHost`
**Type:** ContentControl
**Location:** Grid.Column="2", Lines ~1364
**Purpose:** Hosts the 3D CAD visualization canvas

**Code-Behind Wiring:**
```csharp
_canvasFullLayout = new Canvas { Background = System.Windows.Media.Brushes.White };
ViewportHost.Content = _canvasFullLayout;
```

**Rendering:** `SheetRenderer.RenderFullLayout(_canvasFullLayout, _vm)`

**Update Triggers:**
- Window `Loaded` event
- Window `SizeChanged` event
- ViewModel `PropertyChanged` event

---

### 2D Schematic (Lower Right)

**Name:** `SchematicHost`
**Type:** Border
**Location:** Grid.Row="1" in right column, Lines ~1391
**Purpose:** Shows zoomed corner view with dimension lines

**Code-Behind Wiring:**
```csharp
_canvasCornerZoom = new Canvas { Background = System.Windows.Media.Brushes.White };
SchematicHost.Child = _canvasCornerZoom;
```

**Rendering:** `SheetRenderer.RenderCornerZoom(_canvasCornerZoom, _vm)`

**Update Triggers:** Same as ViewportHost

---

### Case Preview Canvas (Right Column, Packaging → Case)

**Name:** `CasePreviewCanvas`
**Type:** Canvas
**Location:** Grid.Column="3", visible only when SubTabCase is active, Lines ~1439-1446
**Purpose:** Shows 2D plot of pouch foil and separator with offset labels

**Layout Structure:**
```
Grid (2×2)
├─ Column 0: Y axis label (TextBlock "Y")
├─ Column 1, Row 0: CasePreviewCanvas (with Margin="8")
└─ Column 1, Row 1: X axis label (TextBlock "X")
```

**Code-Behind Wiring:**
```csharp
_canvasCasePreview = CasePreviewCanvas;
CasePreviewCanvas.SizeChanged += (s, e) => RenderCasePreview();
```

**Rendering:** `SheetRenderer.DrawPackagingCase(_canvasCasePreview, _vm)`

**Update Triggers:**
- Window `Loaded` event
- Canvas `SizeChanged` event
- ViewModel `PropertyChanged` event

**Visual Constants (SheetRenderer.cs):**
- Padding: 12px (line 263)
- Pouch foil stroke: 3.0px, DarkGray (line 283)
- Separator stroke: 1.5px, Blue (line 292)
- Offset labels: FontSize 9, DarkGray

---

## Data Context and ViewModels

### Window DataContext

**Set in Code-Behind Constructor:**
```csharp
_vm = new PouchCellViewModel(PouchCellParameters.NeutralPreset);
DataContext = _vm;
```

**Type:** `PouchCellViewModel`
**Scope:** Entire window (inherited by all child elements unless overridden)

---

### Stack Configuration Panel DataContext

**Set in Code-Behind Loaded Event:**
```csharp
_stackVm = new StackConfigurationViewModel();
PanelStackConfig.DataContext = _stackVm;
```

**Type:** `StackConfigurationViewModel`
**Scope:** Only `PanelStackConfig` and its children
**Why Separate:** Stack configuration has distinct logic and doesn't belong to pouch cell parameters

---

## Key Binding Properties

### PouchCellViewModel Properties

#### Sheet Design Section
| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `CathodeHeight_mm` | double | TwoWay | Cathode sheet height |
| `CathodeWidth_mm` | double | TwoWay | Cathode sheet width |
| `CathodeArea_cm2` | double | OneWay | Calculated cathode area |
| `AnodeOffsetTop_mm` | double | TwoWay | Anode offset from cathode (top) |
| `AnodeOffsetBottom_mm` | double | TwoWay | Anode offset from cathode (bottom) |
| `AnodeOffsetLeft_mm` | double | TwoWay | Anode offset from cathode (left) |
| `AnodeOffsetRight_mm` | double | TwoWay | Anode offset from cathode (right) |
| `AnodeHeight_mm` | double | OneWay | Calculated anode height |
| `AnodeWidth_mm` | double | OneWay | Calculated anode width |
| `AnodeArea_cm2` | double | OneWay | Calculated anode area |
| `SeparatorOffsetTop_mm` | double | TwoWay | Separator offset from anode (top) |
| `SeparatorOffsetBottom_mm` | double | TwoWay | Separator offset from anode (bottom) |
| `SeparatorOffsetLeft_mm` | double | TwoWay | Separator offset from anode (left) |
| `SeparatorOffsetRight_mm` | double | TwoWay | Separator offset from anode (right) |
| `SeparatorHeight_mm` | double | OneWay | Calculated separator height |
| `SeparatorWidth_mm` | double | OneWay | Calculated separator width |
| `SeparatorArea_cm2` | double | OneWay | Calculated separator area |
| `UpdateStackDimensions` | bool | TwoWay | Toggle auto-update of stack from separator |
| `ShowDimensionLines` | bool | TwoWay | Toggle dimension lines in preview |

#### Packaging → Case Section
| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `CaseComposition` | ObservableCollection<PackagingLayer> | OneWay | Composition layers (read-only) |
| `PackageThicknessSum_um` | double | TwoWay | Total thickness (auto-computed, overrideable) |
| `PackageArealWeight_mgcm2` | double | TwoWay | Total areal weight (auto-computed) |
| `PackageEffDensity_gcm3` | double | TwoWay | Effective density (auto-computed) |
| `PackageCost_EURm2` | double | TwoWay | Cost per m² |
| `PackageThicknessNote` | string | TwoWay | Note field for thickness |
| `PackageArealWeightNote` | string | TwoWay | Note field for areal weight |
| `PackageEffDensityNote` | string | TwoWay | Note field for density |
| `PackageCostNote` | string | TwoWay | Note field for cost |
| `PouchOffsetTop_mm` | double | TwoWay | Pouch foil offset from separator (top) |
| `PouchOffsetBottom_mm` | double | TwoWay | Pouch foil offset from separator (bottom) |
| `PouchOffsetLeft_mm` | double | TwoWay | Pouch foil offset from separator (left) |
| `PouchOffsetRight_mm` | double | TwoWay | Pouch foil offset from separator (right) |
| `CalculatedHeight_mm` | double | TwoWay | Calculated cell height (read-only) |
| `CalculatedWidth_mm` | double | TwoWay | Calculated cell width (read-only) |
| `UseMeasuredDims` | bool | TwoWay | Toggle to use measured dimensions |
| `MeasuredHeight_mm` | double | TwoWay | Measured cell height (manual input) |
| `MeasuredWidth_mm` | double | TwoWay | Measured cell width (manual input) |

#### Packaging → Tabs Section
| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `TabPosHorizontal_mm` | double | TwoWay | Positive tab horizontal position |
| `TabPosVertical_mm` | double | TwoWay | Positive tab vertical position |
| `TabPosWidth_mm` | double | TwoWay | Positive tab width |
| `TabPosLength_mm` | double | TwoWay | Positive tab total length |
| `TabPosLengthOutside_mm` | double | TwoWay | Positive tab length outside case |
| `TabNegHorizontal_mm` | double | TwoWay | Negative tab horizontal position |
| `TabNegVertical_mm` | double | TwoWay | Negative tab vertical position |
| `TabNegWidth_mm` | double | TwoWay | Negative tab width |
| `TabNegLength_mm` | double | TwoWay | Negative tab total length |
| `TabNegLengthOutside_mm` | double | TwoWay | Negative tab length outside case |

---

### StackConfigurationViewModel Properties

#### Input Properties (Black Fields – Editable)
| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `NumberOfStacksInCell` | int | TwoWay | Number of stacks in the cell |
| `ElectrodePairsPerStack` | int | TwoWay | Electrode pairs per stack |
| `SeparatorOverwrapsPerStack` | int | TwoWay | Separator overwraps per stack |
| `AdditionalOverwrapsPerStack` | int | TwoWay | Additional overwraps per stack |
| `InsulationShellAroundAllStacks` | int | TwoWay | Insulation shell count |
| `FixingTapesPerStack` | int | TwoWay | Fixing tapes per stack |
| `FixingTapesOnAllStacks` | bool | TwoWay | Apply fixing tapes to all stacks |
| `EndElectrodes` | EndElectrodesMode | TwoWay | End electrodes mode (enum) |
| `EndCoatings` | EndCoatingsMode | TwoWay | End coatings mode (enum) |

#### Computed Properties (Gray Fields – Read-Only)
| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `ElectrodePairsInCell` | int | OneWay | Total electrode pairs (stacks × pairs) |
| `CathodeSheets_Stack` | int | OneWay | Cathode sheets per stack |
| `AnodeSheets_Stack` | int | OneWay | Anode sheets per stack |
| `AllElectrodeSheets_Stack` | int | OneWay | All electrode sheets per stack |
| `SeparatorSheets_Stack` | int | OneWay | Separator sheets per stack |
| `Overwrap_Stack` | int | OneWay | Overwrap count per stack |
| `InsulationShell_Stack` | int | OneWay | Insulation shell per stack (always 0) |
| `FixingTape_Stack` | int | OneWay | Fixing tape count per stack |
| `CathodeSheets_Cell` | int | OneWay | Cathode sheets in cell |
| `AnodeSheets_Cell` | int | OneWay | Anode sheets in cell |
| `AllElectrodeSheets_Cell` | int | OneWay | All electrode sheets in cell |
| `SeparatorSheets_Cell` | int | OneWay | Separator sheets in cell |
| `Overwrap_Cell` | int | OneWay | Overwrap count in cell |
| `InsulationShell_Cell` | int | OneWay | Insulation shell in cell |
| `FixingTape_Cell` | int | OneWay | Fixing tape count in cell |
| `SingleStackThickness_Dry` | double? | OneWay | Single stack thickness (dry) – null |
| `SingleStackThickness_SoC0` | double? | OneWay | Single stack thickness (SoC 0%) – null |
| `SingleStackThickness_SoC100` | double? | OneWay | Single stack thickness (SoC 100%) – null |
| `AllStacksThickness_Dry` | double? | OneWay | All stacks thickness (dry) – null |
| `AllStacksThickness_SoC0` | double? | OneWay | All stacks thickness (SoC 0%) – null |
| `AllStacksThickness_SoC100` | double? | OneWay | All stacks thickness (SoC 100%) – null |

---

### PackagingLayer Properties (Composition Row Model)

| Property | Type | Mode | Description |
|----------|------|------|-------------|
| `No` | int | Init | Row number |
| `Name` | string | Init | Layer name (e.g., "PA 15µm") |
| `Version` | string | Init | Version string (e.g., "1 – 01.10.25") |
| `Thickness_um` | double | Init | Thickness in micrometers |
| `Porosity_pct` | double | Init | Porosity percentage (0-100) |
| `Density_gcm3` | double | Init | Bulk density in g/cm³ |
| `EffectiveDensity_gcm3` | double | Computed | Porosity-corrected density |
| `ArealWeight_mgcm2` | double | Computed | Areal weight in mg/cm² |

**Formula:**
- `EffectiveDensity_gcm3 = Density_gcm3 × (1 - Porosity_pct/100)`
- `ArealWeight_mgcm2 = Thickness_um × 1e-4 × EffectiveDensity_gcm3 × 1000`

---

## Quick Reference Table

### All Named XAML Elements

| Name | Type | Purpose | Location (Line) |
|------|------|---------|-----------------|
| `TabSheetDesign` | RadioButton | Sheet Design tab selector | ~190 |
| `TabStackConfig` | RadioButton | Stack Config tab selector | ~191 |
| `TabPackaging` | RadioButton | Packaging tab selector | ~192 |
| `SubTabCase` | RadioButton | Case sub-tab selector | ~196 |
| `SubTabTabs` | RadioButton | Tabs sub-tab selector | ~197 |
| `SubTabAddFoils` | RadioButton | Add. Foils sub-tab selector | ~198 |
| `SubTabFixingTape` | RadioButton | Fixing Tape sub-tab selector | ~199 |
| `PanelSheetDesign` | Grid | Sheet Design content panel | ~219 |
| `PanelStackConfig` | Grid | Stack Configuration content panel | ~508 |
| `PanelPackaging` | Grid | Packaging content container | ~875 |
| `SubPanelCase` | Grid | Case sub-panel content | ~879 |
| `SubPanelTabs` | Grid | Tabs sub-panel content | ~1121 |
| `SubPanelAddFoils` | Grid | Add. Foils sub-panel content | ~1291 |
| `SubPanelFixingTape` | Grid | Fixing Tape sub-panel content | ~1306 |
| `FixingTapesOnAllStacksCheckBox` | CheckBox | Fixing tapes toggle | ~611 |
| `ViewportHost` | ContentControl | 3D viewport container | ~1364 |
| `SchematicHost` | Border | 2D schematic container | ~1391 |
| `CasePreviewCanvas` | Canvas | Case preview drawing surface | ~1439 |

---

## Style References

### TextBox Styles

| Style Name | Purpose | Background | Foreground | Usage |
|------------|---------|------------|------------|-------|
| `InputTextBox` | User-editable fields | Black | White (Bold) | Two-way bound properties |
| `CalcTextBox` | Calculated/read-only fields | #F5F5F5 (Gray) | Black | One-way bound computed properties |

### RadioButton Styles

| Style Name | Purpose | Active Color | Inactive Color |
|------------|---------|--------------|----------------|
| `MainTabStyle` | Main section tabs | #0078D4 (Blue) | #E8E8E8 (Gray) |
| `SubTabStyle` | Packaging sub-tabs | #0078D4 (Blue) | #F0F0F0 (Light Gray) |

### Converters

| Converter Name | Purpose | Usage |
|----------------|---------|-------|
| `BoolToVis` | Converts bool to Visibility | Tab panel visibility bindings |
| `EnumToBool` | Converts enum to bool for RadioButton | End electrodes/coatings bindings |
| `NullToDash` | Converts null to "—" | Null thickness values display |

---

## Code-Behind Event Handlers

### Button Click Handlers

| Handler Name | Bound To | Purpose |
|--------------|----------|---------|
| `Normalize_Click` | "Normalize" button | Calls `_vm.Normalize()` |
| `Fit_Click` | "Fit" button | Calls `_scene?.BuildPouchCell(_vm.Model)` |

### Rendering Methods

| Method Name | Canvas Target | Renderer Called | Trigger Events |
|-------------|---------------|-----------------|----------------|
| `RenderSheetViews()` | `_canvasFullLayout`, `_canvasCornerZoom` | `SheetRenderer.RenderFullLayout()`, `SheetRenderer.RenderCornerZoom()` | Loaded, SizeChanged, PropertyChanged |
| `RenderCasePreview()` | `_canvasCasePreview` | `SheetRenderer.DrawPackagingCase()` | Loaded, SizeChanged, PropertyChanged |

---

## Future Enhancements

### Placeholder Sections (Not Yet Implemented)

1. **Additional Foils Sub-Tab** (`SubPanelAddFoils`)
   - Currently empty StackPanel
   - Purpose: Configure additional protective foils

2. **Fixing Tape Sub-Tab** (`SubPanelFixingTape`)
   - Currently empty StackPanel
   - Purpose: Configure fixing tape placement

3. **Calculated Thickness** (Stack Configuration)
   - Properties exist but return null
   - Requires materials database integration

4. **Calculated Mass** (Tabs Sub-Panel)
   - Placeholder label showing "—"
   - Requires tab materials database

---

## Revision History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-01-13 | Initial comprehensive reference document |
| 1.1 | 2025-11-17 | Updated for .NET 10 upgrade |

---

**End of Document**
