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
        public static INamedTypeSymbol[] GetGodotClasses(this GeneratorExecutionContext context)
            => context.Compilation.SyntaxTrees
                .SelectMany(tree =>
                    tree.GetRoot().DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .SelectGodotScriptClasses(context.Compilation)
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

                                return true;
                            }

                            Reporter.ReportNonPartialGodotScriptClass(context, x.cds, x.symbol);
                            return false;
                        })
                        .Select(x => x.symbol)
                )
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .ToArray();
        public static bool TryGetGlobalAnalyzerProperty(
            this GeneratorExecutionContext context, string property, out string? value
        ) => context.AnalyzerConfigOptions.GlobalOptions
            .TryGetValue("build_property." + property, out value);

        public static bool AreGodotSourceGeneratorsDisabled(this GeneratorExecutionContext context)
            => context.TryGetGlobalAnalyzerProperty("GodotSourceGenerators", out string? toggle) &&
               toggle != null &&
               toggle.Equals("disabled", StringComparison.OrdinalIgnoreCase);

        public static bool IsGodotToolsProject(this GeneratorExecutionContext context)
            => context.TryGetGlobalAnalyzerProperty("IsGodotToolsProject", out string? toggle) &&
               toggle != null &&
               toggle.Equals("true", StringComparison.OrdinalIgnoreCase);

        public static bool IsGodotSourceGeneratorDisabled(this GeneratorExecutionContext context, string generatorName) =>
            AreGodotSourceGeneratorsDisabled(context) ||
            (context.TryGetGlobalAnalyzerProperty("GodotDisabledSourceGenerators", out string? disabledGenerators) &&
            disabledGenerators != null &&
            disabledGenerators.Split(';').Contains(generatorName));

        public static bool InheritsFrom(this INamedTypeSymbol? symbol, string assemblyName, string typeFullName)
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
                if (symbol.ContainingAssembly?.Name == "GodotSharp")
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
            this ClassDeclarationSyntax cds, Compilation compilation,
            out INamedTypeSymbol? symbol
        )
        {
            var sm = compilation.GetSemanticModel(cds.SyntaxTree);

            var classTypeSymbol = sm.GetDeclaredSymbol(cds);

            if (classTypeSymbol?.BaseType == null
                || !classTypeSymbol.BaseType.InheritsFrom("GodotSharp", ClassNames.GodotObject))
            {
                symbol = null;
                return false;
            }

            symbol = classTypeSymbol;
            return true;
        }

        public static IEnumerable<(ClassDeclarationSyntax cds, INamedTypeSymbol symbol)> SelectGodotScriptClasses(
            this IEnumerable<ClassDeclarationSyntax> source,
            Compilation compilation
        )
        {
            foreach (var cds in source)
            {
                if (cds.TryGetGodotScriptClass(compilation, out var symbol))
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

        public static string? GodotVariantType(this INamedTypeSymbol symbol)
        {
            switch (symbol.FullQualifiedNameOmitGlobal())
            {
                case "System.Boolean":
                    return "Bool";
                case "System.Int8":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Int128":
                    return "Int";
                case "System.Half":
                case "System.Single":
                case "System.Double":
                    return "Float";
                case "System.String":
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

        public static bool IsGodotObject(this INamedTypeSymbol symbol)
            => symbol.InheritsFrom("GodotSharp", ClassNames.GodotObject);

        public static bool IsGodotInternalNameAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.InternalNameAttr;

        public static bool IsGodotExportAttribute(this INamedTypeSymbol symbol)
            => symbol.FullQualifiedNameOmitGlobal() == ClassNames.ExportAttr || symbol.InheritsFrom("GodotSharp", "Godot.ExportAttribute");

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
            => symbol.Identity.Name == "GodotSharp";

        public static string Path(this Location location)
            => location.SourceTree?.GetLineSpan(location.SourceSpan).Path
               ?? location.GetLineSpan().Path;

        public static int StartLine(this Location location)
            => location.SourceTree?.GetLineSpan(location.SourceSpan).StartLinePosition.Line
               ?? location.GetLineSpan().StartLinePosition.Line;

        public static bool HasAttribute(this ITypeSymbol type, string attribute)
        {
            ImmutableArray<AttributeData> attr = type.GetAttributes();
            foreach (AttributeData a in attr)
                if (a.AttributeClass?.FullQualifiedNameOmitGlobal() == attribute)
                    return true;
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
    }
}
