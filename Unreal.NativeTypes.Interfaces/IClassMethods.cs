#pragma warning disable CS1591

namespace Unreal.NativeTypes.Interfaces;

public interface IClassMethods
{
    public unsafe delegate UObject* StaticConstructObject_Internal(FStaticConstructObjectParameters* pParams);
    public unsafe delegate void GetPrivateStaticClassBody(
        nint packageName,
        nint name,
        UClass** returnClass,
        nint registerNativeFunc,
        uint size,
        uint align,
        uint flags,
        ulong castFlags,
        nint config,
        nint inClassCtor,
        nint vtableHelperCtorCaller,
        nint addRefObjects,
        nint superFn,
        nint withinFn,
        byte isDynamic,
        nint dynamicFn);
}