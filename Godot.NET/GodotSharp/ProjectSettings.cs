namespace Godot
{
    partial class ProjectSettings
    {
        public static void SetSetting<[MustBeVariant] T>(string name, T value)
            => SetSetting(name, Variant.From(value));
        public static T GetSetting<[MustBeVariant] T>(string name, T defaultValue = default)
            => GetSetting(name, Variant.From(defaultValue)).TryAs(out T result) ? result : default;
        public static T GetSettingWithOverride<[MustBeVariant] T>(string name)
            => GetSettingWithOverride(name).TryAs(out T result) ? result : default;
        public static void SetInitialValue<[MustBeVariant] T>(string name, T value)
            => SetInitialValue(name, Variant.From(value));
    }
}
