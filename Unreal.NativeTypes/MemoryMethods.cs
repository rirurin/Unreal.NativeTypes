using Reloaded.Hooks.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.NativeTypes.Interfaces;

namespace Unreal.NativeTypes
{
    internal class MemoryMethods : IMemoryMethods
    {
        private string GMalloc_SIG = "48 8B 0D ?? ?? ?? ?? 48 85 C9 75 ?? E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 48 8B 01 48 8B D3 FF 50 ?? 48 83 C4 20";
        private unsafe nint* gMallocPtr;

        private long _baseAddress;
        private IStartupScanner _scanner;
        private ILogger _logger;
        private IReloadedHooks _hooks;

        private IFunctionPtr<FMallocInternal_Free>? _freeInternal;
        private IFunctionPtr<FMallocInternal_Malloc>? _mallocInternal;
        private IFunctionPtr<FMallocInternal_Realloc>? _reallocInternal;
        private IFunctionPtr<FMallocInternal_GetAllocSize>? _getMallocSizeInternal;

        public unsafe MemoryMethods(IStartupScanner scans, long baseAddress, ILogger logger, IReloadedHooks hooks, string modName)
        {
            _baseAddress = baseAddress;
            _scanner = scans;
            _logger = logger;
            _hooks = hooks;

            _scanner.AddMainModuleScan(GMalloc_SIG, addr =>
            {
                if (!addr.Found)
                {
                    _logger.WriteLine($"[{modName}] Couldn't find location for gMalloc, stuff will break :(");
                    return;
                }
                var realAddr = _baseAddress + addr.Offset;
                gMallocPtr = (nint*)GetGlobalAddress((nint)(realAddr + 3));
                _logger.WriteLine($"[{modName}] Found gMalloc at {(nint)gMallocPtr:X}");
            });
        }

        public static unsafe nuint GetGlobalAddress(nint ptrAddress) => (nuint)((*(int*)ptrAddress) + ptrAddress + 4);

        // TODO: GCreateMalloc (although that probably shouldn't be needed,
        // are you really gonna be the first caller of the memory allocator?)
        public unsafe void FMemory_Free<TType>(TType* ptr) where TType : unmanaged => FMemory_Free((nint)ptr);
        public unsafe void FMemory_Free(nint ptr)
        {
            if (_freeInternal == null)
                _freeInternal = _hooks.CreateFunctionPtr<FMallocInternal_Free>(**(ulong**)gMallocPtr + 0x30);
            _freeInternal.GetDelegate()(*gMallocPtr, ptr);
            
        }
        public unsafe delegate void FMallocInternal_Free(nint gMalloc, nint ptr);

        public unsafe nint FMemory_Malloc<TType>(uint alignment) where TType : unmanaged => FMemory_Malloc(sizeof(TType), alignment);
        public unsafe nint FMemory_Malloc(nint size, uint alignment)
        {
            if (_mallocInternal == null)
            {
                _logger.WriteLine($"Logger funcptr is null: {*gMallocPtr:X}, entry @ {**(ulong**)gMallocPtr + 0x10}");
                _mallocInternal = _hooks.CreateFunctionPtr<FMallocInternal_Malloc>(**(ulong**)gMallocPtr + 0x10);
            }
            return _mallocInternal.GetDelegate()(*gMallocPtr, size, alignment);
        }
        public unsafe delegate nint FMallocInternal_Malloc(nint gMalloc, nint size, uint alignment);
        public unsafe nint FMemory_Realloc(nint ptr, nint size, uint alignment)
        {
            if (_reallocInternal == null)
                _reallocInternal = _hooks.CreateFunctionPtr<FMallocInternal_Realloc>(**(ulong**)gMallocPtr + 0x20);
            return _reallocInternal.GetDelegate()(*gMallocPtr, ptr, size, alignment);
        }
        public unsafe delegate nint FMallocInternal_Realloc(nint gMalloc, nint ptr, nint size, uint alignment);

        public unsafe nint FMemory_GetAllocSize(nint ptr)
        {
            if (_getMallocSizeInternal == null)
                _getMallocSizeInternal = _hooks.CreateFunctionPtr<FMallocInternal_GetAllocSize>(**(ulong**)gMallocPtr + 0x40);
            nint retSize = 0;
            nint finalSize = 0;
            var result = _getMallocSizeInternal.GetDelegate()(*gMallocPtr, ptr, ref retSize);
            if (result != 0) finalSize = retSize;
            return finalSize;
        }
        public unsafe delegate char FMallocInternal_GetAllocSize(nint gMalloc, nint ptr, ref nint size);
    }
}