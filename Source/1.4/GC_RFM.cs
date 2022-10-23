using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using UnityEngine;

namespace aRandomKiwi.RFM
{
    public class GC_RFM : GameComponent
    {

        public GC_RFM(Game game)
        {
            this.game = game;
            Utils.GCRFM = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref this.lastRaidGT, "lastRaidGT", LookMode.Value);
        }

        public override void LoadedGame()
        {
            base.LoadedGame();


        }

        public override void StartedNewGame()
        {

        }

        public Dictionary<string, int> lastRaidGT = new Dictionary<string, int>();
        private Game game;
    }
}