using System;

namespace Godot
{
    [Flags]
    public enum ExportRangeFlags
    {
        CanBeGreater = 1,
        CanBeLess = 2,
        Exponential = 4,
        HideSlider = 8,
        Degrees = 16,
        Radians = 32,
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportRangeAttribute : ExportAttribute
    {
        public ExportRangeAttribute() : base(PropertyHint.Range, null)
        {
        }
        public ExportRangeAttribute(int min, int max)
            : base(PropertyHint.Range, $"{min},{max}")
        {
        }
        public ExportRangeAttribute(int min, int max, bool canBeLesser, bool canBeGreater)
            : base(PropertyHint.Range,
                  $"{min},{max}{
                      (canBeGreater ? ",or_greater" : string.Empty)}{
                      (canBeLesser ? ",or_less" : string.Empty)}")
        {
        }
        public ExportRangeAttribute(int min, int max, bool canBeLesser, bool canBeGreater, string suffix)
            : base(PropertyHint.Range, 
                  $"{min},{max}{
                      (canBeGreater ? ",or_greater" : string.Empty)}{
                      (canBeLesser ? ",or_less" : string.Empty)}{
                      (string.IsNullOrWhiteSpace(suffix) ? $",suffix:{suffix}" : string.Empty)}")
        {
        }

        public ExportRangeAttribute(float min, float max)
            : base(PropertyHint.Range, $"{min},{max}")
        {
        }
        public ExportRangeAttribute(float min, float max, float step)
            : base(PropertyHint.Range, $"{min},{max},{step}")
        {
        }
        public ExportRangeAttribute(float min, float max, float step, bool canBeLesser, bool canBeGreater)
            : base(PropertyHint.Range,
                  $"{min},{max},{step}{
                      (canBeGreater ? ",or_greater" : string.Empty)}{
                      (canBeLesser ? ",or_less" : string.Empty)}")
        {
        }
        public ExportRangeAttribute(float min, float max, float step, ExportRangeFlags flags)
            : base(PropertyHint.Range,
                  $"{min},{max},{step}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeGreater) ? ",or_greater" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeLess) ? ",or_less" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Exponential) ? ",exp" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.HideSlider) ? ",hide_slider" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Degrees) ? ",degrees" : 
                      (flags.FastHasFlag(ExportRangeFlags.Radians) ? ",radians" : string.Empty))}")
        {
        }
        public ExportRangeAttribute(float min, float max, float step, ExportRangeFlags flags, string suffix)
            : base(PropertyHint.Range, 
                  $"{min},{max},{step}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeGreater) ? ",or_greater" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeLess) ? ",or_less" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Exponential) ? ",exp" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.HideSlider) ? ",hide_slider" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Degrees) ? ",degrees" : 
                      (flags.FastHasFlag(ExportRangeFlags.Radians) ? ",radians" : string.Empty))}{
                      (string.IsNullOrWhiteSpace(suffix) ? $",suffix:{suffix}" : string.Empty)}")
        {
        }

        public ExportRangeAttribute(double min, double max)
            : base(PropertyHint.Range, $"{min},{max}")
        {
        }
        public ExportRangeAttribute(double min, double max, double step)
            : base(PropertyHint.Range, $"{min},{max},{step}")
        {
        }
        public ExportRangeAttribute(double min, double max, double step, bool canBeLesser, bool canBeGreater)
            : base(PropertyHint.Range,
                  $"{min},{max},{step}{
                      (canBeGreater ? ",or_greater" : string.Empty)}{
                      (canBeLesser ? ",or_less" : string.Empty)}")
        {
        }
        public ExportRangeAttribute(double min, double max, double step, ExportRangeFlags flags)
            : base(PropertyHint.Range,
                  $"{min},{max},{step}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeGreater) ? ",or_greater" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeLess) ? ",or_less" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Exponential) ? ",exp" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.HideSlider) ? ",hide_slider" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Degrees) ? ",degrees" : 
                      (flags.FastHasFlag(ExportRangeFlags.Radians) ? ",radians" : string.Empty))}")
        {
        }
        public ExportRangeAttribute(double min, double max, double step, ExportRangeFlags flags, string suffix)
            : base(PropertyHint.Range, 
                  $"{min},{max},{step}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeGreater) ? ",or_greater" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.CanBeLess) ? ",or_less" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Exponential) ? ",exp" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.HideSlider) ? ",hide_slider" : string.Empty)}{
                      (flags.FastHasFlag(ExportRangeFlags.Degrees) ? ",degrees" : 
                      (flags.FastHasFlag(ExportRangeFlags.Radians) ? ",radians" : string.Empty))}{
                      (string.IsNullOrWhiteSpace(suffix) ? $",suffix:{suffix}" : string.Empty)}")
        {
        }
    }
}
