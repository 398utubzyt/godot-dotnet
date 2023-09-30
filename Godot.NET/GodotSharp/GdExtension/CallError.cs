namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct CallError
    {
        public CallErrorType Error;
        public int Argument;
        public int Expected;
    }
}
