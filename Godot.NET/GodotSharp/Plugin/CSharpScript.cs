using System;

namespace Godot
{
    public sealed partial class CSharpScript : ScriptExtension
    {
        private string _source;

        private Type _base;
        private Type _type;
        private bool _tool;
        private bool _global;

        public Type ManagedType => _type;

        public override void _SetSourceCode(string code)
        {
            _source = code;
        }

        public override string _GetSourceCode()
            => _source;
        public override bool _HasSourceCode()
            => _source != null;

        public override bool _IsAbstract()
            => _type.IsAbstract;
        public override bool _IsValid()
            => _type != null;
        public override bool _CanInstantiate()
            => !_type.IsAbstract && !_type.IsGenericType
#if TOOLS
            && _tool
#endif
            ;

        public override Script _GetBaseScript()
        {
            return base.GetBaseScript();
        }

        public override bool _IsTool()
            => _tool;

        public override unsafe void* _InstanceCreate(GodotObject forObject)
        {
            ScriptInstanceInfo2 info = new ScriptInstanceInfo2();
            // TODO: Populate info
            return (void*)Main.i.ScriptInstanceCreate2(&info, forObject.Handle);
        }

        public CSharpScript()
        {
            
        }
    }
}
