using HarmonyLib;

namespace AircraftExpansion.Patches;

/// <summary>
/// Harmony 패치 예시 클래스
/// 필요한 경우 게임 메서드를 패치하여 동작을 수정할 수 있습니다.
/// </summary>
// [HarmonyPatch(typeof(TargetClass), nameof(TargetClass.TargetMethod))]
// public static class ExamplePatch
// {
//     /// <summary>
//     /// 원본 메서드 실행 전에 호출됩니다.
//     /// return false 시 원본 메서드 실행을 건너뜁니다.
//     /// </summary>
//     [HarmonyPrefix]
//     public static bool Prefix()
//     {
//         return true; // 원본 메서드 실행
//     }
//
//     /// <summary>
//     /// 원본 메서드 실행 후에 호출됩니다.
//     /// __result 파라미터로 반환값을 수정할 수 있습니다.
//     /// </summary>
//     [HarmonyPostfix]
//     public static void Postfix()
//     {
//     }
// }
