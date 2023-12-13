using System;
using System.Numerics;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportRangeAttribute<T> : Attribute where T : unmanaged, INumber<T>
    {
        /// <summary>
        /// The minimum inclusive value.
        /// </summary>
        public T Min;
        /// <summary>
        /// The maximum inclusive value.
        /// </summary>
        public T Max;
        /// <summary>
        /// The step size from <see cref="Min"/> which the value will snap to in the editor.
        /// </summary>
        public T Step;
        /// <summary>
        /// <see langword="true"/> if the range should not have a minimum bound.
        /// </summary>
        public bool CanBeLess;
        /// <summary>
        /// <see langword="true"/> if the range should not have a maximum bound.
        /// </summary>
        public bool CanBeGreater;
        /// <summary>
        /// <see langword="true"/> if the range slider should be exponential instead of linear.
        /// </summary>
        public bool Exponential;
        /// <summary>
        /// <see langword="true"/> if the slider should be hidden in the editor.
        /// </summary>
        public bool HideSlider;
        /// <summary>
        /// <see langword="true"/> if the editor should display a degree suffix.
        /// </summary>
        public bool Degrees;
        /// <summary>
        /// <see langword="true"/> if the editor should display the value as degrees, even though the actual value is in radians.
        /// </summary>
        public bool RadiansAsDegrees;
        /// <summary>
        /// The suffix to display in the editor. Usually used for displaying units (e.g. cm, ft, dB, etc.)
        /// </summary>
        public string Suffix;
    }
}
