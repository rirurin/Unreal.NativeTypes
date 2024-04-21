#pragma warning disable CS1591

namespace Unreal.NativeTypes.Interfaces;

public interface IMemoryMethods
{
    public unsafe void FMemory_Free(nint ptr);
    public unsafe nint FMemory_Realloc(nint ptr, nint size, uint alignment);
    public unsafe nint FMemory_GetAllocSize(nint ptr);
    public unsafe nint FMemory_Malloc(nint size, uint alignment);
}
