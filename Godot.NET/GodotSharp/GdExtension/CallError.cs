namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct CallError
    {
        public CallErrorType Error;
        public int Argument;
        public int Expected;

        public readonly override string ToString()
            => $"Error: {Error}\nArgument: {Argument}\nExpected: {Expected}\n";
    }
}
