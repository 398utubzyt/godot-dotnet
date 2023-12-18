using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Godot.Roslyn
{
    /// <summary>
    /// Most of this is just taken from the
    /// <seealso href="https://github.com/godotengine/godot/blob/4.2/modules/mono/editor/Godot.NET.Sdk/Godot.SourceGenerators/MustBeVariantAnalyzer.cs">
    /// Godot repository</seealso>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MustBeVariantAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => new() { 
            Reporter.GenericTypeArgumentMustBeVariantRule,
            Reporter.GenericTypeParameterMustBeVariantAnnotatedRule,
            Reporter.TypeArgumentParentSymbolUnhandledRule,
        };

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.TypeArgumentList);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (IsInsideDocumentation(context.Node))
                return;

            TypeArgumentListSyntax typeArgListSyntax = (TypeArgumentListSyntax)context.Node;

            SyntaxNode? parentSyntax = context.Node.Parent;
            Debug.Assert(parentSyntax != null);

            SemanticModel sm = context.SemanticModel;

            INamedTypeSymbol? gdObjType = context.Compilation.GetTypeByMetadataName(ClassNames.GodotObject);
            if (gdObjType == null)
                return; // This shouldn't happen

            for (int i = 0; i < typeArgListSyntax.Arguments.Count; i++)
            {
                TypeSyntax? typeSyntax = typeArgListSyntax.Arguments[i];

                // Ignore omitted type arguments, e.g.: List<>, Dictionary<,>, etc
                if (typeSyntax is OmittedTypeArgumentSyntax)
                    continue;

                ITypeSymbol? typeSymbol = sm.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol;
                Debug.Assert(typeSymbol != null);

                if (!ShouldCheckTypeArgument(sm.GetSymbolInfo(parentSyntax!).Symbol, i))
                    return;

                if (typeSymbol is ITypeParameterSymbol typeParamSymbol)
                {
                    if (!typeParamSymbol.GetAttributes().Any(a => a.AttributeClass?.IsGodotMustBeVariantAttribute() ?? false))
                        Reporter.ReportGenericTypeParameterMustBeVariantAnnotated(context, typeSyntax, typeSymbol);
                } else if (typeSymbol!.GodotVariantType() == null)
                    Reporter.ReportGenericTypeArgumentMustBeVariant(context, typeSyntax, typeSymbol!);
            }
        }

        private bool IsInsideDocumentation(SyntaxNode? syntax)
        {
            while (syntax != null)
            {
                if (syntax is DocumentationCommentTriviaSyntax)
                    return true;

                syntax = syntax.Parent;
            }

            return false;
        }

        private bool ShouldCheckTypeArgument(ISymbol? parentSymbol, int typeArgumentIndex)
        {
            ITypeParameterSymbol? typeParamSymbol = parentSymbol switch
            {
                IMethodSymbol methodSymbol => methodSymbol.TypeParameters[typeArgumentIndex],
                INamedTypeSymbol typeSymbol => typeSymbol.TypeParameters[typeArgumentIndex],
                _ => null,
            };

            if (typeParamSymbol == null)
                return false;

            return typeParamSymbol.GetAttributes()
                .Any(a => a.AttributeClass?.IsGodotMustBeVariantAttribute() ?? false);
        }
    }
}
