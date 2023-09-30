namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct Glyph
    {
        public int Start = -1;
        public int End = -1;
        public byte Count = 0; 
        public sbyte Repeat = 1;
        public ushort Flags = 0; 
        public float XOff = 0.0f; 
        public float YOff = 0.0f; 
        public float Advance = 0.0f; 
        public Rid FontRid; 
        public int FontSize = 0; 
        public int Index = 0;

        public Glyph()
        {
        }
    }
}
