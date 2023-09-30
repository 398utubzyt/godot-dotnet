using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace Godot.Roslyn
{
    public struct GodotMethodInfo
    {
        public IMethodSymbol Method;
        public ImmutableArray<IParameterSymbol> Params;
        public bool HasReturn;
        public ITypeSymbol Return;
    }
}
