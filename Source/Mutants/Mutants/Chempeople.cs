﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Mutants
{
    public class HediffComp_BigExplosion : HediffWithComps
    {
        public override RulePackDef DeathRules
        {
            get
            {
                return RulePackDef.Transition_DiedExplosive;
            }
        }
        public override void PawnDied(Corpse corpse)
        {
            float radius;
            if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0)
            {
                radius = 1.9f;
            }
            else if (corpse.InnerPawn.ageTracker.CurLifeStageIndex == 1)
            {
                radius = 2.9f;
            }
            else
            {
                radius = 4.9f;
            }
            GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, DamageDefOf.Flame, corpse.InnerPawn, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
        }
    }
}
