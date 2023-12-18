using System.Linq;

namespace Godot
{
    internal static class TypeExtensions
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static bool HasAttribute(this System.Type self, System.Type attrType)
            => self.CustomAttributes.Any(attr => attr.GetType() == attrType);
    }
}
