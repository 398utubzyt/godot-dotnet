﻿using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportSubgroupAttribute : Attribute
    {
        public string Name;

        public ExportSubgroupAttribute(string name)
            => Name = name;
    }
}
