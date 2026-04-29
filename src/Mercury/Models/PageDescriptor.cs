using System;
using IconPacks.Avalonia.MaterialDesign;

namespace Mercury.Models;

public record PageDescriptor(
    string Title,
    PackIconMaterialDesignKind Icon,
    Type ViewType,
    Type ViewModelType,
    bool IsTab = false,
    bool IsDetail = false
);