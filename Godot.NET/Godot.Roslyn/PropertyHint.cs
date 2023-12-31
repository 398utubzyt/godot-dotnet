﻿namespace Godot
{
    public enum PropertyHint : uint
    {
        None = 0,
        Range = 1,
        Enum = 2,
        EnumSuggestion = 3,
        ExpEasing = 4,
        Link = 5,
        Flags = 6,
        Layers2dRender = 7,
        Layers2dPhysics = 8,
        Layers2dNavigation = 9,
        Layers3dRender = 10,
        Layers3dPhysics = 11,
        Layers3dNavigation = 12,
        LayersAvoidance = 37,
        File = 13,
        Dir = 14,
        GlobalFile = 15,
        GlobalDir = 16,
        ResourceType = 17,
        MultilineText = 18,
        Expression = 19,
        PlaceholderText = 20,
        ColorNoAlpha = 21,
        ObjectId = 22,
        TypeString = 23,
        NodePathToEditedNode = 24,
        ObjectTooBig = 25,
        NodePathValidTypes = 26,
        SaveFile = 27,
        GlobalSaveFile = 28,
        IntIsObjectid = 29,
        IntIsPointer = 30,
        ArrayType = 31,
        LocaleId = 32,
        LocalizableString = 33,
        NodeType = 34,
        HideQuaternionEdit = 35,
        Password = 36,
        Max = 38,
    }
}
