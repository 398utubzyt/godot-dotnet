using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Godot.Roslyn
{
    public static class Reporter
    {
        public static void ReportNonPartialGodotScriptClass(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax cds, INamedTypeSymbol symbol
        )
        {
            string message =
                "Missing partial modifier on declaration of type '" +
                $"{symbol.FullQualifiedNameOmitGlobal()}' which is a subclass of '{ClassNames.GodotObject}'";

            string description = $"{message}. Subclasses of '{ClassNames.GodotObject}' " +
                                 "must be declared with the partial modifier.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0001",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                cds.GetLocation(),
                cds.SyntaxTree.FilePath));
        }

        public static void ReportNonPartialGodotScriptOuterClass(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax outerTypeDeclSyntax
        )
        {
            var outerSymbol = context.Compilation
                .GetSemanticModel(outerTypeDeclSyntax.SyntaxTree)
                .GetDeclaredSymbol(outerTypeDeclSyntax);

            string fullQualifiedName = outerSymbol is INamedTypeSymbol namedTypeSymbol ?
                namedTypeSymbol.FullQualifiedNameOmitGlobal() :
                "type not found";

            string message =
                $"Missing partial modifier on declaration of type '{fullQualifiedName}', " +
                $"which contains one or more subclasses of '{ClassNames.GodotObject}'";

            string description = $"{message}. Subclasses of '{ClassNames.GodotObject}' and their " +
                                 "containing types must be declared with the partial modifier.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0002",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                outerTypeDeclSyntax.GetLocation(),
                outerTypeDeclSyntax.SyntaxTree.FilePath));
        }

        public static void ReportMultipleScriptClassesInFile(
            GeneratorExecutionContext context,
            string sourceFile
        )
        {
            string message =
                "Multiple script classes were found in  '" +
                $"{sourceFile}'";

            string description = $"{message}. A script must contain only one" +
                                 "valid script class. Move the other script classes" +
                                 "to separate files.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0003",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                null,
                sourceFile));
        }

        public static void ReportMultipleScriptClassDeclarations(
            GeneratorExecutionContext context,
            INamedTypeSymbol symbol
        )
        {
            var locations = symbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message =
                "Multiple declarations of the same script class '" +
                $"{symbol.ToDisplayString()}'";

            string description = $"{message}. A script class must only be declared once" +
                                 " throughout the project. Remove the other declarations.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0004",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberIsStatic(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();
            bool isField = exportedMemberSymbol is IFieldSymbol;

            string message = $"Attempted to export static {(isField ? "field" : "property")}: " +
                             $"'{exportedMemberSymbol.ToDisplayString()}'";

            string description = $"{message}. Only instance fields and properties can be exported." +
                                 " Remove the 'static' modifier or the '[Export]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0101",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberTypeNotSupported(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();
            bool isField = exportedMemberSymbol is IFieldSymbol;

            string message = $"The type of the exported {(isField ? "field" : "property")} " +
                             $"is not supported: '{exportedMemberSymbol.ToDisplayString()}'";

            string description = $"{message}. Use a supported type or remove the '[Export]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0102",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberIsReadOnly(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();
            bool isField = exportedMemberSymbol is IFieldSymbol;

            string message = $"The exported {(isField ? "field" : "property")} " +
                             $"is read-only: '{exportedMemberSymbol.ToDisplayString()}'";

            string description = isField ?
                $"{message}. Exported fields cannot be read-only." :
                $"{message}. Exported properties must be writable.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0103",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberIsWriteOnly(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message = $"The exported property is write-only: '{exportedMemberSymbol.ToDisplayString()}'";

            string description = $"{message}. Exported properties must be readable.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0104",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberIsIndexer(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message = $"Attempted to export indexer property: " +
                             $"'{exportedMemberSymbol.ToDisplayString()}'";

            string description = $"{message}. Indexer properties can't be exported." +
                                 " Remove the '[Export]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0105",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportedMemberIsExplicitInterfaceImplementation(
            GeneratorExecutionContext context,
            ISymbol exportedMemberSymbol
        )
        {
            var locations = exportedMemberSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message = $"Attempted to export explicit interface property implementation: " +
                             $"'{exportedMemberSymbol.ToDisplayString()}'";

            string description = $"{message}. Explicit interface implementations can't be exported." +
                                 " Remove the '[Export]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0106",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportAttributeTypeArgumentIsNotPrimitive(
            GeneratorExecutionContext context,
            ISymbol typeArgSymbol,
            ITypeSymbol typeSymbol
        )
        {
            var locations = typeArgSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message = $"Export attribute type argument is not built-in primitive number type: " +
                             $"'{typeSymbol.ToDisplayString()}'";

            string description = $"{message}. Custom number types are not supported." +
                                 " Change the type argument to a primitive such as 'int' or 'float'.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0107",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static void ReportExportRangeDoesNotHaveMinMax(
            GeneratorExecutionContext context,
            ISymbol exportedSymbol
        )
        {
            var locations = exportedSymbol.Locations;
            var location = locations.FirstOrDefault(l => l.SourceTree != null) ?? locations.FirstOrDefault();

            string message = $"Exported range on property does not have both minimum and maximum bounds: " +
                             $"'{exportedSymbol.ToDisplayString()}'";

            string description = $"{message}. Export range must include a minimum and maximum." +
                                 " Set 'CanBeLess' or 'CanBeGreater' to 'true' if bounds are not needed.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0108",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                location,
                location?.SourceTree?.FilePath));
        }

        public static readonly DiagnosticDescriptor GenericTypeArgumentMustBeVariantRule =
            new DiagnosticDescriptor(id: "GD0301",
                title: "The generic type argument must be a Variant compatible type",
                messageFormat: "The generic type argument must be a Variant compatible type: {0}",
                category: "Usage",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                "The generic type argument must be a Variant compatible type. Use a Variant compatible type as the generic type argument.");

        public static void ReportGenericTypeArgumentMustBeVariant(
            SyntaxNodeAnalysisContext context,
            SyntaxNode typeArgumentSyntax,
            ISymbol typeArgumentSymbol)
        {
            string message = "The generic type argument " +
                            $"must be a Variant compatible type: '{typeArgumentSymbol.ToDisplayString()}'";

            string description = $"{message}. Use a Variant compatible type as the generic type argument.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0301",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                typeArgumentSyntax.GetLocation(),
                typeArgumentSyntax.SyntaxTree.FilePath));
        }

        public static readonly DiagnosticDescriptor GenericTypeParameterMustBeVariantAnnotatedRule =
            new DiagnosticDescriptor(id: "GD0302",
                title: "The generic type parameter must be annotated with the MustBeVariant attribute",
                messageFormat: "The generic type argument must be a Variant type: {0}",
                category: "Usage",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                "The generic type argument must be a Variant type. Use a Variant type as the generic type argument.");

        public static void ReportGenericTypeParameterMustBeVariantAnnotated(
            SyntaxNodeAnalysisContext context,
            SyntaxNode typeArgumentSyntax,
            ISymbol typeArgumentSymbol)
        {
            string message = "The generic type parameter must be annotated with the MustBeVariant attribute";
            string description = $"{message}. Add the MustBeVariant attribute to the generic type parameter.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0302",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                typeArgumentSyntax.GetLocation(),
                typeArgumentSyntax.SyntaxTree.FilePath));
        }

        public static readonly DiagnosticDescriptor TypeArgumentParentSymbolUnhandledRule =
            new DiagnosticDescriptor(id: "GD0303",
                title: "The generic type parameter must be annotated with the MustBeVariant attribute",
                messageFormat: "The generic type argument must be a Variant type: {0}",
                category: "Usage",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                "The generic type argument must be a Variant type. Use a Variant type as the generic type argument.");

        public static void ReportTypeArgumentParentSymbolUnhandled(
            SyntaxNodeAnalysisContext context,
            SyntaxNode typeArgumentSyntax,
            ISymbol parentSymbol)
        {
            string message = $"Symbol '{parentSymbol.ToDisplayString()}' parent of a type argument " +
                             "that must be Variant compatible was not handled.";

            string description = $"{message}. Handle type arguments that are children of the unhandled symbol type.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0303",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                typeArgumentSyntax.GetLocation(),
                typeArgumentSyntax.SyntaxTree.FilePath));
        }

        public static readonly DiagnosticDescriptor GlobalClassMustDeriveFromGodotObjectRule =
            new DiagnosticDescriptor(id: "GD0401",
                title: "The class must derive from GodotObject or a derived class",
                messageFormat: "The class '{0}' must derive from GodotObject or a derived class",
                category: "Usage",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                "The class must derive from GodotObject or a derived class. Change the base class or remove the '[GlobalClass]' attribute.");

        public static void ReportGlobalClassMustDeriveFromGodotObject(
            SyntaxNodeAnalysisContext context,
            SyntaxNode classSyntax,
            ISymbol typeSymbol)
        {
            string message = $"The class '{typeSymbol.ToDisplayString()}' must derive from GodotObject or a derived class";
            string description = $"{message}. Change the base class or remove the '[GlobalClass]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0401",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                classSyntax.GetLocation(),
                classSyntax.SyntaxTree.FilePath));
        }

        public static readonly DiagnosticDescriptor GlobalClassMustNotBeGenericRule =
            new DiagnosticDescriptor(id: "GD0402",
                title: "The class must not contain generic arguments",
                messageFormat: "The class '{0}' must not contain generic arguments",
                category: "Usage",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                "The class must be a non-generic type. Remove the generic arguments or the '[GlobalClass]' attribute.");

        public static void ReportGlobalClassMustNotBeGeneric(
            SyntaxNodeAnalysisContext context,
            SyntaxNode classSyntax,
            ISymbol typeSymbol)
        {
            string message = $"The class '{typeSymbol.ToDisplayString()}' must not contain generic arguments";
            string description = $"{message}. Remove the generic arguments or the '[GlobalClass]' attribute.";

            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(id: "GD0402",
                    title: message,
                    messageFormat: message,
                    category: "Usage",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description),
                classSyntax.GetLocation(),
                classSyntax.SyntaxTree.FilePath));
        }
    }
}
