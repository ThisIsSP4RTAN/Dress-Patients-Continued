﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DressPatient
{
    [StaticConstructorOnStartup]
    static class DressPatientUtility
    {
        public static bool usingHumanoidAlienRaces = false;

        static DressPatientUtility()
        {
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Humanoid Alien Races"))
                usingHumanoidAlienRaces = true;
        }

        public static bool IsPatient(this Pawn pawn)
        {
            // Patient must be in a bed.
            Building_Bed bed = pawn.CurrentBed();
            if (bed == null)
                return false;
            if (pawn.NonHumanlikeOrWildMan())
            {
                // Animals must be in player's bed.
                if (bed.Faction != Faction.OfPlayer)
                    return false;
            }
            else
            {
                // Humanlikes must be of player faction or be guest of player faction.
                if (pawn.Faction != Faction.OfPlayer && pawn.HostFaction != Faction.OfPlayer)
                    return false;
            }

            // Should not be designated for slaughter or execution.
            if (pawn.Spawned && pawn.Map.designationManager.DesignationOn(pawn, DesignationDefOf.Slaughter) != null)
                return false;
            if (pawn.guest != null && pawn.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.Execution)
                return false;
            if (pawn.RaceProps.IsMechanoid) 
                return false;

            // Must require medical care.
            if (!HealthAIUtility.ShouldSeekMedicalRest(pawn))
                return false;

            return true;
        }

        public static bool IsValidPawn(this Pawn pawn)
        {
	        return (pawn.IsPrisonerOfColony && DressPatientMod.settings.dressPrisoners) || pawn.IsPatient();
        }
        

        //ThatHitmann did this method. Blame him if it's broken
        public static bool IsHumanCorpse(this Thing thing, out Pawn innerPawn)
        {
            if (thing is Corpse corpse)
            {
                innerPawn = corpse.InnerPawn;
                return !(innerPawn.RaceProps.IsMechanoid || innerPawn.IsMutant || innerPawn.RaceProps.intelligence < (Intelligence)2);
            }
            innerPawn = null;
            return false;
        }
    }
}
