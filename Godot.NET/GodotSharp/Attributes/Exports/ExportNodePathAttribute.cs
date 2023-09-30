using System;
using System.Linq;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportNodePathAttribute : ExportAttribute
    {
        public ExportNodePathAttribute() : base(PropertyHint.NodePathValidTypes, null)
        {
        }
        public ExportNodePathAttribute(params string[] types)
            : base(PropertyHint.NodePathValidTypes, types != null ? string.Join(',', types) : null)
        {
        }
        public ExportNodePathAttribute(params Type[] types)
            : base(PropertyHint.NodePathValidTypes, types != null ? string.Join(',', types.Select((a) => a.Name)) : null)
        {
        }
    }
}
