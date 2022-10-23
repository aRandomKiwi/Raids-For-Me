using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace aRandomKiwi.RFM
{
    internal class FactionDialogMaker_Patch
    {
        [HarmonyPatch(typeof(FactionDialogMaker), "FactionDialogFor")]
        public class FactionDialogFor
        {
            private static Texture2D insultTex;
            [HarmonyPostfix]
            public static void Listener( ref DiaNode __result, Pawn negotiator, Faction faction)
            {
                DiaOption opt = new DiaOption("RFM_InsultFaction".Translate(faction.leader.LabelCap, faction.Name));
                opt.action = delegate
                {
                    List<RulePackDef> list = new List<RulePackDef>();
                    string text;

                    //Goodwill decrement
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, -1 * Rand.Range(1,Settings.goodwillLoss+1) , true, true, null, null);

                    if (insultTex == null)
                        insultTex = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Insult", true); 

                    //InteractionDefOf.Insult.Worker.Interacted(negotiator, faction.leader, list, out text, out label, out letterDef);
                    PlayLogEntry_Interaction playLogEntry_Interaction = new PlayLogEntry_Interaction(InteractionDefOf.Insult, negotiator, faction.leader, list);
                    MoteMaker.MakeInteractionBubble(negotiator, null, InteractionDefOf.Insult.interactionMote, insultTex);

                    text = (string) Traverse.Create(playLogEntry_Interaction).Method("ToGameStringFromPOV_Worker", faction.leader, true).GetValue();

                    //If still ally then just display message
                    if (faction.PlayerRelationKind == FactionRelationKind.Ally ||  faction.PlayerRelationKind == FactionRelationKind.Neutral)
                    {
                        Messages.Message("RFM_InsultAlly".Translate(text, faction.leader.LabelCap, faction.Name), MessageTypeDefOf.NegativeEvent, false);
                    }
                    else
                    {
                        //If raid
                        if (Rand.Chance(Settings.chanceGetRaided))
                        {
                            int random = 0;
                            string sub = "";
                            if (Settings.maxHourStartRaid > 0)
                                random = Rand.Range(Settings.minHourStartRaid, Settings.maxHourStartRaid);

                            if (random == 0)
                            {
                                sub = "RFM_StartRaidNow".Translate(faction.leader.LabelCap,faction.Name);
                            }
                            else
                            {
                                sub = "RFM_StartRaidDelayed".Translate(faction.leader.LabelCap,faction.Name,Utils.TranslateTicksToTextIRLSeconds(random * 2500));
                            }


                            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(incCat: IncidentCategoryDefOf.ThreatBig, target: negotiator.Map);
                            incidentParms.forced = true;
                            incidentParms.faction = faction;
                            incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                            incidentParms.target = negotiator.Map;

                            int delay = 0;
                            //Small chance to get an instant drop pod raid if activated and if faction at minimum industrial level
                            if (Settings.allowChanceAirDrop && faction.def.techLevel >= TechLevel.Industrial && Rand.Chance(0.05f))
                            {
                                if (Rand.Chance(0.75f))
                                {
                                    incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeDrop;
                                }
                                else
                                {
                                    incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.CenterDrop;
                                }
                                //Execution now
                                sub = "RFM_StartAirDropRaidNow".Translate(faction.leader.LabelCap, faction.Name);
                            }
                            else
                            {
                                incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                                delay = (random * 2500);
                            }

                            Messages.Message("RFM_InsultEnemy".Translate(text, sub), MessageTypeDefOf.NegativeEvent, false);

                            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + delay, incidentParms, 240000);
                            //lastRaidGT increment of the time to wait for the raid + the conventional waiting time
                            Utils.GCRFM.lastRaidGT[faction.GetUniqueLoadID()] = Find.TickManager.TicksGame + delay + (Settings.timeBeforeInsultingAgain * 2500);
                        }
                        else
                        {
                            Messages.Message("RFM_InsultEnemyWithoutRaid".Translate(text, faction.leader.LabelCap, faction.Name), MessageTypeDefOf.NegativeEvent, false);
                        }
                    }
                };

                //If the waiting time is not reached, the control is grayed out
                if ( Utils.GCRFM.lastRaidGT.ContainsKey(faction.GetUniqueLoadID()) && Utils.GCRFM.lastRaidGT[faction.GetUniqueLoadID()] > Find.TickManager.TicksGame)
                {
                    opt.disabled = true;
                }

                opt.resolveTree = true;

                //Add dialog menu allowing to insult the interlocutor just before the finish button
                __result.options.Insert(__result.options.Count-1, opt);
            }
        }
    }
}