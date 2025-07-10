using HarmonyLib;
using System.Reflection;
using Verse;
using UnityEngine;

namespace aRandomKiwi.RFM
{
    [StaticConstructorOnStartup]
    class RaidForMe : Mod
    {
        public RaidForMe(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public void Save()
        {
            LoadedModManager.GetMod<RaidForMe>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "Raids For Me";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}