using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Unreal.NativeTypes.Interfaces;
#pragma warning disable CS1591

[StructLayout(LayoutKind.Explicit, Size = 0x298)]
public unsafe struct ULevel
{
    [FieldOffset(0x0000)] public UObject baseObj;
    [FieldOffset(0x00B8)] public UWorld* OwningWorld;
    //[FieldOffset(0x00C0)] public UModel* Model;
    [FieldOffset(0x00C8)] public TArray<IntPtr> ModelComponents;
    //[FieldOffset(0x00D8)] public ULevelActorContainer* ActorCluster;
    [FieldOffset(0x00E0)] public int NumTextureStreamingUnbuiltComponents;
    [FieldOffset(0x00E4)] public int NumTextureStreamingDirtyResources;
    //[FieldOffset(0x00E8)] public ALevelScriptActor* LevelScriptActor;
    //[FieldOffset(0x00F0)] public ANavigationObjectBase* NavListStart;
    //[FieldOffset(0x00F8)] public ANavigationObjectBase* NavListEnd;
    [FieldOffset(0x0100)] public TArray<IntPtr> NavDataChunks;
    [FieldOffset(0x0110)] public float LightmapTotalSize;
    [FieldOffset(0x0114)] public float ShadowmapTotalSize;
    [FieldOffset(0x0118)] public TArray<FVector> StaticNavigableGeometry;
    [FieldOffset(0x0128)] public TArray<FGuid> StreamingTextureGuids;
    [FieldOffset(0x01D0)] public FGuid LevelBuildDataId;
    //[FieldOffset(0x01E0)] public UMapBuildDataRegistry* MapBuildData;
    //[FieldOffset(0x01E8)] public FIntVector LightBuildLevelOffset;
    [FieldOffset(0x01F4)] public byte bIsLightingScenario;
    [FieldOffset(0x01F4)] public byte bTextureStreamingRotationChanged;
    [FieldOffset(0x01F4)] public byte bStaticComponentsRegisteredInStreamingManager;
    [FieldOffset(0x01F4)] public byte bIsVisible;
    //[FieldOffset(0x0258)] public AWorldSettings* WorldSettings;
    [FieldOffset(0x0268)] public TArray<IntPtr> AssetUserData;
    //[FieldOffset(0x0288)] public TArray<FReplicatedStaticActorDestructionInfo> DestroyedReplicatedStaticActors;
}

[StructLayout(LayoutKind.Explicit, Size = 0x798)]
public unsafe struct UWorld //: public UObject
{
    [FieldOffset(0x0030)] public ULevel* PersistentLevel;
    [FieldOffset(0x00D0)] public ULevel* CurrentLevelPendingVisibility;
    [FieldOffset(0x00D8)] public ULevel* CurrentLevelPendingInvisibility;
};

[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe struct FActorSpawnParameters
{
    [FieldOffset(0x0)] public FName name;
    [FieldOffset(0x8)] public AActor* template;
    [FieldOffset(0x10)] public AActor* owner;
    [FieldOffset(0x18)] public AActor* /* APawn* */ instigator;
    [FieldOffset(0x20)] public ULevel* overrideLevel;
    [FieldOffset(0x2c)] public uint objectFlags;
}

[StructLayout(LayoutKind.Explicit, Size = 0x220)]
public unsafe struct AActor // : UObject
{
    [FieldOffset(0x0094)] public float InitialLifeSpan;
    [FieldOffset(0x0098)] public float CustomTimeDilation;
    [FieldOffset(0x00E0)] public AActor* Owner;
    [FieldOffset(0x00E8)] public FName NetDriverName;
    [FieldOffset(0x00F4)] public int InputPriority;
    //[FieldOffset(0x00F8)] public UInputComponent* InputComponent;
    [FieldOffset(0x00F8)] public nint InputComponent;
    [FieldOffset(0x0100)] public float NetCullDistanceSquared;
    [FieldOffset(0x0104)] public int NetTag;
    [FieldOffset(0x0108)] public float NetUpdateFrequency;
    [FieldOffset(0x010C)] public float MinNetUpdateFrequency;
    [FieldOffset(0x0110)] public float NetPriority;
    //[FieldOffset(0x0118)] public APawn* Instigator;
    [FieldOffset(0x0118)] public nint Instigator;
    [FieldOffset(0x0120)] public TArray<nint> Children;
    //[FieldOffset(0x0130)] public USceneComponent* RootComponent;
    [FieldOffset(0x0130)] public nint RootComponent;
    [FieldOffset(0x0138)] public TArray<nint> ControllingMatineeActors;
    [FieldOffset(0x0150)] public TArray<FName> Layers;
    //[FieldOffset(0x0160)] public UChildActorComponent* ParentComponent;
    [FieldOffset(0x0160)] public nint ParentComponent;
    [FieldOffset(0x0170)] public TArray<FName> Tags;
}

[StructLayout(LayoutKind.Explicit, Size = 0xc8)]
public struct FSubsystemCollectionBase
{
    [FieldOffset(0x10)]
    public TMap<nint, nint> SubsystemMap;

    [FieldOffset(0x60)]
    public TMap<nint, nint> SubsystemArrayMap;
}

[StructLayout(LayoutKind.Explicit, Size = 0x1A8)]
public unsafe struct UGameInstance
{
    [FieldOffset(0x0000)] public UObject baseObj;
    [FieldOffset(0x0038)] public TArray<nint> LocalPlayers;
    [FieldOffset(0x0050)] public TArray<nint> ReferencedObjects;
    [FieldOffset(0xe0)] public FSubsystemCollectionBase Subsystems;
}

[StructLayout(LayoutKind.Explicit, Size = 0x288)]
public unsafe struct FWorldContext 
{
    [FieldOffset(0x280)] public UWorld* ThisCurrentWorld;
}

[StructLayout(LayoutKind.Explicit, Size = 0xD20)]
public unsafe struct UEngine
{
    [FieldOffset(0x0000)] public UObject baseObj;
    [FieldOffset(0xc80)] public TArray<nint> WorldList;
}