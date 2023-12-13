using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Godot.Roslyn
{
    public static class ExportAttributeHandler
    {
        private static TValue Get<TKey, TValue>(this ImmutableArray<KeyValuePair<TKey, TValue>> arr, TKey key) where TKey : IEquatable<TKey>
        {
            for (int i = 0; i < arr.Length; ++i)
                if (key.Equals(arr[i].Key))
                    return arr[i].Value;
            return default!;
        }

        private static bool As<T>(this TypedConstant constant, out T value)
        {
            if (constant.Value is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        private static bool As<T>(this TypedConstant constant, out ImmutableArray<T> value)
        {
            value = constant.Values.Select(tc => { tc.As(out T value); return value; }).ToImmutableArray();
            return !value.IsDefaultOrEmpty;
        }

        private static void HandleEnum(AttributeData data, INamedTypeSymbol type, StringBuilder sb, ref PropertyHint hint)
        {
            TypedConstant nconst = data.NamedArguments.Get("Names");
            if (!nconst.IsNull)
            {
                if (nconst.As(out ImmutableArray<string> names) && names.Length > 0)
                {
                    sb.Append(names[0]);
                    for (int i = 1; i < names.Length; i++)
                    {
                        sb.Append(',');
                        sb.Append(names[i]);
                    }
                }
                return;
            }

            if (type.TypeKind != TypeKind.Enum)
                return;

            IEnumerator<IFieldSymbol> values = type.GetMembers()
                .Where(sym => sym.Kind == SymbolKind.Field)
                .Cast<IFieldSymbol>().GetEnumerator();

            if (!values.MoveNext())
                return;

            IFieldSymbol field = values.Current;
            sb.Append(field.Name);
            sb.Append(':');
            sb.Append(field.ConstantValue);

            while (values.MoveNext())
            {
                field = values.Current;
                sb.Append(',');
                sb.Append(field.Name);
                sb.Append(':');
                sb.Append(field.ConstantValue);
            }
        }

        private static void HandlePath(AttributeData data, string property, INamedTypeSymbol type, StringBuilder sb, ref PropertyHint hint)
        {
            TypedConstant nconst = data.NamedArguments.Get(property);
            
            if (nconst.IsNull)
                return;
            if (!nconst.As(out ImmutableArray<string> names))
                return;
            sb.Append(names[0]);
            for (int i = 1; i < names.Length; i++)
            {
                sb.Append(',');
                sb.Append(names[i]);
            }
        }

        private static void HandlePlaceholder(AttributeData data, INamedTypeSymbol type, StringBuilder sb, ref PropertyHint hint)
        {
            TypedConstant nconst = data.NamedArguments.Get("Placeholder");

            if (nconst.IsNull)
                return;
            if (!nconst.As(out string placeholder))
                return;
            sb.Append(placeholder);
        }

        private static bool RangeAppend(AttributeData data, StringBuilder sb, string property, string show)
        {
            TypedConstant constant = data.NamedArguments.Get(property);
            if (constant.IsNull || (constant.Type!.SpecialType == SpecialType.System_Boolean && !((bool?)constant.Value ?? false)))
                return false;

            sb.Append(',');
            sb.Append(show);
            return true;
        }

        private static void HandleRange(AttributeData data, INamedTypeSymbol type, StringBuilder sb, ref PropertyHint hint)
        {
            // This sucks. I hate this. Why. -398

            INamedTypeSymbol attrType = data.AttributeClass!;
            if (!attrType.IsUnboundGenericType && (attrType.TypeArguments.Length == 0 || !attrType.TypeArguments[0].IsBuiltinNumber()))
            {
                // Reporter.ReportExportAttributeTypeArgumentIsNotPrimitive();
                return;
            }

            TypedConstant min = data.NamedArguments.Get("Min");
            TypedConstant max = data.NamedArguments.Get("Max");
            if (min.IsNull || max.IsNull)
                return;

            sb.Append(min.ToCSharpString());
            sb.Append(',');
            sb.Append(max.ToCSharpString());

            TypedConstant step = data.NamedArguments.Get("Step");
            if (!step.IsNull)
            {
                sb.Append(',');
                sb.Append(step.ToCSharpString());
            }

            RangeAppend(data, sb, "CanBeLess", "or_less");
            RangeAppend(data, sb, "CanBeGreater", "or_greater");
            RangeAppend(data, sb, "Exponential", "exp");
            RangeAppend(data, sb, "HideSlider", "hide_slider");
            RangeAppend(data, sb, "Degrees", "degrees");
            RangeAppend(data, sb, "RadiansAsDegrees", "radians_as_degrees");

            TypedConstant suffix = data.NamedArguments.Get("Suffix");
            if (!suffix.IsNull && suffix.As(out string suffixString))
            {
                sb.Append(",suffix:");
                sb.Append(suffixString);
            }
        }

        public static bool GetPropertyInfo(AttributeData data, INamedTypeSymbol type, ref PropertyHint hint, out string hintString)
        {
            StringBuilder sb = new StringBuilder();

            switch (hint)
            {
                // case PropertyHint.ColorNoAlpha:
                // case PropertyHint.Dir:
                // case PropertyHint.GlobalDir:
                // case PropertyHint.Layers2dNavigation:
                // case PropertyHint.Layers2dPhysics:
                // case PropertyHint.Layers2dRender:
                // case PropertyHint.Layers3dNavigation:
                // case PropertyHint.Layers3dPhysics:
                // case PropertyHint.Layers3dRender:
                // case PropertyHint.LayersAvoidance:
                // case PropertyHint.MultilineText:
                //     break;
                case PropertyHint.PlaceholderText:
                    HandlePlaceholder(data, type, sb, ref hint);
                    break;
                case PropertyHint.NodePathValidTypes:
                    HandlePath(data, "Types", type, sb, ref hint);
                    break;
                case PropertyHint.File:
                case PropertyHint.GlobalFile:
                    HandlePath(data, "Filters", type, sb, ref hint);
                    break;
                case PropertyHint.Enum:
                case PropertyHint.Flags:
                    HandleEnum(data, type, sb, ref hint);
                    break;
                case PropertyHint.Range:
                    HandleRange(data, type, sb, ref hint);
                    break;
            }

            switch ((PropertyExportExt)hint)
            {
                case PropertyExportExt.Export:
                    hint = data.NamedArguments.Get("Hint").As(out long value) ? (PropertyHint)value : PropertyHint.None;
                    if (data.NamedArguments.Get("HintString").As(out string hs))
                        sb.Append(hs);
                    break;
            }

            hintString = sb.ToString();
            return true;
        }
    }
}
