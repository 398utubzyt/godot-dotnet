using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Godot.Roslyn
{
    /// <summary>
    /// Most of this is just taken from the
    /// <seealso href="https://github.com/godotengine/godot/blob/4.2/modules/mono/editor/Godot.NET.Sdk/Godot.SourceGenerators/GlobalClassAnalyzer.cs">
    /// Godot repository</seealso>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GlobalClassAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(
                Reporter.GlobalClassMustDeriveFromGodotObjectRule,
                Reporter.GlobalClassMustNotBeGenericRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax? typeClassDecl = (ClassDeclarationSyntax)context.Node;

            // Return if not a type symbol or the type is not a global class.
            if (context.ContainingSymbol is not INamedTypeSymbol typeSymbol ||
                !typeSymbol.GetAttributes().Any(a => a.AttributeClass?.IsGodotGlobalClassAttribute() ?? false))
                return;

            if (typeSymbol.IsGenericType)
                Reporter.ReportGlobalClassMustNotBeGeneric(context, typeClassDecl, typeSymbol);

            if (!typeSymbol.InheritsFrom("GodotSharp", ClassNames.GodotObject))
                Reporter.ReportGlobalClassMustDeriveFromGodotObject(context, typeClassDecl, typeSymbol);
        }
    }
}
