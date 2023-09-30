using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Godot.Roslyn
{
    [Generator]
    public class AssemblyHasClassesGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.IsGodotSourceGeneratorDisabled("AssemblyHasClasses"))
                return;

            INamedTypeSymbol[] types = context.GetGodotClasses();
            StringBuilder source = new StringBuilder();
            source.Append("[assembly: global::Godot.AssemblyHasClasses(");

            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    source.Append(", ");
                source.Append("typeof(");
                source.Append(types[i].FullQualifiedNameIncludeGlobal());
                source.Append(")");
            }

            source.Append(")]");

            context.AddSource($"{context.Compilation.Assembly.Name}_HasClasses.gen", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }
}
