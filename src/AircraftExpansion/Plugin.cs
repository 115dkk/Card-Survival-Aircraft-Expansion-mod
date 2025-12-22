using AircraftExpansion.Data;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModCore.Data;

namespace AircraftExpansion;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("Pikachu.CSTI.ModCore")]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "AircraftExpansion";
    public const string PluginName = "Aircraft Expansion";
    public const string PluginVersion = "1.0.0";

    public static Plugin Instance { get; private set; }
    public static ManualLogSource Log { get; private set; }

    private static readonly Harmony Harmony = new Harmony(PluginGuid);

    private void Awake()
    {
        Instance = this;
        Log = Logger;

        // ModCore 데이터 로딩 이벤트 등록
        Loader.LoadBeforeEvent += OnLoadBefore;
        Loader.LoadCompleteEvent += OnLoadComplete;

        // Harmony 패치 적용
        Harmony.PatchAll();

        Log.LogInfo($"Plugin {PluginName} v{PluginVersion} is loaded!");
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        Loader.LoadBeforeEvent -= OnLoadBefore;
        Loader.LoadCompleteEvent -= OnLoadComplete;

        // Harmony 패치 해제
        Harmony.UnpatchSelf();
    }

    /// <summary>
    /// ModCore 데이터 로딩 시작 전에 호출됩니다.
    /// 새로운 데이터 타입 등록이나 사전 준비 작업에 사용합니다.
    /// </summary>
    private static void OnLoadBefore()
    {
        Log.LogInfo("OnLoadBefore: Preparing aircraft expansion data...");

        // TODO: 데이터 타입 등록 또는 사전 로딩 작업
    }

    /// <summary>
    /// ModCore 데이터 로딩 완료 후에 호출됩니다.
    /// 기존 게임 데이터 수정 작업에 사용합니다.
    /// </summary>
    private static void OnLoadComplete()
    {
        Log.LogInfo("OnLoadComplete: Applying aircraft expansion modifications...");

        // 비행기에 기존 집 업그레이드 활성화
        AircraftModifications.EnableAircraftUpgrades();
    }
}
