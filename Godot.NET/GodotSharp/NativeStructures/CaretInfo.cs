namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct CaretInfo
    {
        public Rect2 LeadingCaret;
        public Rect2 TrailingCaret;
        public TextServer.Direction LeadingDirection;
        public TextServer.Direction TrailingDirection;
    }
}
