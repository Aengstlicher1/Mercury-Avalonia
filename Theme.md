# Mercury.Avalonia — Theme Authoring Guide

This document describes how themes are structured in Mercury.Avalonia, how to
create a new one, and which style classes a theme is allowed (and expected) to
contribute. It applies to Avalonia 12 with Semi.Avalonia as the base theme.

---

## 1. Overview

- Themes are a collection of Avalonia Styles and ResourceDictionarys.
- They are used to switch how Mercury looks to adjust it to your liking.

---

## 2. File Layout

A theme is composed of two kinds of files:

```
src/Mercury/
├── theme.json                        # 
├── Light.axaml                       # Light variant palette (ResourceDictionary)
├── Dark.axaml                        # Dark variant palette (ResourceDictionary)
├── Resources/
│   └── WindowDrawnDecorations.axaml  # General resources (ControlTemplates, Cornerradius etc.)
└── Styles/
    ├── Button.axaml                  # Per-control style file (Styles root)
    ├── TextBlock.axaml
    └── MaterialDesignIcon.axaml
```

- **`Colors *.axaml`** — `ResourceDictionary` files containing
  `Color` and `SolidColorBrush` entries. Registered as theme dictionaries in
  `App.axaml` under `ResourceDictionary.ThemeDictionaries`.
  Nothing stops you from putting these under Resources if you want to, but you have
  to put the path in the 'theme.json'.
- **`Styles/*.axaml`** — `Styles` root files containing `Style` selectors.

---

## 3. Required Color & Brush Resources

Every palette file **must** define the following keys. Both a `Color` and a
matching `SolidColorBrush` (suffix `Brush`) are expected.

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
We advise you to just use the Color as the Brush:
```xml

<SolidColorBrush x:Key="SurfaceBrush" Color="{StaticResource SurfaceColor}"/>

```

### 3.1 Authoring rules

- If your theme does not support Light or Dark mode, you should just leave the path to the
  resources empty in the theme.json.
- Always reference colors via `{DynamicResource …Brush}` from styles and
  views, never `{StaticResource}` — this enables runtime theme switching.
- Use translucent (`#AARRGGBB`) values for overlay-style brushes
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
| `Button.Link`         | text-only link button (no padding, accent foreground, underline on hover) | Implemented |
| `Button.Accent`       | filled accent button using `SelectedBrush`               | Planned |
| `Button.Icon`         | round icon-only button (used in playback bar)            | Planned |
| `Button.Ghost`        | borderless button with hover-only background             | Planned |

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


### 4.2 Style authoring rules

- Always use `{DynamicResource}` so the style follows variant changes.
- Use the `Selector="Button.Link"` syntax for class-based variants.
- Combine state pseudo-classes with the class selector, e.g.
  `Selector="Button.Link:pointerover"`.
- Never inline colors in styles — always pull from the palette keys
  defined in §3.
- Keep `Design.PreviewWith` in every style file so the Avalonia
  previewer renders the control with a representative example.
- Do not define `ControlTemplate` overrides unless absolutely necessary.

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

1. [ ] Create `Light.axaml` and `Dark.axaml` with **all** keys from §3.
2. [ ] (Optional) Add new style files under `Styles/` for control overrides.
3. [ ] Verify with the Avalonia previewer (`Design.PreviewWith`).
4. [ ] Smoke-test runtime switching on Windows, macOS, and Linux.

---

## 8. Conventions & Anti-patterns

- ✅ Use `DynamicResource` for every theme-driven brush.
- ✅ Keep one control type per style file.
- ✅ Express variants via `Classes="…"`, not via inline properties.
- ❌ Don't hard-code colors in views or ViewModels.

---

## 9. Future Work

- Accent color picker — derive `SelectedBrush` from a single user color?
  This could also be integrated into Windows settings Color.
- High-contrast variant for accessibility.
- Per-platform overrides (e.g. macOS vibrancy on `WindowSurfaceBrush`).
