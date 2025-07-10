using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace aRandomKiwi.KFM
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var inst = new Harmony("rimworld.randomKiwi.RFM");
            inst.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static FieldInfo MapFieldInfo;
    }
}
