using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Godot.Roslyn
{
    static class ExtensionMethods
    {
        private class FirstScriptClassComparer : IEqualityComparer<INamedTypeSymbol>
        {
            private static readonly FirstScriptClassComparer Default = new();
            public static FirstScriptClassComparer For(GeneratorExecutionContext context)
            {
                lock (Default)
                    Default._ctx = context;

                return Default;
            }

            private GeneratorExecutionContext _ctx;

            public bool Equals(INamedTypeSymbol x, INamedTypeSymbol y)
            {
                if (x is null)
                    return y is null;
                if (x.Equals(y, SymbolEqualityComparer.Default))
                    return true;

                foreach (Location xLoc in x.Locations)
                    if (xLoc.IsInSource)
                        foreach (Location yLoc in y.Locations)
                            if (yLoc.IsInSource && xLoc.SourceTree!.FilePath == yLoc.SourceTree!.FilePath)
                            {
                                Reporter.ReportMultipleScriptClassesInFile(_ctx, xLoc.SourceTree!.FilePath);
                                return true;
                            }
                return false;
            }

            public int GetHashCode(INamedTypeSymbol obj)
            {
#pragma warning disable RS1024
                return obj?.GetHashCode() ?? 0;
#pragma warning restore RS1024
            }
        }
        public static INamedTypeSymbol[] GetGodotClasses(this GeneratorExecutionContext context)
            => context.Compilation.SyntaxTrees
                .SelectMany(tree =>
                    tree.GetRoot().DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .SelectGodotScriptClasses(context, context.Compilation)
                        // Report and skip non-partial classes
                        .Where(x =>
                        {
                            if (x.cds.IsPartial())
                            {
                                if (x.cds.IsNested() && !x.cds.AreAllOuterTypesPartial(out var typeMissingPartial))
                                {
                                    Reporter.ReportNonPartialGodotScriptOuterClass(context, typeMissingPartial!);
                                    return false;
                                }

                                if (x.symbol.Locations.Length > 1)
                                {
                                    Reporter.ReportMultipleScriptClassDeclarations(context, x.symbol);
                                    return false;
                                }

                                return true;
                            }

                            Reporter.ReportNonPartialGodotScriptClass(context, x.cds, x.symbol);
                            return false;
                        })
                        .Select(x => x.symbol)
                )
                .Distinct(FirstScriptClassComparer.For(context))
                .ToArray();
        public static bool TryGetGlobalAnalyzerProperty(
            this GeneratorExecutionContext context, string property, out string? value
        ) => context.AnalyzerConfigOptions.GlobalOptions
            .TryGetValue("build_property." + property, out value);

        public static bool AreGodotSourceGeneratorsDisabled(this GeneratorExecutionContext context)
            => context.TryGetGlobalAnalyzerProperty("GodotSourceGenerators", out string? toggle) &&
               toggle != null &&
               toggle.Equals("disabled", StringComparison.OrdinalIgnoreCase);

        public static bool IsGodotSourceGeneratorDisabled(this GeneratorExecutionContext context, string generatorName) =>
            AreGodotSourceGeneratorsDisabled(context) ||
            (context.TryGetGlobalAnalyzerProperty("GodotDisabledSourceGenerators", out string? disabledGenerators) &&
            disabledGenerators != null &&
            disabledGenerators.Split(';').Contains(generatorName));

        public static bool InheritsFrom(this ITypeSymbol? symbol, string typeFullName)
        {
            while (symbol != null)
            {
                if (symbol.FullQualifiedNameOmitGlobal() == typeFullName)
                {
                    return true;
                }

                symbol = symbol.BaseType;
            }

            return false;
        }

        public static bool InheritsFrom(this ITypeSymbol? symbol, string assemblyName, string typeFullName)
        {
            while (symbol != null)
            {
                if (symbol.ContainingAssembly?.Name == assemblyName &&
                    symbol.FullQualifiedNameOmitGlobal() == typeFullName)
                {
                    return true;
                }

                symbol = symbol.BaseType;
            }

            return false;
        }

        public static INamedTypeSymbol? GetGodotScriptNativeClass(this INamedTypeSymbol classTypeSymbol)
        {
            var symbol = classTypeSymbol;

            while (symbol != null)
            {
                if (symbol.ContainingAssembly?.Name == ClassNames.GodotAssembly)
                    return symbol;

                symbol = symbol.BaseType;
            }

            return null;
        }

        public static string? GetGodotScriptNativeClassName(this INamedTypeSymbol classTypeSymbol)
        {
            var nativeType = classTypeSymbol.GetGodotScriptNativeClass();

            if (nativeType == null)
                return null;

            var godotClassNameAttr = nativeType.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.IsGodotClassNameAttribute() ?? false);

            string? godotClassName = null;

            if (godotClassNameAttr is { ConstructorArguments: { Length: > 0 } })
                godotClassName = godotClassNameAttr.ConstructorArguments[0].Value?.ToString();

            return godotClassName ?? nativeType.Name;
        }

        private static bool TryGetGodotScriptClass(
            this ClassDeclarationSyntax cds, GeneratorExecutionContext context,
            Compilation compilation, out INamedTypeSymbol? symbol
        )
        {
            var sm = compilation.GetSemanticModel(cds.SyntaxTree);

            var classTypeSymbol = sm.GetDeclaredSymbol(cds);

            if (classTypeSymbol?.BaseType == null
                || !classTypeSymbol.BaseType.InheritsFrom(ClassNames.GodotAssembly, ClassNames.GodotObject))
            {
                symbol = null;
                return false;
            }

            symbol = classTypeSymbol;
            return true;
        }

        public static IEnumerable<(ClassDeclarationSyntax cds, INamedTypeSymbol symbol)> SelectGodotScriptClasses(
            this IEnumerable<ClassDeclarationSyntax> source,
            GeneratorExecutionContext context,
            Compilation compilation
        )
        {
            foreach (var cds in source)
            {
                if (cds.TryGetGodotScriptClass(context, compilation, out var symbol))
                    yield return (cds, symbol!);
            }
        }

        public static bool IsNested(this TypeDeclarationSyntax cds)
            => cds.Parent is TypeDeclarationSyntax;

        public static bool IsPartial(this TypeDeclarationSyntax cds)
            => cds.Modifiers.Any(SyntaxKind.PartialKeyword);

        public static bool AreAllOuterTypesPartial(
            this TypeDeclarationSyntax cds,
            out TypeDeclarationSyntax? typeMissingPartial
        )
        {
            SyntaxNode? outerSyntaxNode = cds.Parent;

            while (outerSyntaxNode is TypeDeclarationSyntax outerTypeDeclSyntax)
            {
                if (!outerTypeDeclSyntax.IsPartial())
                {
                    typeMissingPartial = outerTypeDeclSyntax;
                    return false;
                }

                outerSyntaxNode = outerSyntaxNode.Parent;
            }

            typeMissingPartial = null;
            return true;
        }

        public static string GetDeclarationKeyword(this INamedTypeSymbol namedTypeSymbol)
        {
            string? keyword = namedTypeSymbol.DeclaringSyntaxReferences
                .OfType<TypeDeclarationSyntax>().FirstOrDefault()?
                .Keyword.Text;

            return keyword ?? namedTypeSymbol.TypeKind switch
            {
                TypeKind.Interface => "interface",
                TypeKind.Struct => "struct",
                _ => "class"
            };
        }

        public static string NameWithTypeParameters(this INamedTypeSymbol symbol)
        {
            return symbol.IsGenericType ?
                string.Concat(symbol.Name, "<", string.Join(", ", symbol.TypeParameters), ">") :
                symbol.Name;
        }

        private static SymbolDisplayFormat FullyQualifiedFormatOmitGlobal { get; } =
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        private static SymbolDisplayFormat FullyQualifiedFormatIncludeGlobal { get; } =
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included);

        public static string FullQualifiedNameOmitGlobal(this ITypeSymbol symbol)
            => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatOmitGlobal);

        public static string FullQualifiedNameOmitGlobal(this INamespaceSymbol namespaceSymbol)
            => namespaceSymbol.ToDisplayString(FullyQualifiedFormatOmitGlobal);

        public static string FullQualifiedNameIncludeGlobal(this ITypeSymbol symbol)
            => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatIncludeGlobal);

        public static string FullQualifiedNameIncludeGlobal(this INamespaceSymbol namespaceSymbol)
            => namespaceSymbol.ToDisplayString(FullyQualifiedFormatIncludeGlobal);

        public static string AssemblyQualifiedNameIncludeGlobal(this ITypeSymbol symbol)
            => symbol.ContainingAssembly.Name;

        public static string FullQualifiedSyntax(this SyntaxNode node, SemanticModel sm)
        {
            StringBuilder sb = new();
            FullQualifiedSyntax(node, sm, sb, true);
            return sb.ToString();
        }

        private static void FullQualifiedSyntax(SyntaxNode node, SemanticModel sm, StringBuilder sb, bool isFirstNode)
        {
            if (node is NameSyntax ns && isFirstNode)
            {
                SymbolInfo nameInfo = sm.GetSymbolInfo(ns);
                sb.Append(nameInfo.Symbol?.ToDisplayString(FullyQualifiedFormatIncludeGlobal) ?? ns.ToString());
                return;
            }

            bool innerIsFirstNode = true;
            foreach (var child in node.ChildNodesAndTokens())
            {
                if (child.HasLeadingTrivia)
                {
                    sb.Append(child.GetLeadingTrivia());
                }

                if (child.IsNode)
                {
                    FullQualifiedSyntax(child.AsNode()!, sm, sb, isFirstNode: innerIsFirstNode);
                    innerIsFirstNode = false;
                }
                else
                {
                    sb.Append(child);
                }

                if (child.HasTrailingTrivia)
                {
                    sb.Append(child.GetTrailingTrivia());
                }
            }
        }

        public static string SanitizeQualifiedNameForUniqueHint(this string qualifiedName)
            => qualifiedName
                // AddSource() doesn't support angle brackets
                .Replace("<", "(Of ")
                .Replace(">", ")");

        public static string GodotVariantType(this ITypeSymbol symbol)
        {
            switch (symbol.FullQualifiedNameOmitGlobal())
            {
                case "bool":
                    return "Bool";
                case "sbyte":
                case "short":
                case "int":
                case "long":
                case "System.Int128":
                case "byte":
                case "ushort":
                case "uint":
                case "ulong":
                case "System.UInt128":
                    return "Int";
                case "System.Half":
                case "float":
                case "double":
                    return "Float";
                case "string":
                    return "String";

                case "Godot.Vector2":
                    return "Vector2";
                case "Godot.Vector3":
                    return "Vector3";
                case "Godot.Vector4":
                    return "Vector4";
                case "Godot.Vector2I":
                    return "Vector2I";
                case "Godot.Vector3I":
                    return "Vector3I";
                case "Godot.Vector4I":
                    return "Vector4I";

                case "Godot.Rect2":
                    return "Rect2";
                case "Godot.Rect2I":
                    return "Rect2I";

                case "Godot.Aabb":
                    return "AABB";

                case "Godot.Plane":
                    return "Plane";
                case "Godot.Quaternion":
                    return "Quaternion";

                case "Godot.Basis":
                    return "Basis";
                case "Godot.Transform2D":
                    return "Transform2D";
                case "Godot.Transform3D":
                    return "Transform3D";
                case "Godot.Projection":
                    return "Projection";

                case "Godot.Color":
                    return "Color";
                case "Godot.StringName":
                    return "StringName";
                case "Godot.NodePath":
                    return "NodePath";
                case "Godot.Rid":
                    return "Rid";
                case "Godot.Callable":
                    return "Callable";
                case "Godot.Signal":
                    return "Signal";
                case "Godot.VariantDictionary":
                    return "Dictionary";
                case "Godot.VariantArray":
                    return "Array";

                default:
                    if (symbol.IsGodotObject())
                        return "Object";
                    break;
            }

            return "Nil";
        }

        public static string GodotNativeType(this ITypeSymbol symbol, out bool modify)
        {
            switch (symbol.FullQualifiedNameOmitGlobal())
            {
                case "System.Boolean":
                    modify = true;
                    return "byte";

                case "System.String":
                    modify = true;
                    return "nint";

                case "Godot.VariantDictionary":
                    modify = true;
                    return "Dictionary";
                case "Godot.VariantArray":
                    modify = true;
                    return "Array";

                default:
                    if (symbol.IsGodotObject())
                    {
                        modify = true;
                        return "nint";
                    }

                    modify = false;
                    return symbol.FullQualifiedNameIncludeGlobal();
            }
        }

        public static string InternalMethodName(this IMethodSymbol method)
        {
            while (method.IsOverride && !method.HasGodotExposeAsAttribute() && method.OverriddenMethod!.ContainingAssembly.Name == ClassNames.GodotAssembly)
                method = method.OverriddenMethod!;
            return (method.GetGodotExposeAsAttribute(out AttributeData? exposed)
                && exposed!.NamedArguments.Get("Name").As(out string mname)) ? mname : method.Name;
        }

        public static bool IsGodotObject(this ITypeSymbol symbol)
            => symbol.InheritsFrom(ClassNames.GodotAssembly, ClassNames.GodotObject);

        public static bool GetGodotExposeAsAttribute(this ISymbol symbol, out AttributeData? data)
            => (data = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.IsGodotExposeAsAttribute() ?? false)) != null;
        public static bool HasGodotExposeAsAttribute(this ISymbol symbol)
            => symbol.GetAttributes().Any(attr => attr.AttributeClass?.IsGodotExposeAsAttribute() ?? false);
        public static bool IsGodotExposeAsAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.ExposeAsAttr;

        public static bool IsBuiltinNumber(this ITypeSymbol symbol)
            => symbol.SpecialType switch { 
                SpecialType.System_SByte => true,
                SpecialType.System_Byte => true,
                SpecialType.System_Int16 => true,
                SpecialType.System_Int32 => true,
                SpecialType.System_Int64 => true,
                SpecialType.System_UInt16 => true,
                SpecialType.System_UInt32 => true,
                SpecialType.System_UInt64 => true,
                SpecialType.System_Single => true,
                SpecialType.System_Double => true,
                _ => false,
            };

        public static bool IsAnyGodotExportAttribute(this INamedTypeSymbol symbol)
        {
            string name = symbol.FullQualifiedNameOmitGlobal();
            return name.Equals(ClassNames.ExportAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportCategoryAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGroupAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportSubgroupAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportColorNoAlphaAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportDirAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportEnumAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportExpEasingAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportFileAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportFlagsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGlobalDirAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGlobalFileAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DNavigationAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DPhysicsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DRenderAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DNavigationAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DPhysicsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DRenderAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayersAvoidanceAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportMultilineAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportNodePathAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportPlaceholderAttr, StringComparison.Ordinal) ||
                name.StartsWith(ClassNames.ExportRangeAttr, StringComparison.Ordinal);
        }

        public static bool IsSomeGodotExportAttribute(this INamedTypeSymbol symbol)
        {
            string name = symbol.FullQualifiedNameOmitGlobal();
            return name.Equals(ClassNames.ExportAttr, StringComparison.Ordinal) ||
                // name.Equals(ClassNames.ExportCategoryAttr, StringComparison.Ordinal) ||
                // name.Equals(ClassNames.ExportGroupAttr, StringComparison.Ordinal) ||
                // name.Equals(ClassNames.ExportSubgroupAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportColorNoAlphaAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportDirAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportEnumAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportExpEasingAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportFileAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportFlagsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGlobalDirAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGlobalFileAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DNavigationAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DPhysicsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers2DRenderAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DNavigationAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DPhysicsAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayers3DRenderAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportLayersAvoidanceAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportMultilineAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportNodePathAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportPlaceholderAttr, StringComparison.Ordinal) ||
                name.StartsWith(ClassNames.ExportRangeAttr, StringComparison.Ordinal);
        }

        public static bool IsGodotExportSectionAttribute(this INamedTypeSymbol symbol)
        {
            string name = symbol.FullQualifiedNameOmitGlobal();
            return name.Equals(ClassNames.ExportCategoryAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportGroupAttr, StringComparison.Ordinal) ||
                name.Equals(ClassNames.ExportSubgroupAttr, StringComparison.Ordinal);
        }

        public static PropertyHint GetPropertyHint(this AttributeData data)
        {
            string name = data.AttributeClass!.FullQualifiedNameOmitGlobal();
            return name switch
            {
                ClassNames.ExportAttr => (PropertyHint)PropertyExportExt.Export, // Resolve property hint elsewhere
                ClassNames.ExportCategoryAttr => (PropertyHint)PropertyExportExt.Category,
                ClassNames.ExportGroupAttr => (PropertyHint)PropertyExportExt.Group,
                ClassNames.ExportSubgroupAttr => (PropertyHint)PropertyExportExt.Subgroup,
                ClassNames.ExportColorNoAlphaAttr => PropertyHint.ColorNoAlpha,
                ClassNames.ExportDirAttr => PropertyHint.Dir,
                ClassNames.ExportEnumAttr => PropertyHint.Enum,
                ClassNames.ExportExpEasingAttr => PropertyHint.ExpEasing,
                ClassNames.ExportFileAttr => PropertyHint.File,
                ClassNames.ExportFlagsAttr => PropertyHint.Flags,
                ClassNames.ExportGlobalDirAttr => PropertyHint.GlobalDir,
                ClassNames.ExportGlobalFileAttr => PropertyHint.GlobalFile,
                ClassNames.ExportLayers2DNavigationAttr => PropertyHint.Layers2dNavigation,
                ClassNames.ExportLayers2DPhysicsAttr => PropertyHint.Layers2dPhysics,
                ClassNames.ExportLayers2DRenderAttr => PropertyHint.Layers2dRender,
                ClassNames.ExportLayers3DNavigationAttr => PropertyHint.Layers3dNavigation,
                ClassNames.ExportLayers3DPhysicsAttr => PropertyHint.Layers3dPhysics,
                ClassNames.ExportLayers3DRenderAttr => PropertyHint.Layers3dRender,
                ClassNames.ExportLayersAvoidanceAttr => PropertyHint.LayersAvoidance,
                ClassNames.ExportMultilineAttr => PropertyHint.MultilineText,
                ClassNames.ExportNodePathAttr => PropertyHint.NodePathValidTypes,
                ClassNames.ExportPlaceholderAttr => PropertyHint.PlaceholderText,
                _ => name.StartsWith(ClassNames.ExportRangeAttr) ? PropertyHint.Range : PropertyHint.None,
            };
        }

        public static bool IsGodotSignalAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.SignalAttr;

        public static bool IsGodotMustBeVariantAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.MustBeVariantAttr;

        public static bool IsGodotClassNameAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.GodotClassNameAttr;

        public static bool IsGodotGlobalClassAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.GlobalClassAttr;

        public static bool IsSystemFlagsAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.SystemFlagsAttr;

        public static bool IsGodotAssembly(this IAssemblySymbol symbol)
            => symbol.Identity.Name == ClassNames.GodotAssembly;

        public static string Path(this Location location)
            => location.SourceTree?.GetLineSpan(location.SourceSpan).Path
               ?? location.GetLineSpan().Path;

        public static int StartLine(this Location location)
            => location.SourceTree?.GetLineSpan(location.SourceSpan).StartLinePosition.Line
               ?? location.GetLineSpan().StartLinePosition.Line;

        public static bool IsInheritableAttribute(this INamedTypeSymbol attrType)
        {
            ImmutableArray<AttributeData> attr = attrType.GetAttributes();
            foreach (AttributeData a in attr)
            {
                if (a.AttributeClass?.FullQualifiedNameOmitGlobal() == typeof(AttributeUsageAttribute).FullName)
                    return a.NamedArguments.Get("Inherited").As(out bool isInherited) && isInherited;
            }
            return false;
        }

        public static bool HasAttribute(this ITypeSymbol type, string attribute)
        {
            ImmutableArray<AttributeData> attr = type.GetAttributes();
            foreach (AttributeData a in attr)
            {
                if (a.AttributeClass?.FullQualifiedNameOmitGlobal() == attribute)
                    return true;
            }
            return false;
        }
        public static bool IsGlobalType(this ITypeSymbol type)
            => type.HasAttribute(ClassNames.GlobalClassAttr);
        public static bool IsCompatibleType(this ITypeSymbol type)
            => type.IsReferenceType switch
            {
                true => type.IsGlobalType() || type.IsUnmanagedType,
                false => type.ContainingAssembly.IsGodotAssembly(),
            };

        public static bool IsCompatibleSignature(this IParameterSymbol param)
        {
            return param.Type is not INamedTypeSymbol type || type.IsUnboundGenericType;
        }
        public static bool TryGetCompatibleSignature(this IMethodSymbol method, out GodotMethodInfo info)
        {
            info = default;
            if (method.IsGenericMethod)
                return false;

            if (!method.ReturnsVoid && (method.ReturnsByRef || method.ReturnsByRefReadonly))
                return false;

            var parameters = method.Parameters;
            foreach (var p in parameters)
                if (!p.IsCompatibleSignature())
                    return false;

            info = new GodotMethodInfo() { Method = method, Params = method.Parameters, HasReturn = method.ReturnsVoid, Return = method.ReturnType };
            return true;
        }

        public static IEnumerable<GodotMethodInfo> WhereCompatibleSignature(this IEnumerable<IMethodSymbol> methods)
        {
            foreach (var method in methods)
            {
                if (TryGetCompatibleSignature(method, out GodotMethodInfo info))
                    yield return info;
            }
        }

        public static TValue Get<TKey, TValue>(this ImmutableArray<KeyValuePair<TKey, TValue>> arr, TKey key) where TKey : IEquatable<TKey>
        {
            for (int i = 0; i < arr.Length; ++i)
                if (key.Equals(arr[i].Key))
                    return arr[i].Value;
            return default!;
        }

        public static bool As<T>(this TypedConstant constant, out T value)
        {
            if (constant.Value is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        public static bool As<T>(this TypedConstant constant, out ImmutableArray<T> value)
        {
            value = constant.Values.Select(tc => { tc.As(out T value); return value; }).ToImmutableArray();
            return !value.IsDefaultOrEmpty;
        }
    }
}
