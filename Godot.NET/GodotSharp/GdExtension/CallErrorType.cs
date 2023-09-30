namespace Godot.GdExtension
{
    internal enum CallErrorType
    {
        Ok,
        InvalidMethod,
        InvalidArgument,
        TooManyArguments,
        TooFewArguments,
        InstanceIsNull,
        MethodNotConst
    }
}
