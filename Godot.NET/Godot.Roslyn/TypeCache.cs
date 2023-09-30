using Microsoft.CodeAnalysis;

using System;

namespace Godot.Roslyn
{
    public class TypeCache
    {
        public INamedTypeSymbol GodotObjectType { get; }

        public TypeCache(Compilation compilation)
        {
            INamedTypeSymbol GetTypeByMetadataNameOrThrow(string fullyQualifiedMetadataName)
            {
                return compilation.GetTypeByMetadataName(fullyQualifiedMetadataName) ??
                       throw new InvalidOperationException($"Type not found: '{fullyQualifiedMetadataName}'.");
            }

            GodotObjectType = GetTypeByMetadataNameOrThrow(ClassNames.GodotObject);
        }
    }
}
