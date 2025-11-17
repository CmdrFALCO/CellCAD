# CellCAD – Named Objects Reference

**Version:** 1.0
**Date:** 2025-11-17
**Application:** CellCAD Pouch Cell Designer
**File:** `PouchCellWindow.xaml` + `PouchCellWindow.xaml.cs`
**Framework:** WPF / .NET 10.0

---

## Overview

This document provides a complete reference of all named XAML elements and their relationships in the **CellCAD Pouch Cell Window**. Named elements are those with an `x:Name` attribute in XAML or created programmatically in the code-behind.

Named objects enable:
- Event handling and user interaction
- Visibility control via data binding
- Code-behind access for rendering and manipulation
- Tab/sub-tab navigation logic

---

## Table of Contents

1. [Navigation Structure](#navigation-structure)
2. [Main Tab Panels](#main-tab-panels)
3. [Packaging Sub-Tab Panels](#packaging-sub-tab-panels)
4. [Canvas Rendering Elements](#canvas-rendering-elements)
5. [Special UI Elements](#special-ui-elements)
6. [Element Relationships](#element-relationships)
7. [Code-Behind References](#code-behind-references)

---

## 1. Navigation Structure

### Main Tabs (RadioButtons)

These RadioButtons control which main panel is visible.

| Name | Content | Type | Group | Default | Line |
|------|---------|------|-------|---------|------|
| **TabSheetDesign** | "Sheet Design" | RadioButton | MainTabs | ✓ Checked | 190 |
| **TabStackConfig** | "Stack Config." | RadioButton | MainTabs | Unchecked | 191 |
| **TabPackaging** | "Packaging" | RadioButton | MainTabs | Unchecked | 192 |

**Relationship:**
- Each controls the visibility of its corresponding main panel via `ElementName` binding
- Only one can be checked at a time (RadioButton group behavior)

---

### Packaging Sub-Tabs (RadioButtons)

These RadioButtons control which Packaging sub-panel is visible. Only visible when `TabPackaging` is checked.

| Name | Content | Type | Group | Default | Line |
|------|---------|------|-------|---------|------|
| **SubTabCase** | "Case" | RadioButton | PackagingSubTabs | ✓ Checked | 196 |
| **SubTabTabs** | "Tabs" | RadioButton | PackagingSubTabs | Unchecked | 197 |
| **SubTabAddFoils** | "Add. Foils" | RadioButton | PackagingSubTabs | Unchecked | 198 |
| **SubTabFixingTape** | "Fixing Tape" | RadioButton | PackagingSubTabs | Unchecked | 199 |

**Relationship:**
- Nested within `PanelPackaging`
- Each controls the visibility of its corresponding sub-panel via `ElementName` binding
- Only one can be checked at a time (RadioButton group behavior)

---

## 2. Main Tab Panels

### Main Panel Grid Elements

These Grid elements represent the main content areas. Their visibility is controlled by the main tab RadioButtons.

| Name | Purpose | Tab | Visibility Binding | Line |
|------|---------|-----|-------------------|------|
| **PanelSheetDesign** | Sheet design inputs & 2D views | Sheet Design | `TabSheetDesign.IsChecked` | 219 |
| **PanelStackConfig** | Stack configuration & geometry | Stack Config. | `TabStackConfig.IsChecked` | 508 |
| **PanelPackaging** | Packaging materials & sub-tabs | Packaging | `TabPackaging.IsChecked` | 875 |

**Binding Pattern:**
```xml
Visibility="{Binding IsChecked, ElementName=TabSheetDesign, Converter={StaticResource BoolToVis}}"
```

---

## 3. Packaging Sub-Tab Panels

### Sub-Panel Grid Elements

These Grid elements represent sub-sections within the Packaging tab. Their visibility is controlled by the Packaging sub-tab RadioButtons.

| Name | Purpose | Sub-Tab | Visibility Binding | Line |
|------|---------|---------|-------------------|------|
| **SubPanelCase** | Pouch foil case material & properties | Case | `SubTabCase.IsChecked` | 879 |
| **SubPanelTabs** | Cathode/Anode tab materials & geometry | Tabs | `SubTabTabs.IsChecked` | 1121 |
| **SubPanelAddFoils** | Overwrap & Insulation Shell materials | Add. Foils | `SubTabAddFoils.IsChecked` | 1325 |
| **SubPanelInsulationShell** | (Legacy placeholder - unused) | N/A | `SubTabInsulationShell.IsChecked` | 1646 |
| **SubPanelFixingTape** | Fixing tape material & geometry | Fixing Tape | `SubTabFixingTape.IsChecked` | 1652 |

**Note:** `SubPanelInsulationShell` (line 1646) is a legacy element with no corresponding RadioButton. It contains only an empty StackPanel and is effectively unused.

**Binding Pattern:**
```xml
Visibility="{Binding IsChecked, ElementName=SubTabCase, Converter={StaticResource BoolToVis}}"
```

---

## 4. Canvas Rendering Elements

### XAML-Defined Canvas

| Name | Purpose | Location | Renderer | Bound To | Line |
|------|---------|----------|----------|----------|------|
| **CasePreviewCanvas** | 2D preview for Packaging → Case tab | Right column of `SubPanelCase` | `SheetRenderer.DrawPackagingCase()` | `_canvasCasePreview` | 1991 |

**Usage:**
- Renders separator and pouch foil outlines with offset labels
- Updated on property changes and size changes
- Wired in code-behind `Loaded` event

---

### Code-Behind Created Canvases

These canvases are **created programmatically** in `PouchCellWindow.xaml.cs` and attached to named host elements.

| Variable | Purpose | Host Element | Renderer | Created | Attached |
|----------|---------|--------------|----------|---------|----------|
| **_canvasFullLayout** | Full sheet layout (Cathode/Anode/Separator) | `ViewportHost` | `SheetRenderer.RenderFullLayout()` | Line 40 | Line 43 |
| **_canvasCornerZoom** | Zoomed corner offset view | `SchematicHost` | `SheetRenderer.RenderCornerZoom()` | Line 41 | Line 44 |
| **_canvasCasePreview** | Case preview (reference to XAML element) | `CasePreviewCanvas` | `SheetRenderer.DrawPackagingCase()` | N/A | Line 69 |

**Code-Behind Creation (lines 40-44):**
```csharp
_canvasFullLayout = new Canvas { Background = System.Windows.Media.Brushes.White };
_canvasCornerZoom = new Canvas { Background = System.Windows.Media.Brushes.White };

ViewportHost.Content = _canvasFullLayout;
SchematicHost.Child = _canvasCornerZoom;
```

---

## 5. Special UI Elements

### Host Elements for Canvases

| Name | Type | Purpose | Child | Parent | Line |
|------|------|---------|-------|--------|------|
| **ViewportHost** | ContentControl | Hosts full layout 2D canvas | `_canvasFullLayout` | Right column of `PanelSheetDesign` | 1916 |
| **SchematicHost** | Border | Hosts corner zoom 2D canvas | `_canvasCornerZoom` | Below ViewportHost | 1943 |

**Relationship:**
- Both are visible only when `TabSheetDesign` is checked
- Content set programmatically in constructor (lines 43-44)

---

### CheckBox Elements

| Name | Purpose | Binding | Parent | Line |
|------|---------|---------|--------|------|
| **FixingTapesOnAllStacksCheckBox** | Enable fixing tapes on all stacks | `_stackVm.FixingTapesOnAllStacks` | `PanelStackConfig` | 611 |

---

### Template-Internal Elements

These elements exist within ControlTemplates and are **not** directly accessible from code-behind.

| Name | Type | Location | Purpose | Line |
|------|------|----------|---------|------|
| **Border** (1) | Border | MainTabStyle template | Visual representation of main tab | 60 |
| **Border** (2) | Border | SubTabStyle template | Visual representation of sub-tab | 89 |

**Note:** Template-internal named elements are scoped to the template instance and cannot be accessed via `FindName()` from the Window level.

---

## 6. Element Relationships

### Navigation Hierarchy

```
Window
│
├─ MainTabs (RadioButton Group: "MainTabs")
│  ├─ TabSheetDesign (default checked)
│  ├─ TabStackConfig
│  └─ TabPackaging
│
├─ Main Panels (Grid, visibility controlled by MainTabs)
│  ├─ PanelSheetDesign (visible when TabSheetDesign.IsChecked)
│  ├─ PanelStackConfig (visible when TabStackConfig.IsChecked)
│  └─ PanelPackaging (visible when TabPackaging.IsChecked)
│     │
│     ├─ PackagingSubTabs (RadioButton Group: "PackagingSubTabs")
│     │  ├─ SubTabCase (default checked)
│     │  ├─ SubTabTabs
│     │  ├─ SubTabAddFoils
│     │  └─ SubTabFixingTape
│     │
│     └─ Sub-Panels (Grid, visibility controlled by PackagingSubTabs)
│        ├─ SubPanelCase (visible when SubTabCase.IsChecked)
│        ├─ SubPanelTabs (visible when SubTabTabs.IsChecked)
│        ├─ SubPanelAddFoils (visible when SubTabAddFoils.IsChecked)
│        ├─ SubPanelInsulationShell (unused legacy)
│        └─ SubPanelFixingTape (visible when SubTabFixingTape.IsChecked)
```

---

### Canvas/Host Relationships

```
PanelSheetDesign
│
├─ ViewportHost (ContentControl)
│  └─ _canvasFullLayout (Canvas, created in code)
│     └─ Renders: RenderFullLayout() → Cathode/Anode/Separator/Flags
│
└─ SchematicHost (Border)
   └─ _canvasCornerZoom (Canvas, created in code)
      └─ Renders: RenderCornerZoom() → Offset detail view

SubPanelCase
│
└─ CasePreviewCanvas (Canvas, XAML-defined)
   └─ _canvasCasePreview (reference variable)
      └─ Renders: DrawPackagingCase() → Separator + Pouch Foil
```

---

## 7. Code-Behind References

### Private Fields (PouchCellWindow.xaml.cs)

| Field | Type | Purpose | Initialized | Line |
|-------|------|---------|-------------|------|
| `_scene` | CadSceneHost? | 3D CAD viewport (future use) | Line 37 | 13 |
| `_vm` | PouchCellViewModel | Main ViewModel for data binding | Line 25 | 14 |
| `_stackVm` | StackConfigurationViewModel | Stack configuration ViewModel | Line 26 | 15 |
| `_canvasFullLayout` | Canvas? | Full sheet layout canvas | Line 40 | 16 |
| `_canvasCornerZoom` | Canvas? | Corner zoom canvas | Line 41 | 17 |
| `_canvasCasePreview` | Canvas? | Case preview canvas (ref) | Line 69 | 18 |

---

### Rendering Methods

| Method | Canvas | Renderer | Trigger |
|--------|--------|----------|---------|
| `RenderSheetViews()` | `_canvasFullLayout`, `_canvasCornerZoom` | `SheetRenderer.RenderFullLayout()`, `RenderCornerZoom()` | Loaded, SizeChanged, PropertyChanged |
| `RenderCasePreview()` | `_canvasCasePreview` | `SheetRenderer.DrawPackagingCase()` | Loaded, SizeChanged, PropertyChanged, Canvas.SizeChanged |

---

### Event Wiring

#### Loaded Event (lines 46-50)
```csharp
Loaded += (_, __) =>
{
    RenderSheetViews();
    RenderCasePreview();
};
```

#### SizeChanged Event (lines 52-56)
```csharp
SizeChanged += (_, __) =>
{
    RenderSheetViews();
    RenderCasePreview();
};
```

#### ViewModel PropertyChanged (lines 58-62)
```csharp
_vm.PropertyChanged += (_, __) =>
{
    RenderSheetViews();
    RenderCasePreview();
};
```

#### CasePreviewCanvas Wiring (lines 65-72)
```csharp
Loaded += (_, __) =>
{
    if (CasePreviewCanvas != null)
    {
        _canvasCasePreview = CasePreviewCanvas;
        CasePreviewCanvas.SizeChanged += (s, e) => RenderCasePreview();
    }
};
```

---

## 8. Visibility Control Pattern

All main panels and sub-panels use the same visibility binding pattern:

```xml
<Grid x:Name="PanelName"
      Visibility="{Binding IsChecked,
                           ElementName=CorrespondingRadioButton,
                           Converter={StaticResource BoolToVis}}">
```

The `BoolToVisibilityConverter` (referenced as `BoolToVis`) converts:
- `true` → `Visibility.Visible`
- `false` → `Visibility.Collapsed`

This ensures only one main panel and one sub-panel are visible at a time.

---

## 9. Data Context Binding

### Root DataContext

The Window's `DataContext` is set in the code-behind constructor:

```csharp
DataContext = _vm; // PouchCellViewModel
```

### Stack Configuration Binding

The Stack Configuration panel uses a dedicated ViewModel:

```xml
<Grid x:Name="PanelStackConfig" DataContext="{Binding ElementName=ThisWindow, Path=DataContext._stackVm}">
```

However, based on the XAML structure, `_stackVm` is bound via direct bindings to `_stackVm` properties in event handlers and initialization code.

---

## 10. Summary Table: All Named Elements

| Name | Type | Purpose | Location | Access |
|------|------|---------|----------|--------|
| **TabSheetDesign** | RadioButton | Main tab: Sheet Design | Top nav | XAML + Code |
| **TabStackConfig** | RadioButton | Main tab: Stack Config | Top nav | XAML + Code |
| **TabPackaging** | RadioButton | Main tab: Packaging | Top nav | XAML + Code |
| **SubTabCase** | RadioButton | Sub-tab: Case | Packaging nav | XAML + Code |
| **SubTabTabs** | RadioButton | Sub-tab: Tabs | Packaging nav | XAML + Code |
| **SubTabAddFoils** | RadioButton | Sub-tab: Add. Foils | Packaging nav | XAML + Code |
| **SubTabFixingTape** | RadioButton | Sub-tab: Fixing Tape | Packaging nav | XAML + Code |
| **PanelSheetDesign** | Grid | Sheet design content | Main content | XAML + Code |
| **PanelStackConfig** | Grid | Stack config content | Main content | XAML + Code |
| **PanelPackaging** | Grid | Packaging content | Main content | XAML + Code |
| **SubPanelCase** | Grid | Case sub-content | Packaging content | XAML + Code |
| **SubPanelTabs** | Grid | Tabs sub-content | Packaging content | XAML + Code |
| **SubPanelAddFoils** | Grid | Add. Foils sub-content | Packaging content | XAML + Code |
| **SubPanelInsulationShell** | Grid | (Unused legacy) | Packaging content | XAML + Code |
| **SubPanelFixingTape** | Grid | Fixing Tape sub-content | Packaging content | XAML + Code |
| **ViewportHost** | ContentControl | Canvas host for full layout | Sheet Design right | XAML + Code |
| **SchematicHost** | Border | Canvas host for corner zoom | Sheet Design right | XAML + Code |
| **CasePreviewCanvas** | Canvas | 2D case preview | Case sub-panel right | XAML + Code |
| **FixingTapesOnAllStacksCheckBox** | CheckBox | Fixing tapes toggle | Stack Config panel | XAML + Code |
| **_canvasFullLayout** | Canvas | Full layout renderer | (Created in code) | Code only |
| **_canvasCornerZoom** | Canvas | Corner zoom renderer | (Created in code) | Code only |
| **_canvasCasePreview** | Canvas | Case preview (ref) | (Reference to XAML) | Code only |

---

## 11. Cross-Reference: Named Elements by Function

### Navigation Control
- `TabSheetDesign`, `TabStackConfig`, `TabPackaging`
- `SubTabCase`, `SubTabTabs`, `SubTabAddFoils`, `SubTabFixingTape`

### Content Panels
- `PanelSheetDesign`, `PanelStackConfig`, `PanelPackaging`
- `SubPanelCase`, `SubPanelTabs`, `SubPanelAddFoils`, `SubPanelFixingTape`

### Rendering Infrastructure
- `ViewportHost`, `SchematicHost`, `CasePreviewCanvas`
- `_canvasFullLayout`, `_canvasCornerZoom`, `_canvasCasePreview`

### User Input
- `FixingTapesOnAllStacksCheckBox`

---

## 12. Usage Guidelines

### Accessing Named Elements in Code-Behind

```csharp
// Accessing XAML-defined elements by name
var tabIsChecked = TabSheetDesign.IsChecked;
var panelVisibility = PanelSheetDesign.Visibility;

// Accessing code-created canvases
if (_canvasFullLayout != null)
{
    SheetRenderer.RenderFullLayout(_canvasFullLayout, _vm);
}
```

### Adding New Sub-Tabs

To add a new Packaging sub-tab:

1. Add a new `RadioButton` in the `PackagingSubTabs` group with a unique `x:Name`
2. Add a new `Grid` with `x:Name="SubPanelYourName"` in `PanelPackaging`
3. Bind visibility: `Visibility="{Binding IsChecked, ElementName=YourSubTab, Converter={StaticResource BoolToVis}}"`
4. Add ViewModel properties and XAML content following existing patterns

### Best Practices

- **Unique Names**: All `x:Name` values must be unique across the entire XAML file
- **Visibility Pattern**: Use the `BoolToVis` converter pattern for tab/panel visibility
- **Code-Behind Access**: Only access named elements from code-behind when necessary (prefer MVVM data binding)
- **Canvas Rendering**: Always check for null before rendering to canvases
- **Event Cleanup**: Remove event handlers in `OnClosed()` to prevent memory leaks

---

## Appendix: Line Number Reference

Quick reference for locating named elements in the source files:

| Element | Line |
|---------|------|
| TabSheetDesign | 190 |
| TabStackConfig | 191 |
| TabPackaging | 192 |
| SubTabCase | 196 |
| SubTabTabs | 197 |
| SubTabAddFoils | 198 |
| SubTabFixingTape | 199 |
| PanelSheetDesign | 219 |
| PanelStackConfig | 508 |
| FixingTapesOnAllStacksCheckBox | 611 |
| PanelPackaging | 875 |
| SubPanelCase | 879 |
| SubPanelTabs | 1121 |
| SubPanelAddFoils | 1325 |
| SubPanelInsulationShell | 1646 |
| SubPanelFixingTape | 1652 |
| ViewportHost | 1916 |
| SchematicHost | 1943 |
| CasePreviewCanvas | 1991 |

---

**Document Version:** 1.0
**Last Updated:** 2025-11-17
**Maintained By:** CellCAD Development Team
