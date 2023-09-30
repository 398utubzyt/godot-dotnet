using System.Runtime.CompilerServices;

namespace Godot
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if <paramref name="value"/> has the provided bit <paramref name="flag"/> set.
        /// </summary>
        /// <typeparam name="T">The type of the <see langword="enum"/>.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="flag">The flag(s) to check for.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> contains <paramref name="flag"/>, 
        ///     otherwise <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentException"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe bool FastHasFlag<T>(this T value, T flag) where T : unmanaged, System.Enum
            => sizeof(T) switch
            {
                sizeof(byte) => (*(byte*)(void*)&value & *(byte*)(void*)&flag) == *(byte*)(void*)&flag,
                sizeof(short) => (*(short*)(void*)&value & *(short*)(void*)&flag) == *(short*)(void*)&flag,
                sizeof(int) => (*(int*)(void*)&value & *(int*)(void*)&flag) == *(int*)(void*)&flag,
                sizeof(long) => (*(long*)(void*)&value & *(long*)(void*)&flag) == *(long*)(void*)&flag,
                _ => throw new System.ArgumentException("Invalid enum size.", nameof(T)),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe bool IsZero<T>(this T value) where T : unmanaged, System.Enum
            => sizeof(T) switch
            {
                sizeof(byte) => (*(byte*)(void*)&value) == (byte)0,
                sizeof(short) => (*(short*)(void*)&value) == (short)0,
                sizeof(int) => (*(int*)(void*)&value) == (int)0,
                sizeof(long) => (*(long*)(void*)&value) == (long)0,
                _ => throw new System.ArgumentException("Invalid enum size.", nameof(T)),
            };
    }
}
