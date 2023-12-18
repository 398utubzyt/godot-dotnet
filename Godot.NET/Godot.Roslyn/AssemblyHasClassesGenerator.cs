using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;

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

            if (!context.TryGetGlobalAnalyzerProperty("GodotProjectDir", out string? baseDir))
                return;

            INamedTypeSymbol[] types = context.GetGodotClasses();
            StringBuilder source = new StringBuilder();
            source.Append("[assembly: global::Godot.AssemblyHasClasses(\n    Types = new global::System.Type[] {\n        ");

            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    source.Append(",\n        ");
                source.Append("typeof(");
                source.Append(types[i].FullQualifiedNameIncludeGlobal());
                source.Append(')');
            }

            source.Append(" },\n    Paths = new string[] {\n        ");
            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0)
                    source.Append(",\n        ");
                source.Append('\"');
                source.Append(types[i].Locations[0].SourceTree?.FilePath.Replace(baseDir, "res://") ?? string.Empty);
                source.Append('\"');
            }

            source.Append(" })]");

            context.AddSource($"{context.Compilation.Assembly.Name}_HasClasses.gen", SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }
}
