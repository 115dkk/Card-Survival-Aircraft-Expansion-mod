using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using ModCore.Data;

namespace CardDumper;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency("Pikachu.CSTI.ModCore")]
public class CardDumper : BaseUnityPlugin
{
    public const string PluginGuid = "CardDumper.Debug";
    public const string PluginName = "Card Dumper";
    public const string PluginVersion = "1.0.0";

    public static ManualLogSource Log { get; private set; }

    private static readonly string[] SearchKeywords =
    {
        "Plane", "Crash", "WhiteWash", "StitchedHide",
        "Floor", "Wall", "Shelter", "House"
    };

    private void Awake()
    {
        Log = Logger;
        Loader.LoadCompleteEvent += OnLoadComplete;
        Log.LogInfo($"Plugin {PluginName} loaded. Will dump cards after game data loads.");
    }

    private void OnDestroy()
    {
        Loader.LoadCompleteEvent -= OnLoadComplete;
    }

    private static void OnLoadComplete()
    {
        try
        {
            DumpCards();
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to dump cards: {ex}");
        }
    }

    private static void DumpCards()
    {
        var allObjects = GetAllUniqueObjects();
        if (allObjects == null || allObjects.Count == 0)
        {
            Log.LogWarning("No UniqueIDScriptable objects found!");
            return;
        }

        Log.LogInfo($"Total UniqueIDScriptable objects: {allObjects.Count}");

        var matchingObjects = allObjects.Values
            .Where(obj => obj != null && MatchesKeyword(obj.name))
            .OrderBy(obj => obj.name)
            .ToList();

        Log.LogInfo($"Matching objects: {matchingObjects.Count}");

        var sb = new StringBuilder();
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine("Card Dumper - UniqueIDScriptable Objects Dump");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Total objects: {allObjects.Count}");
        sb.AppendLine($"Matching objects: {matchingObjects.Count}");
        sb.AppendLine($"Keywords: {string.Join(", ", SearchKeywords)}");
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine();

        foreach (var obj in matchingObjects)
        {
            DumpObject(sb, obj);
        }

        // Also dump all EnvImprovement cards
        sb.AppendLine();
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine("ALL EnvImprovement Cards");
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine();

        var envImprovements = allObjects.Values
            .OfType<CardData>()
            .Where(c => c != null && c.CardType == CardTypes.EnvImprovement)
            .OrderBy(c => c.name)
            .ToList();

        foreach (var card in envImprovements)
        {
            DumpObject(sb, card);
        }

        // Also dump all Explorable cards
        sb.AppendLine();
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine("ALL Explorable Cards");
        sb.AppendLine("=".PadRight(80, '='));
        sb.AppendLine();

        var explorables = allObjects.Values
            .OfType<CardData>()
            .Where(c => c != null && c.CardType == CardTypes.Explorable)
            .OrderBy(c => c.name)
            .ToList();

        foreach (var card in explorables)
        {
            DumpObject(sb, card);
        }

        // Write to file
        var dumpPath = Path.Combine(Paths.BepInExRootPath, "dumps");
        Directory.CreateDirectory(dumpPath);
        var filePath = Path.Combine(dumpPath, "card_dump.txt");
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        Log.LogInfo($"Card dump written to: {filePath}");
    }

    private static bool MatchesKeyword(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        return SearchKeywords.Any(kw =>
            name.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static void DumpObject(StringBuilder sb, UniqueIDScriptable obj)
    {
        sb.AppendLine("-".PadRight(60, '-'));
        sb.AppendLine($"Name: {obj.name}");
        sb.AppendLine($"UniqueID: {obj.UniqueID}");
        sb.AppendLine($"Type: {obj.GetType().FullName}");

        if (obj is CardData card)
        {
            sb.AppendLine($"CardType: {card.CardType}");

            // CardName (localized)
            if (card.CardName != null)
            {
                sb.AppendLine($"CardName.DefaultText: {card.CardName.DefaultText}");
                sb.AppendLine($"CardName.LocalizationKey: {card.CardName.LocalizationKey}");
            }

            // EnvironmentImprovements
            if (card.EnvironmentImprovements != null && card.EnvironmentImprovements.Length > 0)
            {
                sb.AppendLine($"EnvironmentImprovements ({card.EnvironmentImprovements.Length}):");
                foreach (var ei in card.EnvironmentImprovements)
                {
                    if (ei?.Card != null)
                    {
                        sb.AppendLine($"  - {ei.Card.name} (ID: {ei.Card.UniqueID})");
                    }
                    else
                    {
                        sb.AppendLine($"  - (null or missing reference)");
                    }
                }
            }
            else
            {
                sb.AppendLine("EnvironmentImprovements: (none)");
            }

            // BlueprintCardConditions
            if (card.BlueprintCardConditions != null && card.BlueprintCardConditions.Length > 0)
            {
                sb.AppendLine($"BlueprintCardConditions ({card.BlueprintCardConditions.Length}):");
                foreach (var cond in card.BlueprintCardConditions)
                {
                    var triggerName = cond.TriggerCard != null ? cond.TriggerCard.name : "(null)";
                    sb.AppendLine($"  - TriggerCard: {triggerName}, Inverted: {cond.Inverted}");
                }
            }

            // BlueprintTagConditions
            if (card.BlueprintTagConditions != null && card.BlueprintTagConditions.Length > 0)
            {
                sb.AppendLine($"BlueprintTagConditions ({card.BlueprintTagConditions.Length}):");
                foreach (var cond in card.BlueprintTagConditions)
                {
                    var tagName = cond.TriggerTag != null ? cond.TriggerTag.name : "(null)";
                    sb.AppendLine($"  - TriggerTag: {tagName}, Inverted: {cond.Inverted}");
                }
            }

            // CardTags
            if (card.CardTags != null && card.CardTags.Length > 0)
            {
                sb.AppendLine($"CardTags: {string.Join(", ", card.CardTags.Where(t => t != null).Select(t => t.name))}");
            }
        }

        sb.AppendLine();
    }

    private static Dictionary<string, UniqueIDScriptable> GetAllUniqueObjects()
    {
        var field = typeof(UniqueIDScriptable).GetField(
            "AllUniqueObjects",
            BindingFlags.NonPublic | BindingFlags.Static);

        if (field == null)
        {
            Log.LogError("Could not find AllUniqueObjects field!");
            return new Dictionary<string, UniqueIDScriptable>();
        }

        return field.GetValue(null) as Dictionary<string, UniqueIDScriptable>
               ?? new Dictionary<string, UniqueIDScriptable>();
    }
}
