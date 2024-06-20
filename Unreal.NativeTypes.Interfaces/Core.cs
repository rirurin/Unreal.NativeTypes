using System.Runtime.InteropServices;
namespace Unreal.NativeTypes.Interfaces;

#pragma warning disable CS1591

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public unsafe struct TArray<T> where T : unmanaged
{
    public T* allocator_instance;
    public int arr_num;
    public int arr_max;

    public T* GetRef(int index) // for arrays of type TArray<FValueType>
    {
        if (index < 0 || index >= arr_num) return null;
        return &allocator_instance[index];
    }

    public V* Get<V>(int index) where V : unmanaged // for arrays of type TArray<FValueType*>
    {
        if (index < 0 || index >= arr_num) return null;
        return *(V**)&allocator_instance[index];
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TMapElement<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>
    where ValueType : unmanaged
{
    public KeyType Key;
    public ValueType Value;
    long Unk;
}
//[StructLayout(LayoutKind.Explicit, Size = 0x50)] // inherits from TSortableMapBase
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TMap<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>
    where ValueType : unmanaged
{
    public TMapElement<KeyType, ValueType>* elements;
    public int mapNum;
    public int mapMax;
    public ValueType* TryGet(KeyType key)
    {
        if (mapNum == 0 || elements == null) return null;
        ValueType* value = null;
        for (int i = 0; i < mapNum; i++)
        {
            var currElem = &elements[i];
            if (currElem->Key.Equals(key))
            {
                value = &currElem->Value;
                break;
            }
        }
        return value;
    }
    public ValueType* GetByIndex(int idx)
    {
        if (idx < 0 || idx > mapNum) return null;
        return &elements[idx].Value;
    }
}

public interface IMapHashable
{
    public uint GetTypeHash();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TMapElementHashable<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>, IMapHashable
    where ValueType : unmanaged
{
    public KeyType Key;
    public ValueType Value;
    public int HashNextId;
    public int HashIndex;
}
public unsafe class TMapHashable<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>, IMapHashable
    where ValueType : unmanaged
{
    // Each instance assumes that values are fixed at particular addresses from init onwards
    public TArray<TMapElementHashable<KeyType, ValueType>>* Elements;
    public int** Hashes;
    public uint* HashSize;
    public TMapHashable(nint ptr, nint hashArrOffset, nint hashSizeOffset) // Address of start of TMap struct (e.g &class_instance->func_map)
    {
        Elements = (TArray<TMapElementHashable<KeyType, ValueType>>*)ptr;
        Hashes = (int**)(ptr + hashArrOffset);
        HashSize = (uint*)(ptr + hashSizeOffset);
    }

    public ValueType* GetByIndex(int idx)
    {
        if (idx < 0 || idx > Elements->arr_num) return null;
        return &Elements->allocator_instance[idx].Value;
    }

    public ValueType* TryGetLinear(KeyType key)
    {
        if (Elements->arr_num == 0 || Elements->allocator_instance == null) return null;
        ValueType* value = null;
        for (int i = 0; i < Elements->arr_num; i++)
        {
            var currElem = &Elements->allocator_instance[i];
            if (currElem->Key.Equals(key))
            {
                value = &currElem->Value;
                break;
            }
        }
        return value;
    }

    public ValueType* TryGetByHash(KeyType key)
    {
        ValueType* value = null;
        // Hash alloc doesn't exist for single element maps,
        // so fallback to linear search
        if (*Hashes == null) return TryGetLinear(key);
        var elementTarget = (*Hashes)[key.GetTypeHash() & (*HashSize - 1)];
        while (elementTarget != -1)
        {
            if (Elements->allocator_instance[elementTarget].Key.Equals(key))
            {
                value = &Elements->allocator_instance[elementTarget].Value;
                break;
            }
            elementTarget = Elements->allocator_instance[elementTarget].HashNextId;
        }
        return value;
    }
}

[StructLayout(LayoutKind.Explicit, Size = 0x8)]
public unsafe struct FName : IEquatable<FName>, IMapHashable
{
    [FieldOffset(0x0)] public uint pool_location;
    [FieldOffset(0x4)] public uint field04;
    public bool Equals(FName other) => pool_location == other.pool_location;

    public uint GetTypeHash()
    {
        uint num = pool_location >> 16;
        uint num2 = pool_location & 0xFFFFu;
        return (num << 19) + num + (num2 << 16) + num2 + (num2 >> 4) + field04;
    }
}

public enum EFindType
{
    FNAME_Find,
    FNAME_Add,
    FNAME_Replace_Not_Safe_For_Threading
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FString : IEquatable<FString>
{
    public TArray<nint> text;
    public override string ToString() => Marshal.PtrToStringUni((nint)text.allocator_instance, text.arr_num - 1);
    public bool Equals(FString other) => ToString().Equals(other.ToString());
}

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public unsafe struct FFieldObjectUnion
{
    public IntPtr field_or_obj;
    public bool b_is_uobj;
}
[StructLayout(LayoutKind.Explicit, Size = 0x40)]
public unsafe struct FFieldClass
{
    [FieldOffset(0x0)] public FName name;
    [FieldOffset(0x20)] public FFieldClass* super;
    [FieldOffset(0x28)] public FField* default_obj;
    [FieldOffset(0x30)] public IntPtr ctor; // [PropertyName]::Construct (e.g for ArrayProperty, this would be FArrayProperty::Construct)
}

[StructLayout(LayoutKind.Sequential, Size = 0x38)]
public unsafe struct FField
{
    public IntPtr _vtable; // @ 0x0
    public FFieldClass* class_private; // @ 0x8
    public FFieldObjectUnion owner; // @ 0x10
    public FField* next; // @ 0x20
    public FName name_private; // @ 0x28
    public EObjectFlags flags_private; // @ 0x30

}
public enum EPropertyFlags : ulong
{
    CPF_None = 0,

    CPF_Edit = 0x0000000000000001,  //< Property is user-settable in the editor.
    CPF_ConstParm = 0x0000000000000002, //< This is a constant function parameter
    CPF_BlueprintVisible = 0x0000000000000004,  //< This property can be read by blueprint code
    CPF_ExportObject = 0x0000000000000008,  //< Object can be exported with actor.
    CPF_BlueprintReadOnly = 0x0000000000000010, //< This property cannot be modified by blueprint code
    CPF_Net = 0x0000000000000020,   //< Property is relevant to network replication.
    CPF_EditFixedSize = 0x0000000000000040, //< Indicates that elements of an array can be modified, but its size cannot be changed.
    CPF_Parm = 0x0000000000000080,  //< Function/When call parameter.
    CPF_OutParm = 0x0000000000000100,   //< Value is copied out after function call.
    CPF_ZeroConstructor = 0x0000000000000200,   //< memset is fine for construction
    CPF_ReturnParm = 0x0000000000000400,    //< Return value.
    CPF_DisableEditOnTemplate = 0x0000000000000800, //< Disable editing of this property on an archetype/sub-blueprint
    CPF_NonNullable = 0x0000000000001000,   //< Object property can never be null
    CPF_Transient = 0x0000000000002000, //< Property is transient: shouldn't be saved or loaded, except for Blueprint CDOs.
    CPF_Config = 0x0000000000004000,    //< Property should be loaded/saved as permanent profile.
    //CPF_								= 0x0000000000008000,	//< 
    CPF_DisableEditOnInstance = 0x0000000000010000, //< Disable editing on an instance of this class
    CPF_EditConst = 0x0000000000020000, //< Property is uneditable in the editor.
    CPF_GlobalConfig = 0x0000000000040000,  //< Load config from base class, not subclass.
    CPF_InstancedReference = 0x0000000000080000,    //< Property is a component references.
    //CPF_								= 0x0000000000100000,	//<
    CPF_DuplicateTransient = 0x0000000000200000,    //< Property should always be reset to the default value during any type of duplication (copy/paste, binary duplication, etc.)
    //CPF_								= 0x0000000000400000,	//< 
    //CPF_    							= 0x0000000000800000,	//< 
    CPF_SaveGame = 0x0000000001000000,  //< Property should be serialized for save games, this is only checked for game-specific archives with ArIsSaveGame
    CPF_NoClear = 0x0000000002000000,   //< Hide clear (and browse) button.
    //CPF_  							= 0x0000000004000000,	//<
    CPF_ReferenceParm = 0x0000000008000000, //< Value is passed by reference; CPF_OutParam and CPF_Param should also be set.
    CPF_BlueprintAssignable = 0x0000000010000000,   //< MC Delegates only.  Property should be exposed for assigning in blueprint code
    CPF_Deprecated = 0x0000000020000000,    //< Property is deprecated.  Read it from an archive, but don't save it.
    CPF_IsPlainOldData = 0x0000000040000000,    //< If this is set, then the property can be memcopied instead of CopyCompleteValue / CopySingleValue
    CPF_RepSkip = 0x0000000080000000,   //< Not replicated. For non replicated properties in replicated structs 
    CPF_RepNotify = 0x0000000100000000, //< Notify actors when a property is replicated
    CPF_Interp = 0x0000000200000000,    //< interpolatable property for use with cinematics
    CPF_NonTransactional = 0x0000000400000000,  //< Property isn't transacted
    CPF_EditorOnly = 0x0000000800000000,    //< Property should only be loaded in the editor
    CPF_NoDestructor = 0x0000001000000000,  //< No destructor
    //CPF_								= 0x0000002000000000,	//<
    CPF_AutoWeak = 0x0000004000000000,  //< Only used for weak pointers, means the export type is autoweak
    CPF_ContainsInstancedReference = 0x0000008000000000,    //< Property contains component references.
    CPF_AssetRegistrySearchable = 0x0000010000000000,   //< asset instances will add properties with this flag to the asset registry automatically
    CPF_SimpleDisplay = 0x0000020000000000, //< The property is visible by default in the editor details view
    CPF_AdvancedDisplay = 0x0000040000000000,   //< The property is advanced and not visible by default in the editor details view
    CPF_Protected = 0x0000080000000000, //< property is protected from the perspective of script
    CPF_BlueprintCallable = 0x0000100000000000, //< MC Delegates only.  Property should be exposed for calling in blueprint code
    CPF_BlueprintAuthorityOnly = 0x0000200000000000,    //< MC Delegates only.  This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly.
    CPF_TextExportTransient = 0x0000400000000000,   //< Property shouldn't be exported to text format (e.g. copy/paste)
    CPF_NonPIEDuplicateTransient = 0x0000800000000000,  //< Property should only be copied in PIE
    CPF_ExposeOnSpawn = 0x0001000000000000, //< Property is exposed on spawn
    CPF_PersistentInstance = 0x0002000000000000,    //< A object referenced by the property is duplicated like a component. (Each actor should have an own instance.)
    CPF_UObjectWrapper = 0x0004000000000000,    //< Property was parsed as a wrapper class like TSubclassOf<T>, FScriptInterface etc., rather than a USomething*
    CPF_HasGetValueTypeHash = 0x0008000000000000,   //< This property can generate a meaningful hash value.
    CPF_NativeAccessSpecifierPublic = 0x0010000000000000,   //< Public native access specifier
    CPF_NativeAccessSpecifierProtected = 0x0020000000000000,    //< Protected native access specifier
    CPF_NativeAccessSpecifierPrivate = 0x0040000000000000,  //< Private native access specifier
    CPF_SkipSerialization = 0x0080000000000000, //< Property shouldn't be serialized, can still be exported to text
};

[StructLayout(LayoutKind.Sequential, Size = 0x78)]
public unsafe struct FProperty
// FInt8Property, FInt16Property, FIntProperty, FInt64Property
// FUint8Property, FUint16Property, FUintProperty, FUint64Property
// FFloatProperty, FDoubleProperty, FNameProperty, FStrProperty
{
    public FField _super; // @ 0x0
    public int array_dim; // @ 0x38
    public int element_size; // @ 0x3c
    public EPropertyFlags property_flags; // @ 0x40
    public ushort rep_index; // @ 0x48
    public byte blueprint_rep_cond; // @ 0x4a
    public byte Field4B; // @ 0x4b
    public int offset_internal; // @ 0x4c
    public FName rep_notify_func; // @ 0x50
    public FProperty* prop_link_next; // @ 0x58
    public FProperty* next_ref; // @ 0x60
    public FProperty* dtor_link_next; // @ 0x68
    public FProperty* post_ct_link_next; // @ 0x70
}

[StructLayout(LayoutKind.Explicit, Size = 0x2)]
public unsafe struct FNamePoolString
{
    // Flags:
    // bIsWide : 1;
    // probeHashBits : 5;
    // Length : 10;
    // Get Length: flags >> 6
    [FieldOffset(0x0)] public short flags;
    public string GetString() { fixed (FNamePoolString* self = &this) return Marshal.PtrToStringAnsi((IntPtr)(self + 1), flags >> 6); }
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public unsafe struct FNamePool
{
    [FieldOffset(0x8)] public uint pool_count;
    [FieldOffset(0xc)] public uint name_count;
    public IntPtr GetPool(uint pool_idx) { fixed (FNamePool* self = &this) return *((IntPtr*)(self + 1) + pool_idx); }
    public string GetString(FName name) => GetString(name.pool_location);
    public string GetString(uint pool_loc)
    {
        fixed (FNamePool* self = &this)
        {
            // Get appropriate pool
            IntPtr ptr = GetPool(pool_loc >> 0x10); // 0xABB2B - pool 0xA
            // Go to name entry in pool
            ptr += (nint)((pool_loc & 0xFFFF) * 2);
            return ((FNamePoolString*)ptr)->GetString();
        }
    }

}

[StructLayout(LayoutKind.Explicit, Size = 0x260)]
public unsafe struct FUObjectHashTables
{
    // 0x0: Critical Section
    [FieldOffset(0x28)] public TMap<int, nint> Hashes; // TMap<int, FHashBucket>
    [FieldOffset(0x78)] public TMap<int, nint> HashesOuter; // TMap<int, FHashBucket>
}

public unsafe struct HashablePointer : IMapHashable, IEquatable<HashablePointer>
{
    public nint Ptr;
    public HashablePointer(nint ptr) { Ptr = ptr; }
    public uint GetTypeHash() // FUN_140904980
    {
        uint iVar4 = (uint)(Ptr >> 4);
        uint uVar3 = 0x9e3779b9U - iVar4 ^ iVar4 << 8;
        uint uVar1 = (uint)-(uVar3 + iVar4) ^ uVar3 >> 0xd;
        uint uVar5 = (iVar4 - uVar3) - uVar1 ^ uVar1 >> 0xc;
        uVar3 = (uVar3 - uVar5) - uVar1 ^ uVar5 << 0x10;
        uVar1 = (uVar1 - uVar3) - uVar5 ^ uVar3 >> 5;
        uVar5 = (uVar5 - uVar3) - uVar1 ^ uVar1 >> 3;
        uVar3 = (uVar3 - uVar5) - uVar1 ^ uVar5 << 10;
        uint ret = ((uVar1 - uVar3) - uVar5) ^ (uVar3 >> 0xf);
        return ret;
    }
    public bool Equals(HashablePointer other) => Ptr == other.Ptr;
}
public unsafe struct HashableInt : IMapHashable, IEquatable<HashableInt>
{
    public int Value;
    public HashableInt(int value) { Value = value; }
    public uint GetTypeHash() => (uint)Value;
    public bool Equals(HashableInt other) => other.Value == Value;
}
public static class TypeExtensions
{
    public static HashablePointer AsHashable(this nint ptr) => new HashablePointer(ptr);
    public static HashableInt AsHashable(this int val) => new HashableInt(val);
    public static uint HashCombine(uint a, uint b)
    { // FUN_141cbc830
        uint uVar1 = a - b ^ b >> 0xd;
        uint uVar3 = (uint)(-0x61c88647 - uVar1) - b ^ uVar1 << 8;
        uint uVar2 = (b - uVar3) - uVar1 ^ uVar3 >> 0xd;
        uVar1 = (uVar1 - uVar3) - uVar2 ^ uVar2 >> 0xc;
        uVar3 = (uVar3 - uVar1) - uVar2 ^ uVar1 << 0x10;
        uVar2 = (uVar2 - uVar3) - uVar1 ^ uVar3 >> 5;
        uVar1 = (uVar1 - uVar3) - uVar2 ^ uVar2 >> 3;
        uVar3 = (uVar3 - uVar1) - uVar2 ^ uVar1 << 10;
        return (uVar2 - uVar3) - uVar1 ^ uVar3 >> 0xf;
    }
}