using System.Runtime.InteropServices;
namespace Unreal.NativeTypes.Interfaces;

#pragma warning disable CS1591

public enum EObjectFlags : uint
{
    Public = 1 << 0x0,
    Standalone = 1 << 0x1,
    MarkAsNative = 1 << 0x2,
    Transactional = 1 << 0x3,
    ClassDefaultObject = 1 << 0x4,
    ArchetypeObject = 1 << 0x5,
    Transient = 1 << 0x6,
    MarkAsRootSet = 1 << 0x7,
    TagGarbageTemp = 1 << 0x8,
    NeedInitialization = 1 << 0x9,
    NeedLoad = 1 << 0xa,
    KeepForCooker = 1 << 0xb,
    NeedPostLoad = 1 << 0xc,
    NeedPostLoadSubobjects = 1 << 0xd,
    NewerVersionExists = 1 << 0xe,
    BeginDestroyed = 1 << 0xf,
    FinishDestroyed = 1 << 0x10,
    BeingRegenerated = 1 << 0x11,
    DefaultSubObject = 1 << 0x12,
    WasLoaded = 1 << 0x13,
    TextExportTransient = 1 << 0x14,
    LoadCompleted = 1 << 0x15,
    InheritableComponentTemplate = 1 << 0x16,
    DuplicateTransient = 1 << 0x17,
    StrongRefOnFrame = 1 << 0x18,
    NonPIEDuplicateTransient = 1 << 0x19,
    Dynamic = 1 << 0x1a,
    WillBeLoaded = 1 << 0x1b,
    HasExternalPackage = 1 << 0x1c,
    PendingKill = 1 << 0x1d,
    Garbage = 1 << 0x1e,
    AllocatedInSharedPage = (uint)1 << 0x1f
}
public enum EInternalObjectFlags : uint
{
    None = 0,
    LoaderImport = 1 << 20, //< Object is ready to be imported by another package during loading
    Garbage = 1 << 21, //< Garbage from logical point of view and should not be referenced. This flag is mirrored in EObjectFlags as RF_Garbage for performance
    PersistentGarbage = 1 << 22, //< Same as above but referenced through a persistent reference so it can't be GC'd
    ReachableInCluster = 1 << 23, //< External reference to object in cluster exists
    ClusterRoot = 1 << 24, //< Root of a cluster
    Native = 1 << 25, //< Native (UClass only). 
    Async = 1 << 26, //< Object exists only on a different thread than the game thread.
    AsyncLoading = 1 << 27, //< Object is being asynchronously loaded.
    Unreachable = 1 << 28, //< Object is not reachable on the object graph.
    PendingKill = 1 << 29, //< Objects that are pending destruction (invalid for gameplay but valid objects). This flag is mirrored in EObjectFlags as RF_PendingKill for performance
    RootSet = 1 << 30, //< Object will not be garbage collected, even if unreferenced.
    PendingConstruction = (uint)1 << 31, //< Object didn't have its class constructor called yet (only the UObject one to initialize its most basic member
};
[StructLayout(LayoutKind.Sequential, Size = 0x28)]
public unsafe struct UObject
{
    public IntPtr _vtable; // @ 0x0
    public EObjectFlags ObjectFlags; // @ 0x8
    public uint InternalIndex; // @ 0xc
    public UClass* ClassPrivate; // @ 0x10 Type of this object. Used for reflection
    public FName NamePrivate; // @ 0x18
    public UObject* OuterPrivate; // @ 0x20 Object that is containing this object
}

// Class data
[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe struct UField
{
    [FieldOffset(0x0)] public UObject _super;
    [FieldOffset(0x28)] public UField* next;
}
[StructLayout(LayoutKind.Explicit, Size = 0xb0)]
public unsafe struct UStruct
{
    [FieldOffset(0x0)] public UField _super;
    [FieldOffset(0x40)] public UStruct* super_struct;
    [FieldOffset(0x48)] public UField* children; // anything not a type field (e.g a class method) - beginning of linked list
    [FieldOffset(0x50)] public FField* child_properties; // the data model - beginning of linked list
    [FieldOffset(0x58)] public int properties_size;
    [FieldOffset(0x5c)] public int min_alignment;
    [FieldOffset(0x60)] public TArray<byte> Script;
    [FieldOffset(0x70)] public FProperty* prop_link;
    [FieldOffset(0x78)] public FProperty* ref_link;
    [FieldOffset(0x80)] public FProperty* dtor_link;
    [FieldOffset(0x88)] public FProperty* post_ct_link;
}
[StructLayout(LayoutKind.Explicit, Size = 0xc0)]
public unsafe struct UScriptStruct
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public uint flags;
    [FieldOffset(0xb4)] public bool b_prepare_cpp_struct_ops_completed;
    [FieldOffset(0xb8)] public IntPtr cpp_struct_ops;
}

public enum EFunctionFlags : uint
{
    // Function flags.
    FUNC_None = 0x00000000,

    FUNC_Final = 0x00000001,    // Function is final (prebindable, non-overridable function).
    FUNC_RequiredAPI = 0x00000002,  // Indicates this function is DLL exported/imported.
    FUNC_BlueprintAuthorityOnly = 0x00000004,   // Function will only run if the object has network authority
    FUNC_BlueprintCosmetic = 0x00000008,   // Function is cosmetic in nature and should not be invoked on dedicated servers
                                           // FUNC_				= 0x00000010,   // unused.
                                           // FUNC_				= 0x00000020,   // unused.
    FUNC_Net = 0x00000040,   // Function is network-replicated.
    FUNC_NetReliable = 0x00000080,   // Function should be sent reliably on the network.
    FUNC_NetRequest = 0x00000100,   // Function is sent to a net service
    FUNC_Exec = 0x00000200, // Executable from command line.
    FUNC_Native = 0x00000400,   // Native function.
    FUNC_Event = 0x00000800,   // Event function.
    FUNC_NetResponse = 0x00001000,   // Function response from a net service
    FUNC_Static = 0x00002000,   // Static function.
    FUNC_NetMulticast = 0x00004000, // Function is networked multicast Server -> All Clients
    FUNC_UbergraphFunction = 0x00008000,   // Function is used as the merge 'ubergraph' for a blueprint, only assigned when using the persistent 'ubergraph' frame
    FUNC_MulticastDelegate = 0x00010000,    // Function is a multi-cast delegate signature (also requires FUNC_Delegate to be set!)
    FUNC_Public = 0x00020000,   // Function is accessible in all classes (if overridden, parameters must remain unchanged).
    FUNC_Private = 0x00040000,  // Function is accessible only in the class it is defined in (cannot be overridden, but function name may be reused in subclasses.  IOW: if overridden, parameters don't need to match, and Super.Func() cannot be accessed since it's private.)
    FUNC_Protected = 0x00080000,    // Function is accessible only in the class it is defined in and subclasses (if overridden, parameters much remain unchanged).
    FUNC_Delegate = 0x00100000, // Function is delegate signature (either single-cast or multi-cast, depending on whether FUNC_MulticastDelegate is set.)
    FUNC_NetServer = 0x00200000,    // Function is executed on servers (set by replication code if passes check)
    FUNC_HasOutParms = 0x00400000,  // function has out (pass by reference) parameters
    FUNC_HasDefaults = 0x00800000,  // function has structs that contain defaults
    FUNC_NetClient = 0x01000000,    // function is executed on clients
    FUNC_DLLImport = 0x02000000,    // function is imported from a DLL
    FUNC_BlueprintCallable = 0x04000000,    // function can be called from blueprint code
    FUNC_BlueprintEvent = 0x08000000,   // function can be overridden/implemented from a blueprint
    FUNC_BlueprintPure = 0x10000000,    // function can be called from blueprint code, and is also pure (produces no side effects). If you set this, you should set FUNC_BlueprintCallable as well.
    FUNC_EditorOnly = 0x20000000,   // function can only be called from an editor scrippt.
    FUNC_Const = 0x40000000,    // function can be called from blueprint code, and only reads state (never writes state)
    FUNC_NetValidate = 0x80000000,  // function must supply a _Validate implementation
};

[StructLayout(LayoutKind.Explicit, Size = 0xe0)]
public unsafe struct UFunction
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public EFunctionFlags func_flags;
    [FieldOffset(0xb4)] public byte num_params;
    [FieldOffset(0xb6)] public ushort params_size;
    [FieldOffset(0xc0)] public FProperty* first_prop_to_init;
    [FieldOffset(0xc8)] public UFunction* event_graph_func;
    [FieldOffset(0xd8)] public IntPtr exec_func_ptr;
}

public enum EClassFlags : uint
{
    /* No Flags */
    CLASS_None = 0x00000000u,
    /* Class is abstract and can't be instantiated directly. */
    CLASS_Abstract = 0x00000001u,
    /* Save object configuration only to Default INIs, never to local INIs. Must be combined with CLASS_Config */
    CLASS_DefaultConfig = 0x00000002u,
    /* Load object configuration at construction time. */
    CLASS_Config = 0x00000004u,
    /* This object type can't be saved; null it out at save time. */
    CLASS_Transient = 0x00000008u,
    /* This object type may not be available in certain context. (i.e. game runtime or in certain configuration). Optional class data is saved separately to other object types. (i.e. might use sidecar files) */
    CLASS_Optional = 0x00000010u,
    /* */
    CLASS_MatchedSerializers = 0x00000020u,
    /* Indicates that the config settings for this class will be saved to Project/User*.ini (similar to CLASS_GlobalUserConfig) */
    CLASS_ProjectUserConfig = 0x00000040u,
    /* Class is a native class - native interfaces will have CLASS_Native set, but not RF_MarkAsNative */
    CLASS_Native = 0x00000080u,
    /* Don't export to C++ header. */
    CLASS_NoExport = 0x00000100u,
    /* Do not allow users to create in the editor. */
    CLASS_NotPlaceable = 0x00000200u,
    /* Handle object configuration on a per-object basis, rather than per-class. */
    CLASS_PerObjectConfig = 0x00000400u,

    /* Whether SetUpRuntimeReplicationData still needs to be called for this class */
    CLASS_ReplicationDataIsSetUp = 0x00000800u,

    /* Class can be constructed from editinline New button. */
    CLASS_EditInlineNew = 0x00001000u,
    /* Display properties in the editor without using categories. */
    CLASS_CollapseCategories = 0x00002000u,
    /* Class is an interface **/
    CLASS_Interface = 0x00004000u,
    /*  Do not export a constructor for this class, assuming it is in the cpptext **/
    CLASS_CustomConstructor = 0x00008000u,
    /* all properties and functions in this class are const and should be exported as const */
    CLASS_Const = 0x00010000u,

    /* Class flag indicating objects of this class need deferred dependency loading */
    CLASS_NeedsDeferredDependencyLoading = 0x00020000u,

    /* Indicates that the class was created from blueprint source material */
    CLASS_CompiledFromBlueprint = 0x00040000u,

    /* Indicates that only the bare minimum bits of this class should be DLL exported/imported */
    CLASS_MinimalAPI = 0x00080000u,

    /* Indicates this class must be DLL exported/imported (along with all of it's members) */
    CLASS_RequiredAPI = 0x00100000u,

    /* Indicates that references to this class default to instanced. Used to be subclasses of UComponent, but now can be any UObject */
    CLASS_DefaultToInstanced = 0x00200000u,

    /* Indicates that the parent token stream has been merged with ours. */
    CLASS_TokenStreamAssembled = 0x00400000u,
    /* Class has component properties. */
    CLASS_HasInstancedReference = 0x00800000u,
    /* Don't show this class in the editor class browser or edit inline new menus. */
    CLASS_Hidden = 0x01000000u,
    /* Don't save objects of this class when serializing */
    CLASS_Deprecated = 0x02000000u,
    /* Class not shown in editor drop down for class selection */
    CLASS_HideDropDown = 0x04000000u,
    /* Class settings are saved to <AppData>/..../Blah.ini (as opposed to CLASS_DefaultConfig) */
    CLASS_GlobalUserConfig = 0x08000000u,
    /* Class was declared directly in C++ and has no boilerplate generated by UnrealHeaderTool */
    CLASS_Intrinsic = 0x10000000u,
    /* Class has already been constructed (maybe in a previous DLL version before hot-reload). */
    CLASS_Constructed = 0x20000000u,
    /* Indicates that object configuration will not check against ini base/defaults when serialized */
    CLASS_ConfigDoNotCheckDefaults = 0x40000000u,
    /* Class has been consigned to oblivion as part of a blueprint recompile, and a newer version currently exists. */
    CLASS_NewerVersionExists = 0x80000000u,
};
public enum EClassCastFlags : ulong
{
    CASTCLASS_None = 0x0000000000000000,

    CASTCLASS_UField = 0x0000000000000001,
    CASTCLASS_FInt8Property = 0x0000000000000002,
    CASTCLASS_UEnum = 0x0000000000000004,
    CASTCLASS_UStruct = 0x0000000000000008,
    CASTCLASS_UScriptStruct = 0x0000000000000010,
    CASTCLASS_UClass = 0x0000000000000020,
    CASTCLASS_FByteProperty = 0x0000000000000040,
    CASTCLASS_FIntProperty = 0x0000000000000080,
    CASTCLASS_FFloatProperty = 0x0000000000000100,
    CASTCLASS_FUInt64Property = 0x0000000000000200,
    CASTCLASS_FClassProperty = 0x0000000000000400,
    CASTCLASS_FUInt32Property = 0x0000000000000800,
    CASTCLASS_FInterfaceProperty = 0x0000000000001000,
    CASTCLASS_FNameProperty = 0x0000000000002000,
    CASTCLASS_FStrProperty = 0x0000000000004000,
    CASTCLASS_FProperty = 0x0000000000008000,
    CASTCLASS_FObjectProperty = 0x0000000000010000,
    CASTCLASS_FBoolProperty = 0x0000000000020000,
    CASTCLASS_FUInt16Property = 0x0000000000040000,
    CASTCLASS_UFunction = 0x0000000000080000,
    CASTCLASS_FStructProperty = 0x0000000000100000,
    CASTCLASS_FArrayProperty = 0x0000000000200000,
    CASTCLASS_FInt64Property = 0x0000000000400000,
    CASTCLASS_FDelegateProperty = 0x0000000000800000,
    CASTCLASS_FNumericProperty = 0x0000000001000000,
    CASTCLASS_FMulticastDelegateProperty = 0x0000000002000000,
    CASTCLASS_FObjectPropertyBase = 0x0000000004000000,
    CASTCLASS_FWeakObjectProperty = 0x0000000008000000,
    CASTCLASS_FLazyObjectProperty = 0x0000000010000000,
    CASTCLASS_FSoftObjectProperty = 0x0000000020000000,
    CASTCLASS_FTextProperty = 0x0000000040000000,
    CASTCLASS_FInt16Property = 0x0000000080000000,
    CASTCLASS_FDoubleProperty = 0x0000000100000000,
    CASTCLASS_FSoftClassProperty = 0x0000000200000000,
    CASTCLASS_UPackage = 0x0000000400000000,
    CASTCLASS_ULevel = 0x0000000800000000,
    CASTCLASS_AActor = 0x0000001000000000,
    CASTCLASS_APlayerController = 0x0000002000000000,
    CASTCLASS_APawn = 0x0000004000000000,
    CASTCLASS_USceneComponent = 0x0000008000000000,
    CASTCLASS_UPrimitiveComponent = 0x0000010000000000,
    CASTCLASS_USkinnedMeshComponent = 0x0000020000000000,
    CASTCLASS_USkeletalMeshComponent = 0x0000040000000000,
    CASTCLASS_UBlueprint = 0x0000080000000000,
    CASTCLASS_UDelegateFunction = 0x0000100000000000,
    CASTCLASS_UStaticMeshComponent = 0x0000200000000000,
    CASTCLASS_FMapProperty = 0x0000400000000000,
    CASTCLASS_FSetProperty = 0x0000800000000000,
    CASTCLASS_FEnumProperty = 0x0001000000000000,
    CASTCLASS_USparseDelegateFunction = 0x0002000000000000,
    CASTCLASS_FMulticastInlineDelegateProperty = 0x0004000000000000,
    CASTCLASS_FMulticastSparseDelegateProperty = 0x0008000000000000,
    CASTCLASS_FFieldPathProperty = 0x0010000000000000,
};
[StructLayout(LayoutKind.Explicit, Size = 0x230)]
public unsafe struct UClass
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public IntPtr class_ctor; // InternalConstructor<class_UClassName> => UClassName::UClassName
    [FieldOffset(0xb8)] public IntPtr class_vtable_helper_ctor_caller;
    [FieldOffset(0xc0)] public IntPtr class_add_ref_objects;
    [FieldOffset(0xc8)] public uint class_status; // ClassUnique : 31, bCooked : 1
    [FieldOffset(0xcc)] public EClassFlags class_flags;
    [FieldOffset(0xd0)] public EClassCastFlags class_cast_flags;
    [FieldOffset(0xd8)] public UClass* class_within; // type of object containing the current object
    [FieldOffset(0xe0)] public UObject* class_gen_by;
    [FieldOffset(0xe8)] public FName class_conf_name;
    [FieldOffset(0x100)] public TArray<UField> net_fields;
    [FieldOffset(0x118)] public UObject* class_default_obj; // Default object of type described in UClass instance
    //[FieldOffset(0x130)] public TMap func_map;
    //[FieldOffset(0x180)] public TMap super_func_map;
    [FieldOffset(0x1d8)] public TArray<IntPtr> interfaces;
    [FieldOffset(0x220)] public TArray<FNativeFunctionLookup> native_func_lookup;
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public unsafe struct FNativeFunctionLookup
{
    [FieldOffset(0x0)] FName name;
    [FieldOffset(0x8)] /*FNativeFuncPtr*/ nint Pointer;
}

public unsafe delegate void FNativeFuncPtr(UObject* context, nuint stack, nuint returnValue);

public unsafe struct UEnumEntry // TTuple<FName, long>
{
    public FName name;
    public long value; // Size : 0x10
}

[StructLayout(LayoutKind.Explicit, Size = 0x60)]
public unsafe struct UEnum
{
    [FieldOffset(0x0)] public UField _super;
    [FieldOffset(0x30)] public FString cpp_type;
    [FieldOffset(0x40)] public TArray<UEnumEntry> entries;
    [FieldOffset(0x58)] public IntPtr enum_disp_name_fn;
}

[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe struct FUObjectArray
{
    [FieldOffset(0x0)] public int ObjFirstGCIndex;
    [FieldOffset(0x4)] public int ObjLastNonGCIndex;
    [FieldOffset(0x10)] public FUObjectItem** Objects;
    [FieldOffset(0x24)] public int NumElements;
    // Max number of elements is 0x210000 (2162688)
    // Each chunk can hold 0x10000 elements
    // Max number of chunks is 0x21 (33)
    [FieldOffset(0x2c)] public int NumChunks;
    // 0x30: Critical Section
}
[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public unsafe struct FUObjectItem
{
    [FieldOffset(0x0)] public UObject* Object;
    [FieldOffset(0x8)] public EInternalObjectFlags Flags;
}
// For StaticConstructObject_Internal
[StructLayout(LayoutKind.Explicit, Size = 0x40)]
public unsafe struct FStaticConstructObjectParameters
{
    [FieldOffset(0x0)] public UClass* Class; // Type Info
    [FieldOffset(0x8)] public UObject* Outer; // The created object will be a child of this object
    [FieldOffset(0x10)] public FName Name;
    [FieldOffset(0x18)] public EObjectFlags SetFlags;
    [FieldOffset(0x1c)] public EInternalObjectFlags InternalSetFlags;
    [FieldOffset(0x20)] public byte bCopyTransientsFromClassDefaults;
    [FieldOffset(0x21)] public byte bAssumeTemplateIsArchetype;
    [FieldOffset(0x28)] public UObject* Template;
    [FieldOffset(0x30)] public IntPtr InstanceGraph;
    [FieldOffset(0x38)] public IntPtr ExternalPackage;
}

[StructLayout(LayoutKind.Sequential, Size = 0xc)]
public struct FVector
{
    public float X;                                                                          // 0x0000 (size: 0x4)
    public float Y;                                                                          // 0x0004 (size: 0x4)
    public float Z;                                                                          // 0x0008 (size: 0x4)

    public FVector(float x, float y, float z) { X = x; Y = y; Z = z; }
    public override string ToString() => $"({X}, {Y}, {Z})";
}; // Size: 0xC

[StructLayout(LayoutKind.Sequential, Size = 0x8)]
public struct FVector2D
{
    public float X;                                                                          // 0x0000 (size: 0x4)
    public float Y;                                                                          // 0x0004 (size: 0x4)

    public FVector2D(float x, float y) { X = x; Y = y; }

    public override string ToString() => $"({X}, {Y})";
}; // Size: 0x8

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public struct FVector4
{
    public float X;                                                                          // 0x0000 (size: 0x4)
    public float Y;                                                                          // 0x0004 (size: 0x4)
    public float Z;                                                                          // 0x0008 (size: 0x4)
    public float W;                                                                          // 0x000C (size: 0x4)

    public FVector4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }
    public override string ToString() => $"({X}, {Y}, {Z}, {W})";

}; // Size: 0x10

[StructLayout(LayoutKind.Sequential, Size = 0x4)]
public struct FColor
{
    public byte B; // 0x0000 (size: 0x1)
    public byte G; // 0x0001 (size: 0x1)
    public byte R; // 0x0002 (size: 0x1)
    public byte A; // 0x0003 (size: 0x1)

    public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    public FColor(byte r, byte g, byte b, byte a)
    {
        B = a;
        G = b;
        R = g;
        A = r;
    }

    public FColor(uint color)
    {
        R = (byte)((color >> 0x18) & 0xff);
        G = (byte)((color >> 0x10) & 0xff);
        B = (byte)((color >> 0x8) & 0xff);
        A = (byte)(color & 0xff);
    }

    public void SetColor(uint color)
    {
        R = (byte)((color >> 0x18) & 0xff);
        G = (byte)((color >> 0x10) & 0xff);
        B = (byte)((color >> 0x8) & 0xff);
        A = (byte)(color & 0xff);
    }

}; // Size: 0x4
public struct FSprColor
{
    // Different color component order
    public byte A;
    public byte B;
    public byte G;
    public byte R;

    public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    public FSprColor(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    public FSprColor(uint color)
    {
        R = (byte)((color >> 0x18) & 0xff);
        G = (byte)((color >> 0x10) & 0xff);
        B = (byte)((color >> 0x8) & 0xff);
        A = (byte)(color & 0xff);
    }
    public void SetColor(uint color)
    {
        R = (byte)((color >> 0x18) & 0xff);
        G = (byte)((color >> 0x10) & 0xff);
        B = (byte)((color >> 0x8) & 0xff);
        A = (byte)(color & 0xff);
    }
}
[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public struct FLinearColor
{
    public float R;
    public float G;
    public float B;
    public float A;

    public override string ToString() => $"#{(uint)(R * 255):X2}{(uint)(G * 255):X2}{(uint)(B * 255):X2}{(uint)(A * 255):X2}";

    public void SetColor(uint color)
    {
        R = (float)(byte)((color >> 0x18) & 0xff) / 256;
        G = (float)(byte)((color >> 0x10) & 0xff) / 256;
        B = (float)(byte)((color >> 0x8) & 0xff) / 256;
        A = (float)(byte)(color & 0xff) / 256;
    }

    public FLinearColor(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

}; // Size: 0x10