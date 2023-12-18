using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            source.Append("using Godot;\nusing System;\n\n");

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
            source.Append("protected override int _PropertyCount() => ");
            ImmutableArray<ISymbol> exportPropSymbols = symbol.GetMembers().Where(
                m => m.GetAttributes().Any(attr => attr.AttributeClass?.IsAnyGodotExportAttribute() ?? false) &&
                (m.Kind == SymbolKind.Field || (m.Kind == SymbolKind.Property &&
                    ((IPropertySymbol)m).GetMethod != null && ((IPropertySymbol)m).SetMethod != null))).ToImmutableArray();
            source.Append(exportPropSymbols.Length);
            source.Append(";\nprotected override void _GetPropertyList(Span<PropertyInfo> info)\n{\n");
            for (int i = 0; i < exportPropSymbols.Length; i++)
            {
                string symType;
                INamedTypeSymbol typeSymbol;
                if (exportPropSymbols[i] is IPropertySymbol propSym)
                    symType = (typeSymbol = (INamedTypeSymbol)propSym.Type)!.GodotVariantType() ?? "Nil";
                else if (exportPropSymbols[i] is IFieldSymbol fieldSym)
                    symType = (typeSymbol = (INamedTypeSymbol)fieldSym.Type)!.GodotVariantType() ?? "Nil";
                else
                    continue;

                source.Append("info[");
                source.Append(i);
                source.Append("] = new PropertyInfo() { Name = \"");
                source.Append(exportPropSymbols[i].Name);
                source.Append("\", Type = Variant.Type.");
                
                source.Append(symType);

                if (symType == "Object")
                {
                    source.Append(", ClassName = \"");
                    source.Append(typeSymbol.FullQualifiedNameOmitGlobal() == "Godot.GodotObject" ? "Object" : typeSymbol);
                    source.Append('\"');
                }
                
                AttributeData expAttr = exportPropSymbols[i].GetAttributes().First(attr => attr.AttributeClass?.IsSomeGodotExportAttribute() ?? false);
                HandleExportProperty(source, typeSymbol, expAttr);

                source.Append(" };\n");
            }
            source.Append("}\n");
        }

        private static void HandleExportProperty(StringBuilder source, INamedTypeSymbol symbol, AttributeData data)
        {
            PropertyHint hint = data.GetPropertyHint();
            PropertyExportExt ext = (PropertyExportExt)hint;
            if (!System.Enum.IsDefined(typeof(PropertyHint), hint) && ext != PropertyExportExt.Export)
                return;

            if (!ExportAttributeHandler.GetPropertyInfo(data, symbol, ref hint, out string hintString))
                return;

            source.Append(", Hint = PropertyHint.");
            source.Append(hint.ToString());
            source.Append(", HintString = \"");
            source.Append(hintString);
            source.Append("\"");
        }

        private static void AppendMethods(StringBuilder source, INamedTypeSymbol symbol)
        {
            source.Append("private static bool _CanCall(StringName method)\n{\nreturn");

            ImmutableArray<ISymbol> callSymbols = symbol.GetMembers().Where(m => m.Kind == SymbolKind.Method && m is IMethodSymbol ms
                && ms.DeclaredAccessibility == Accessibility.Public && !ms.IsStatic && !ms.IsImplicitlyDeclared
                && !ms.IsExtern && !ms.IsGenericMethod).ToImmutableArray();

            if (callSymbols.Length > 0)
            {
                if (!symbol.HasAttribute(ClassNames.ToolAttr))
                    source.Append("\n#if TOOLS\nEngine.IsEditorHint() &&\n#endif\n");

                source.Append('(');
                source.Append("method == \"");
                source.Append(((IMethodSymbol)callSymbols[0]).InternalMethodName());
                source.Append('\"');
                for (int i = 1; i < callSymbols.Length; i++)
                {
                    source.Append(" ||\n");
                    source.Append("method == \"");
                    source.Append(((IMethodSymbol)callSymbols[i]).InternalMethodName());
                    source.Append('\"');
                }
                source.Append(')');
            } else
                source.Append(" false");

            source.Append(";\n}\nprotected override void _Call(StringName method, ref nint args, nint ret)\n{\n");
            foreach (IMethodSymbol msym in callSymbols)
            {
                source.Append("if (method == \"");
                source.Append(msym.InternalMethodName());
                source.Append("\")\n{\n");
                bool modify = false;
                if (!msym.ReturnsVoid)
                {
                    source.Append("global::Godot.Interop.RefHelper.IntAsRef<");
                    source.Append((msym.ReturnType as INamedTypeSymbol)?.GodotNativeType(out modify) ?? "nint");
                    source.Append(">(ret) = ");
                }
                source.Append(msym.Name);
                source.Append('(');
                int i = 0;
                foreach (IParameterSymbol param in msym.Parameters)
                {
                    if (param.IsThis)
                        continue;
                    if (i > 0)
                        source.Append(", ");
                    source.Append("global::Godot.Interop.RefHelper.IntAsRef<");
                    source.Append(param.Type.FullQualifiedNameIncludeGlobal());
                    source.Append(">(");
                    if (i != 0)
                    {
                        source.Append("global::Godot.Interop.RefHelper.AddRef(ref args, ");
                        source.Append(i++);
                        source.Append(')');
                    } else
                        source.Append("args");
                    source.Append(')');
                }
                source.Append(')');
                if (modify)
                {
                    source.Append("global::Godot.Interop.RefHelper.IntAsRef<");
                    switch (msym.ReturnType.SpecialType)
                    {
                        case SpecialType.System_Boolean:
                            source.Append(" ? 1 : 0");
                            break;
                    }
                }
                source.Append(";\nreturn;\n}\n");
            }
            source.Append("base._Call(method, ref args, ret);\n}\n");
        }
    }
}
