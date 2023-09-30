namespace Godot.GdExtension
{
	[SLayout(SLayoutOpt.Sequential)]
    internal struct CallableCustomInfo
    {
        /* Only `call_func` and `token` are strictly required, however, `object` should be passed if its not a static method.
		 *
		 * `token` should point to an address that uniquely identifies the GDExtension (for example, the
		 * `GDExtensionClassLibraryPtr` passed to the entry symbol function.
		 *
		 * `hash_func`, `equal_func`, and `less_than_func` are optional. If not provided both `call_func` and
		 * `callable_userdata` together are used as the identity of the callable for hashing and comparison purposes.
		 *
		 * The hash returned by `hash_func` is cached, `hash_func` will not be called more than once per callable.
		 *
		 * `is_valid_func` is necessary if the validity of the callable can change before destruction.
		 *
		 * `free_func` is necessary if `callable_userdata` needs to be cleaned up when the callable is freed.
		 */
		public unsafe void *CallableUserData;
		public unsafe void *Token;

		public GDExtensionObjectPtr Object;

		public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomCall> CallFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomIsValid> IsValidFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomFree> FreeFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomHash> HashFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomEqual> EqualFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomLessThan> LessThanFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionCallableCustomToString> ToStringFunc;
    }
}
