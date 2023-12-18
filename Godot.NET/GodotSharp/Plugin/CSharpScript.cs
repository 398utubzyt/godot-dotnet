using System;
using System.Collections.Generic;

namespace Godot
{
    public sealed partial class CSharpScript : ScriptExtension
    {
        private string _source;

        // Scripts are required to only contain a single script class -- no need to store any more!
        private nint _type;
        private StringName _name;
        private bool _valid;
        private bool _generic;
        private bool _abstract;
        private bool _tool;
        private bool _global;

        public Type ManagedType => _type.AsManagedType();

        public override void _SetSourceCode(string code)
        {
            _source = code;
        }

        public override string _GetSourceCode()
            => _source;
        public override bool _HasSourceCode()
            => !string.IsNullOrEmpty(_source);

        public override bool _IsAbstract()
            => _abstract;
        public override bool _IsValid()
            => _valid;
        public override bool _CanInstantiate()
            => !_abstract && !_generic
#if TOOLS
            && _tool
#endif
            ;
        public override StringName _GetGlobalName()
            => _global ? _name : default;

        public override Script _GetBaseScript()
            => ScriptDB.Search(_type.AsManagedType().BaseType, out string basePath) ? ResourceLoader.Load<Script>(basePath) : null;

        public override bool _IsTool()
            => _tool;

        public override Error _Reload(bool keepState)
        {
            _valid = ScriptDB.Search(GetPath(), out Type type);

            if (_valid)
            {
                _type = type.TypeHandle.Value;
                _name = type.Name;
                _abstract = type.IsAbstract;
                _generic = type.IsGenericType;
                _tool = type.HasAttribute(typeof(ToolAttribute));
                _global = type.HasAttribute(typeof(GlobalClassAttribute));
            }
            
            return Error.Ok;
        }

        public override unsafe void* _InstanceCreate(GodotObject forObject)
        {
            // ScriptInstanceInfo2 info = new ScriptInstanceInfo2();
            // TODO: Populate info
            return null;//(void*)Main.i.ScriptInstanceCreate2(&info, forObject.Handle);
        }

        public CSharpScript()
        {
        }

        private static bool _CanCall(StringName method)
        {
            return method == "_set_source_code" ||
                method == "_get_source_code" ||
                method == "has_source_code" ||
                method == "_is_abstract" ||
                method == "_is_valid" ||
                method == "_can_instantiate" ||
                method == "_get_global_name" ||
                method == "_get_base_script" ||
                method == "_is_tool" ||
                method == "_instance_create";
        }

        protected override void _Call(StringName method, ref nint args, nint ret)
        {
            if (method == "_set_source_code")
            {
                _SetSourceCode(StringDB.SearchOrCreate(MemUtil.IntAsRef<nint>(args)));
                return;
            }

            if (method == "_get_source_code")
            {
                StringDB.Placement(_GetSourceCode(), ref MemUtil.IntAsRef<nint>(ret));
                return;
            }

            if (method == "_has_source_code")
            {
                MemUtil.IntAsRef<byte>(ret) = _HasSourceCode().ToExtBool();
                return;
            }

            if (method == "_is_abstract")
            {
                MemUtil.IntAsRef<byte>(ret) = _IsAbstract().ToExtBool();
                return;
            }

            if (method == "_is_valid")
            {
                MemUtil.IntAsRef<byte>(ret) = _IsValid().ToExtBool();
                return;
            }

            if (method == "_can_instantiate")
            {
                MemUtil.IntAsRef<byte>(ret) = _CanInstantiate().ToExtBool();
                return;
            }

            if (method == "_get_global_name")
            {
                MemUtil.IntAsRef<StringName>(ret) = _GetGlobalName();
                return;
            }

            if (method == "_get_base_script")
            {
                MemUtil.IntAsRef<nint>(ret) = (nint)_GetBaseScript();
                return;
            }

            if (method == "_is_tool")
            {
                MemUtil.IntAsRef<byte>(ret) = _IsTool().ToExtBool();
                return;
            }

            if (method == "_instance_create")
            {
                unsafe
                {
                    MemUtil.IntAsRef<nint>(ret) = 
                        (nint)_InstanceCreate(
                            ClassDB.GetOrMakeHandleFromNative(MemUtil.IntAsRef<nint>(args)).Target as GodotObject);
                }
                return;
            }
        }
    }
}
