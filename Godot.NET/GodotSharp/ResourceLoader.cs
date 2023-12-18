namespace Godot
{
    public partial class ResourceLoader
    {
        public static T Load<T>(string path, CacheMode cacheMode = CacheMode.Ignore) where T : Resource
        {
            return Load(path, typeof(T).Name, cacheMode) as T;
        }
    }
}
