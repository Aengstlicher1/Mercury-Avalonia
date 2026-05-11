# Mercury.Avalonia — Theme Authoring Guide

> Skeleton — fill in the prose under each heading as the theming system evolves.

This document describes how themes are structured in Mercury.Avalonia, how to
create a new one, and which style classes a theme is allowed (and expected) to
contribute. It applies to Avalonia 12 with Semi.Avalonia as the base theme.

---

## 1. Overview

- TODO: Short summary of what a "theme" is in Mercury (a color palette + an
  optional set of control style overrides layered on top of `SemiTheme`).
- TODO: Distinguish between **theme variants** (Light / Dark, swapped via
  `RequestedThemeVariant`) and **full themes** (a complete replacement of the
  palette and/or style set).
- TODO: Note that the Core library (`Mercury.Core`) must never be referenced
  from theme files — themes live exclusively in the Avalonia project.

---

## 2. File Layout

A theme is composed of two kinds of files:

```
src/Mercury/
├── Resources/
│   ├── Colors Light.axaml        # Light variant palette (ResourceDictionary)
│   ├── Colors Dark.axaml         # Dark variant palette (ResourceDictionary)
│   └── WindowDrawnDecorations.axaml
└── Styles/
    ├── Button.axaml              # Per-control style file (Styles root)
    ├── TextBlock.axaml
    └── MaterialDesignIcon.axaml
```

- **`Resources/Colors *.axaml`** — `ResourceDictionary` files containing
  `Color` and `SolidColorBrush` entries. Registered as theme dictionaries in
  `App.axaml` under `ResourceDictionary.ThemeDictionaries`.
- **`Styles/*.axaml`** — `Styles` root files containing `Style` selectors.
  Registered in `App.axaml` via `<StyleInclude Source="…" />`.

> TODO: Document naming conventions for new theme bundles (e.g. a future
> `Themes/Mercury/` folder if multiple full themes are added).

---

## 3. Required Color & Brush Resources

Every palette file **must** define the following keys. Both a `Color` and a
matching `SolidColorBrush` (suffix `Brush`) are expected so consumers can pick
whichever they need.

| Key                    | Purpose                                                   |
| ---------------------- | --------------------------------------------------------- |
| `WindowSurfaceColor`   | Window chrome / outer background                          |
| `SurfaceColor`         | Primary content surface                                   |
| `SubSurfaceColor`      | Secondary / translucent surface (overlays, popovers)      |
| `SurfaceItemColor`     | Cards, list items, elevated tiles                         |
| `OutlineColor`         | Borders, dividers, separators                             |
| `HoverColor`           | Pointer-over overlay (typically translucent)              |
| `SelectedColor`        | Accent / selection / focus highlight                      |
| `TextColor`            | Primary foreground                                        |
| `SubTextColor`         | Secondary / muted foreground                              |

Brush counterparts: `WindowSurfaceBrush`, `SurfaceBrush`, `SubSurfaceBrush`,
`SurfaceItemBrush`, `OutlineBrush`, `HoverBrush`, `SelectedBrush`, `TextBrush`,
`SubTextBrush`.

> TODO: Add a "Reserved for future" subsection (e.g. `DangerBrush`,
> `WarningBrush`, `SuccessBrush`, `DisabledTextBrush`) once the palette grows.

### 3.1 Authoring rules

- TODO: All resources must be declared in **both** `Colors Light.axaml` and
  `Colors Dark.axaml` — variants are swapped at runtime, missing keys cause
  binding failures.
- TODO: Always reference colors via `{DynamicResource …Brush}` from styles and
  views, never `{StaticResource}` — this enables runtime theme switching.
- TODO: Use translucent (`#AARRGGBB`) values for overlay-style brushes
  (`HoverBrush`, `SubSurfaceBrush`) so they compose correctly on any base.

---

## 4. Style Files (`Styles/*.axaml`)

Each style file is a `<Styles>` root containing one or more `<Style>` selectors
that target a single control type. Mercury currently ships:

- `Button.axaml`
- `TextBlock.axaml`
- `MaterialDesignIcon.axaml`

> TODO: Add new files here (`Slider.axaml`, `ListBox.axaml`, `TabView.axaml`, …)
> as theming coverage grows. One file per primary control type is the rule.

### 4.1 Style class catalog

The following style classes are part of the Mercury theme contract. A new
theme **may** override or extend them, but **must** keep the same class names
so that views continue to work unchanged.

#### `Button`

| Selector              | Meaning                                                  | Status |
| --------------------- | -------------------------------------------------------- | ------ |
| `Button`              | Default button — transparent bg, hover overlay, 8px radius | Implemented |
| `Button:pointerover`  | Hover state using `HoverBrush`                           | Implemented |
| `Button.Link`         | TODO — text-only link button (no padding, accent foreground, underline on hover) | Planned |
| `Button.Accent`       | TODO — filled accent button using `SelectedBrush`        | Planned |
| `Button.Icon`         | TODO — square icon-only button (used in playback bar)    | Planned |
| `Button.Ghost`        | TODO — borderless button with hover-only background      | Planned |
| `Button.Toolbar`      | TODO — compact button for toolbars / headers             | Planned |

#### `TextBlock`

| Selector              | Meaning                                       | Status |
| --------------------- | --------------------------------------------- | ------ |
| `TextBlock`           | Default foreground = `TextBrush`              | Implemented |
| `TextBlock.Title`     | TODO — large page / header text               | Planned |
| `TextBlock.Subtitle`  | TODO — secondary header text                  | Planned |
| `TextBlock.Caption`   | TODO — small muted text using `SubTextBrush`  | Planned |
| `TextBlock.Mono`      | TODO — monospace (JetBrains Mono / Fira Code) | Planned |

#### `PackIconMaterialDesign` (icon style)

| Selector                       | Meaning                                            | Status |
| ------------------------------ | -------------------------------------------------- | ------ |
| `icon|PackIconMaterialDesign`  | Default — `TextBrush`, centered                    | Implemented |
| `…​.Accent`                    | TODO — accent-colored icon (`SelectedBrush`)       | Planned |
| `…​.Muted`                     | TODO — muted icon (`SubTextBrush`)                 | Planned |

> TODO: Repeat this table layout for any additional controls a theme touches
> (e.g. `Slider`, `ToggleButton`, `ListBoxItem`, `TabItem`, `ScrollBar`).

### 4.2 Style authoring rules

- TODO: Always use `{DynamicResource}` so the style follows variant changes.
- TODO: Use the `Selector="Button.Link"` syntax for class-based variants;
  consumers opt in via `<Button Classes="Link" />`.
- TODO: Combine state pseudo-classes with the class selector, e.g.
  `Selector="Button.Link:pointerover"`.
- TODO: Never inline colors in styles — always pull from the palette keys
  defined in §3.
- TODO: Keep `Design.PreviewWith` in every style file so the Avalonia
  previewer renders the control with a representative example.
- TODO: Do not define `ControlTemplate` overrides unless absolutely necessary;
  prefer setters that delegate to Semi.Avalonia's templates.

---

## 5. Wiring a Theme into `App.axaml`

Themes are registered in two places inside `App.axaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary>
                <ResourceDictionary.ThemeDictionaries>
                    <ResourceInclude x:Key="Light" Source="Resources/Colors Light.axaml"/>
                    <ResourceInclude x:Key="Dark"  Source="Resources/Colors Dark.axaml"/>
                </ResourceDictionary.ThemeDictionaries>
            </ResourceDictionary>
            <!-- Window chrome resources -->
            <ResourceInclude Source="Resources/WindowDrawnDecorations.axaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>

<Application.Styles>
    <semi:SemiTheme />
    <StyleInclude Source="avares://IconPacks.Avalonia.MaterialDesign/MaterialDesign.axaml" />

    <StyleInclude Source="Styles/Button.axaml"/>
    <StyleInclude Source="Styles/TextBlock.axaml"/>
    <StyleInclude Source="Styles/MaterialDesignIcon.axaml"/>
</Application.Styles>
```

- TODO: Document the order constraint — `SemiTheme` must come first so
  Mercury's styles override it.
- TODO: Document how to add a third variant or a fully custom theme bundle.

---

## 6. Runtime Theme Switching

- TODO: Describe how to flip variants at runtime via
  `Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;`.
- TODO: Note where the user-facing setting will live (Settings page / service)
  and how it persists across launches.
- TODO: List things known **not** to update when the variant changes (and how
  to fix them — usually by replacing `StaticResource` with `DynamicResource`).

---

## 7. Adding a New Theme — Checklist

1. [ ] Create `Resources/Colors <Name>.axaml` with **all** keys from §3.
2. [ ] (Optional) Add new style files under `Styles/` for control overrides.
3. [ ] Register palette in `App.axaml` `ThemeDictionaries`.
4. [ ] Register any new style files in `Application.Styles`.
5. [ ] Verify with the Avalonia previewer (`Design.PreviewWith`).
6. [ ] Smoke-test runtime switching on Windows, macOS, and Linux.
7. [ ] Update this document's tables with any new style classes.

---

## 8. Conventions & Anti-patterns

- TODO: ✅ Use `DynamicResource` for every theme-driven brush.
- TODO: ✅ Keep one control type per style file.
- TODO: ✅ Express variants via `Classes="…"`, not via inline properties.
- TODO: ❌ Don't hard-code colors in views or ViewModels.
- TODO: ❌ Don't introduce theme code into `Mercury.Core` — themes are
  Avalonia-only.
- TODO: ❌ Don't replace Semi.Avalonia control templates wholesale; extend.

---

## 9. Open Questions / Future Work

- TODO: Should themes be discoverable as plug-ins (e.g. dropped into a
  `Themes/` folder and listed in Settings)?
- TODO: Accent color picker — derive `SelectedBrush` from a single user color?
- TODO: High-contrast variant for accessibility.
- TODO: Per-platform overrides (e.g. macOS vibrancy on `WindowSurfaceBrush`).
