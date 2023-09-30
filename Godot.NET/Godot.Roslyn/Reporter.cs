using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
    }
}
