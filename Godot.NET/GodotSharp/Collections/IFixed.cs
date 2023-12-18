using System;
using System.Collections.Generic;

namespace Godot.Collections
{
    public interface IFixed<T>
    {
        ref T GetPinnableReference();
        Span<T> ToSpan();
    }
}
