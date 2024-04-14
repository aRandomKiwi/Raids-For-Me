using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;

namespace aRandomKiwi.RFM
{
    public class Settings : ModSettings
    {
        public static float chanceGetRaided = 1.0f;
        public static int timeBeforeInsultingAgain = 0;
        public static bool allowRangedAttack = true;
        public static int goodwillLoss = 15;
        public static int minHourStartRaid = 1;
        public static int maxHourStartRaid = 8;
        public static bool allowChanceAirDrop = true;

        public static Vector2 scrollPosition = Vector2.zero;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;

            var defaultColumnWidth = (inRect.width - 50);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            var scrollRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height * 2.2f + 300);
            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);

            list.Begin(scrollRect);


            list.CheckboxLabeled("RFM_SettingsAllowChanceAirDrop".Translate(), ref allowChanceAirDrop);
            //Percentage chance of RAID generation
            list.Label("RFM_SettingsChanceGetRaided".Translate((int)(chanceGetRaided * 100)));
            chanceGetRaided = list.Slider(chanceGetRaided, 0.01f, 1.0f);
            //Minimum time between raids
            list.Label("RFM_SettingsTimeBeforeInsultingAgain".Translate(timeBeforeInsultingAgain));
            timeBeforeInsultingAgain = (int) list.Slider(timeBeforeInsultingAgain, 0, 1000);
            //Loss of goodwill when insulting
            list.Label("RFM_SettingsGoodwillLoss".Translate(goodwillLoss));
            goodwillLoss = (int)list.Slider(goodwillLoss, 0, 100);
            //Minimum hour before start of the raid
            list.Label("RFM_SettingsMinHourStartRaid".Translate(minHourStartRaid));
            minHourStartRaid = (int)list.Slider(minHourStartRaid, 0, 128);
            //maximum hour before start of the raid
            list.Label("RFM_SettingsMaxHourStartRaid".Translate(maxHourStartRaid));
            if (maxHourStartRaid < minHourStartRaid)
                maxHourStartRaid += 1;

            maxHourStartRaid = (int)list.Slider(maxHourStartRaid, 0, 128);

            list.End();
            Widgets.EndScrollView();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look<float>(ref chanceGetRaided, "chanceGetRaided", 1.0f);
            Scribe_Values.Look<int>(ref timeBeforeInsultingAgain, "timeBeforeInsultingAgain", 0);
            Scribe_Values.Look<int>(ref goodwillLoss, "goodwillLoss", 15);
            Scribe_Values.Look<int>(ref minHourStartRaid, "minHourStartRaid", 1);
            Scribe_Values.Look<int>(ref maxHourStartRaid, "maxHourStartRaid", 8);
            Scribe_Values.Look<bool>(ref allowChanceAirDrop, "allowChanceAirDrop", true);

        }
    }
}