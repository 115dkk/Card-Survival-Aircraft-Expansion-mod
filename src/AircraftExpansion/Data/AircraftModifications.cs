using System.Linq;

namespace AircraftExpansion.Data;

/// <summary>
/// 비행기 카드에 환경 개선(업그레이드) 기능을 추가하는 클래스
/// </summary>
public static class AircraftModifications
{
    // 검증된 UniqueID (card_dump.txt에서 확인)
    private const string PlaneCrashId = "d20527131ef12624da48a0fd0e52cb11";
    private const string WhiteWashedWallsId = "359709c0f50d26a4fb50e82e26cdf57c";
    private const string StitchedHideFloorId = "2afb6d1f2179a9742b38a4cb30d58e2d";

    /// <summary>
    /// 비행기에 기존 집 업그레이드를 활성화합니다.
    /// LoadCompleteEvent에서 호출되어야 합니다.
    /// </summary>
    public static void EnableAircraftUpgrades()
    {
        // 비행기 카드 가져오기
        var aircraftCard = UniqueIDScriptable.GetFromID<CardData>(PlaneCrashId);
        if (aircraftCard == null)
        {
            Plugin.Log.LogError($"Aircraft card not found! (ID: {PlaneCrashId})");
            return;
        }

        Plugin.Log.LogInfo($"Found aircraft card: {aircraftCard.name} (UniqueID: {aircraftCard.UniqueID})");
        LogCurrentImprovements(aircraftCard);

        // 흰 벽 업그레이드 추가
        var whiteWalls = UniqueIDScriptable.GetFromID<CardData>(WhiteWashedWallsId);
        if (whiteWalls != null)
        {
            AddEnvironmentImprovement(aircraftCard, whiteWalls);
        }
        else
        {
            Plugin.Log.LogWarning($"White walls card not found! (ID: {WhiteWashedWallsId})");
        }

        // 가죽 바닥 업그레이드 추가
        var leatherFloor = UniqueIDScriptable.GetFromID<CardData>(StitchedHideFloorId);
        if (leatherFloor != null)
        {
            AddEnvironmentImprovement(aircraftCard, leatherFloor);
        }
        else
        {
            Plugin.Log.LogWarning($"Leather floor card not found! (ID: {StitchedHideFloorId})");
        }

        Plugin.Log.LogInfo("Aircraft upgrade modifications complete!");
        LogCurrentImprovements(aircraftCard);
    }

    /// <summary>
    /// 현재 환경 개선 목록을 로깅합니다.
    /// </summary>
    private static void LogCurrentImprovements(CardData card)
    {
        if (card.EnvironmentImprovements == null || card.EnvironmentImprovements.Length == 0)
        {
            Plugin.Log.LogInfo($"  {card.name} has no EnvironmentImprovements");
            return;
        }

        Plugin.Log.LogInfo($"  {card.name} EnvironmentImprovements ({card.EnvironmentImprovements.Length}):");
        foreach (var ei in card.EnvironmentImprovements)
        {
            if (ei?.Card != null)
            {
                Plugin.Log.LogInfo($"    - {ei.Card.name} (ID: {ei.Card.UniqueID})");
            }
        }
    }

    /// <summary>
    /// 카드에 환경 개선을 추가합니다.
    /// </summary>
    private static void AddEnvironmentImprovement(CardData targetCard, CardData improvement)
    {
        if (targetCard == null || improvement == null)
        {
            return;
        }

        // 이미 존재하는지 확인
        if (targetCard.EnvironmentImprovements != null &&
            targetCard.EnvironmentImprovements.Any(ei => ei?.Card == improvement))
        {
            Plugin.Log.LogInfo($"  Improvement '{improvement.name}' already exists on '{targetCard.name}'");
            return;
        }

        // 새 CardDataRef 생성 및 추가
        var newRef = new CardDataRef { Card = improvement };

        if (targetCard.EnvironmentImprovements == null)
        {
            targetCard.EnvironmentImprovements = new CardDataRef[] { newRef };
        }
        else
        {
            var newArray = new CardDataRef[targetCard.EnvironmentImprovements.Length + 1];
            targetCard.EnvironmentImprovements.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = newRef;
            targetCard.EnvironmentImprovements = newArray;
        }

        Plugin.Log.LogInfo($"  Added improvement '{improvement.name}' to '{targetCard.name}'");
    }
}
