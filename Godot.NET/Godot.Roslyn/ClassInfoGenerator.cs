using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Godot.Roslyn
{
    [Generator]
    public class ClassInfoGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.IsGodotSourceGeneratorDisabled("ClassInfo"))
                return;

            foreach (var godotClass in context.GetGodotClasses())
            {
                VisitGodotScriptClass(context, godotClass);
            }
        }

        private class MethodOverloadEqualityComparer : IEqualityComparer<GodotMethodInfo>
        {
            public bool Equals(GodotMethodInfo x, GodotMethodInfo y)
                => x.Params.Length == y.Params.Length && x.Method.Name == y.Method.Name;

            public int GetHashCode(GodotMethodInfo obj)
            {
                unchecked
                {
                    return (obj.Params.Length.GetHashCode() * 397) ^ obj.Method.Name.GetHashCode();
                }
            }
        }

        private static void VisitGodotScriptClass(GeneratorExecutionContext context, INamedTypeSymbol symbol)
        {
            INamespaceSymbol namespaceSymbol = symbol.ContainingNamespace;
            string classNs = namespaceSymbol != null && !namespaceSymbol.IsGlobalNamespace ?
                namespaceSymbol.FullQualifiedNameOmitGlobal() :
                string.Empty;
            bool hasNamespace = classNs.Length != 0;

            bool isInnerClass = symbol.ContainingType != null;

            string uniqueHint = symbol.FullQualifiedNameOmitGlobal().SanitizeQualifiedNameForUniqueHint()
                                + "_ClassInfo.gen";

            var source = new StringBuilder();

            source.Append("using Godot;\n");
            source.Append("\n");

            if (hasNamespace)
            {
                source.Append("namespace ");
                source.Append(classNs);
                source.Append(";\n\n");
            }

            if (isInnerClass)
            {
                var containingType = symbol.ContainingType;

                while (containingType != null)
                {
                    source.Append("partial ");
                    source.Append(containingType.GetDeclarationKeyword());
                    source.Append(" ");
                    source.Append(containingType.NameWithTypeParameters());
                    source.Append("\n{\n");

                    containingType = containingType.ContainingType;
                }
            }

            source.Append("partial class ");
            source.Append(symbol.NameWithTypeParameters());
            source.Append("\n{\n");

            AppendExportedProperties(source, symbol);
            AppendMethods(source, symbol);
            
            source.Append("}\n");

            if (isInnerClass)
            {
                var containingType = symbol.ContainingType;
                while (containingType != null)
                {
                    source.Append("\n}\n");
                    containingType = containingType.ContainingType;
                }
            }


            context.AddSource(uniqueHint, SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private static void AppendExportedProperties(StringBuilder source, INamedTypeSymbol symbol)
        {
            source.Append("protected override nuint _PropertyCount() => ");
            ImmutableArray<ISymbol> exportPropSymbols = symbol.GetMembers().Where(
                m => m.GetAttributes().Any(attr => attr.AttributeClass?.IsGodotExportAttribute() ?? false) &&
                (m.Kind == SymbolKind.Field || (m.Kind == SymbolKind.Property &&
                    ((IPropertySymbol)m).GetMethod != null && ((IPropertySymbol)m).SetMethod != null))).ToImmutableArray();
            source.Append(exportPropSymbols.Length);
            source.Append(";\nprotected override void _GetPropertyList(Span<PropertyInfo> info)\n{\n");
            for (int i = 0; i < exportPropSymbols.Length; i++)
            {
                source.Append("info[");
                source.Append(i);
                source.Append("] = new PropertyInfo() { Name = \"");
                source.Append(exportPropSymbols[i].Name);
                source.Append("\", Type = VariantType.");
                if (exportPropSymbols[i] is IPropertySymbol propSym)
                    source.Append((propSym.Type as INamedTypeSymbol)?.GodotVariantType() ?? "Nil");
                else if (exportPropSymbols[i] is IFieldSymbol fieldSym)
                    source.Append((fieldSym.Type as INamedTypeSymbol)?.GodotVariantType() ?? "Nil");
                else
                    source.Append("Nil");

                AttributeData expAttr = exportPropSymbols[i].GetAttributes().First(attr => attr.AttributeClass?.IsGodotExportAttribute() ?? false);
                foreach (KeyValuePair<string, TypedConstant> param in expAttr.NamedArguments)
                {
                    switch (param.Key)
                    {
                        case "Hint":
                            source.Append(", Hint = PropertyHint.");
                            if (param.Value.Type?.FullQualifiedNameOmitGlobal() == "Godot.PropertyHint")
                                source.Append(param.Value.Value?.ToString() ?? "None");
                            else
                                source.Append("None");
                            break;
                        case "HintString":
                            source.Append(", HintString = ");
                            if (param.Value.Type?.FullQualifiedNameOmitGlobal() == "System.String")
                                source.Append(param.Value.Value?.ToString() ?? "null");
                            else
                                source.Append("null");
                            break;
                    }
                }

                source.Append(" };");
            }
            source.Append("\n}\n");
        }

        private static void AppendMethods(StringBuilder source, INamedTypeSymbol symbol)
        {
            source.Append("protected override bool _Call(StringName method, ref Variant arg0, ref Variant ret)\n{\n");
            ImmutableArray<ISymbol> callSymbols = symbol.GetMembers().Where(m => m.Kind == SymbolKind.Method && m is IMethodSymbol ms
                && ms.DeclaredAccessibility == Accessibility.Public && !ms.IsStatic && !ms.IsImplicitlyDeclared
                && !ms.IsExtern && !ms.IsGenericMethod).ToImmutableArray();
            foreach (IMethodSymbol msym in callSymbols)
            {
                source.Append("if (method == \"");
                source.Append(msym.GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass?.IsGodotInternalNameAttribute() ?? false)?
                    .NamedArguments[0].Value.Value?.ToString() ?? msym.Name);
                source.Append("\")\n{\n");
                if (!msym.ReturnsVoid)
                    source.Append("ret = ");
                source.Append(msym.Name);
                source.Append('(');
                int i = 0;
                foreach (IParameterSymbol param in msym.Parameters)
                {
                    if (param.IsThis)
                        continue;
                    if (i > 0)
                        source.Append(", ");
                    source.Append('(');
                    source.Append(param.Type.FullQualifiedNameIncludeGlobal());
                    source.Append(")(global::System.Runtime.CompilerServices.Unsafe.Add(ref arg0, ");
                    source.Append(i++);
                    source.Append(')');
                }
                source.Append(");\nreturn true;\n}\n");
            }
            source.Append("return base._Call(method, ref arg0, ref ret);\n}\n");
        }
    }
}
