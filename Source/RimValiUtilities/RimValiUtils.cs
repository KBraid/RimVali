﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace AvaliMod
{
    public static class RimValiUtility
    {

        public static string build = "Keo";
        public static Dictionary<SkillRecord, float> Skills(Pawn pawn)
        {
            Dictionary<SkillRecord, float> skillNums = new Dictionary<SkillRecord, float>();
            IEnumerable<SkillRecord> skills = pawn.skills.skills;
            foreach (SkillRecord skill in skills)
            {
                skillNums.Add(skill, skill.XpTotalEarned);
            }
            return skillNums;
        }

        public static Dictionary<Pawn, PawnRelationDef> PawnRelations(Pawn pawn)
        {
            Dictionary<Pawn, PawnRelationDef> relatedPawnsToReturn = new Dictionary<Pawn, PawnRelationDef>();

            IEnumerable<Pawn> pawns = pawn.relations.RelatedPawns;
            foreach (Pawn relatedPawn in pawns)
            {
                int onRelation = 0;
                while (onRelation > relatedPawn.relations.RelatedPawns.Count())
                {
                    foreach (PawnRelationDef relationDef in RimValiRelationsFound.relationsFound)
                    {
                        if (relatedPawn.relations.DirectRelationExists(relationDef, pawn))
                        {
                            relatedPawnsToReturn.Add(relatedPawn, relationDef);
                        }
                    }
                    onRelation += 1;
                }
            }
            return null;
        }

        public static Dictionary<Trait, int> Traits(Pawn pawn)
        {
            IEnumerable<Trait> traits = pawn.story.traits.allTraits;
            Dictionary<Trait, int> traitDataToReturn = new Dictionary<Trait, int>();
            foreach (Trait trait in traits)
            {
                traitDataToReturn.Add(trait, trait.Degree);
            }
            return traitDataToReturn;
        }




        public static bool CheckIfPackmatesInRoom(Pawn pawn, PawnRelationDef relationDef)
        {
            Room room = pawn.GetRoom();
            if (!(room == null) && (pawn.Position.Roofed(pawn.Map)))
            {
                IEnumerable<Pawn> pawns = RimValiUtility.GetPackPawns(pawn, relationDef);
                foreach (Pawn packmate in pawns)
                {
                    if (packmate.GetRoom(RegionType.Set_Passable) == room)
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        public static int GetPackSize(Pawn pawn, PawnRelationDef relationDef)
        {
            int foundMembers = 1;
            IEnumerable<Pawn> relatedPawns = pawn.relations.RelatedPawns;
            foreach (Pawn packmate in relatedPawns)
            {
                if (packmate.relations.DirectRelationExists(relationDef, pawn) || pawn.relations.DirectRelationExists(relationDef, packmate))
                {
                    foundMembers += 1;
                }
            }
            return foundMembers;
        }

        public static IEnumerable<Pawn> AllPawnsOfRaceOnMap(ThingDef race, Map map)
        {
            IEnumerable<Pawn> pawns = map.mapPawns.AllPawns;
            List<Pawn> pawnsToReturn = new List<Pawn>();
            foreach (Pawn pawn in pawns)
            {
                if (pawn.def == race)
                {
                    pawnsToReturn.Add(pawn);
                }
            }
            return pawnsToReturn;
        }

        //Added in the "Keo" build. 
        public static IEnumerable<Pawn> AllPawnsOfRaceInMapAndFaction(Pawn pawn, Faction faction)
        {
            List<Pawn> pawnsInMap = new List<Pawn>();
            IEnumerable<Pawn> pawns = CheckAllPawnsInMapAndFaction(pawn.Map, pawn.Faction);
            foreach(Pawn pawn1 in pawns)
            {
                if(pawn1.def == pawn.def)
                {
                    pawnsInMap.Add(pawn1);
                }
            }
            return pawnsInMap;
        }

        public static IEnumerable<Pawn> CheckAllPawnsInMapAndFaction(Map map, Faction faction)
        {
            IEnumerable<Pawn> pawns = PawnsFinder.AllMaps_SpawnedPawnsInFaction(faction);
            List<Pawn> pawnsInMap = new List<Pawn>();
            foreach (Pawn pawn in pawns)
            {
                if (pawn.Map == map)
                {
                    pawnsInMap.Add(pawn);
                }
            }
            IEnumerable<Pawn> pawnsFound = pawnsInMap;
            return pawnsFound;
        }



        public static bool CheckIfBedRoomHasPackmates(Pawn pawn, PawnRelationDef relationDef)
        {
            int packmatesFound = 0;
            Room room = pawn.GetRoom();
            if (!pawn.Awake())
            {
                if (room.ContainedBeds.Count() > 0)
                {
                    IEnumerable<Building_Bed> beds = room.ContainedBeds;
                    foreach (Building_Bed bed in beds)
                    {
                        if (bed.OwnersForReading != null)
                        {
                            IEnumerable<Pawn> owners = bed.OwnersForReading;
                            foreach (Pawn other in owners)
                            {
                                if (other.relations.DirectRelationExists(relationDef, pawn))
                                {
                                    packmatesFound += 1;
                                }
                            }
                        }
                    }
                }
                if (packmatesFound > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        public static void AddThought(Pawn pawn, ThoughtDef thought)
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(thought);
        }

        public static void RemoveThought(Pawn pawn, ThoughtDef thought)
        {
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(thought);
        }

        public static IEnumerable<Pawn> GetPackPawns(Pawn pawn, PawnRelationDef relationDef)
        {
            List<Pawn> packmates = new List<Pawn>();
            IEnumerable<Pawn> pawns = pawn.relations.RelatedPawns.ToList<Pawn>();
            foreach (Pawn packmate in pawns)
            {
                if (packmate.relations.DirectRelationExists(relationDef, pawn))
                {
                    packmates.Add(packmate);
                }
            }
            return packmates;
        }
        public static bool IsOfRace(Pawn pawn, ThingDef race)
        {
            if (pawn.def.defName == race.defName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }






        public static void RemovePackRelationIfDead(Pawn pawn, List<Pawn> packmates, PawnRelationDef relationDef)
        {
            foreach (Pawn packmate in packmates)
            {
                if (packmate.DestroyedOrNull())
                {
                    pawn.relations.RemoveDirectRelation(relationDef, packmate);
                }
            }
        }

        public static void ThoughtWithStage(Pawn pawn, ThoughtDef thought, int stage)
        {
            Thought_Memory thought1 = (Thought_Memory)ThoughtMaker.MakeThought(thought);
            pawn.needs.mood.thoughts.memories.TryGainMemory(thought1, null);
            pawn.needs.mood.thoughts.memories.OldestMemoryOfDef(thought).SetForcedStage(stage);

        }

        public static bool AllowDropWhenPawnCountMet(IncidentParms parms, List<ThingDef> things, ThingDef race, int requiredCount)
        {
            List<Thing> thingList = new List<Thing>();
            foreach (ThingDef thingDef in things)
            {
                thingList.Add(ThingMaker.MakeThing(thingDef));
            }
            Map target = (Map)parms.target;
            IntVec3 intVec3 = DropCellFinder.TradeDropSpot(target);
            if (RimValiUtility.PawnOfRaceCount(Faction.OfPlayer, race) >= requiredCount)
            {
                DropPodUtility.DropThingsNear(intVec3, target, (IEnumerable<Thing>)thingList);
            }
            return true;
        }

        public static int PawnOfRaceCount(Faction faction, ThingDef race)
        {

            IEnumerable<Pawn> pawns = PawnsFinder.AllMaps_SpawnedPawnsInFaction(faction);
            int foundMatches = 0;
            foreach (Pawn pawn in pawns)
            {
                if (IsOfRace(pawn, race))
                {
                    foundMatches += 1;
                }
            }
            return foundMatches;
        }

        public static IEnumerable<Pawn> PawnsOfRaceInFaction(ThingDef race, Faction faction)
        {
            IEnumerable<Pawn> pawns = PawnsFinder.AllMaps_SpawnedPawnsInFaction(faction);
            List<Pawn> pawnsToReturn = null;
            foreach (Pawn pawn in pawns)
            {
                if (IsOfRace(pawn, race))
                {
                    pawnsToReturn.Add(pawn);
                }
            }
            return pawnsToReturn;
        }

        public static bool FactionHasRace(ThingDef race, Faction faction)
        {
            if (PawnOfRaceCount(faction, race) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CollectPackmates(Pawn pawn, Pawn pawn2, PawnRelationDef relationDef)
        {
            //Grab a list of all of their pack relations.
            IEnumerable<Pawn> firstRelatedPawns = pawn.relations.RelatedPawns.Where(x => x.relations.DirectRelationExists(relationDef, pawn));
            IEnumerable<Pawn> secondRelatedPawns = pawn2.relations.RelatedPawns.Where(x => x.relations.DirectRelationExists(relationDef, pawn2));
            foreach (Pawn pawnFound in firstRelatedPawns)
            {
                //if a packmate relation exists between pawn and firstRelatedPawns[relatedItem]
                if (pawn.relations.DirectRelationExists(relationDef, pawnFound))
                {
                    //and the other pawn does not have it
                    if (!pawn2.relations.DirectRelationExists(relationDef, pawnFound) && !(pawn2 == pawnFound))
                    {
                        //add the relation to both
                        pawn2.relations.AddDirectRelation(relationDef, pawnFound);
                    }
                }
            }
            //same thing, except pawn2 is in place of pawn, and secondRelatedPawns replaces firstRelated
            foreach (Pawn foundPawn in secondRelatedPawns)
            {
                if (pawn2.relations.DirectRelationExists(relationDef, foundPawn))
                {
                    if (!pawn.relations.DirectRelationExists(relationDef, foundPawn) && !(pawn == foundPawn))
                    {
                        pawn.relations.AddDirectRelation(relationDef, foundPawn);
                    }
                }
            }
        }

        public static void TrimPack(Pawn pawn, Pawn pawn2, PawnRelationDef relationDef, Faction faction = null, int limit = 5)
        {
            if (faction == null)
            {
                faction = pawn.Faction;
            }
            IEnumerable<Pawn> firstRelatedPawns = pawn.relations.RelatedPawns.Where(x => x.relations.DirectRelationExists(relationDef, pawn));
            IEnumerable<Pawn> secondRelatedPawns = pawn2.relations.RelatedPawns.Where(x => x.relations.DirectRelationExists(relationDef, pawn2));
            foreach (Pawn pawnFound in firstRelatedPawns)
            {
                //if a packmate relation exists between pawn and firstRelatedPawns[relatedItem]
                if (pawn.relations.DirectRelationExists(relationDef, pawnFound) && GetPackSize(pawn, relationDef) > limit)
                {
                    //and the other pawn does not have it
                    if (pawn2.relations.DirectRelationExists(relationDef, pawnFound))
                    {
                        //remove the relation from both
                        pawn2.relations.TryRemoveDirectRelation(relationDef, pawnFound);
                    }
                }
            }
            //same thing, except pawn2 is in place of pawn, and secondRelatedPawns replaces firstRelated
            foreach (Pawn foundPawn in secondRelatedPawns)
            {
                if (pawn2.relations.DirectRelationExists(relationDef, foundPawn) && GetPackSize(pawn2, relationDef) > limit)
                {
                    if (pawn.relations.DirectRelationExists(relationDef, foundPawn))
                    {
                        pawn.relations.TryRemoveDirectRelation(relationDef, foundPawn);
                    }
                }
            }
        }


        //The "Keo" build's newer pack system.
        //It's faster, easier to understand, and smarter. 
        //It can allow any races enabled to join, like a few of its predecessors.
        public static void KeoBuildMakePack(Pawn pawn, PawnRelationDef relationDef, IEnumerable<ThingDef> racesInPacks, int packLimit)
        {
            //Get all pawns in the allowed list.
            IEnumerable<Pawn> packMates = PawnsFinder.AllMaps_SpawnedPawnsInFaction(pawn.Faction).Where<Pawn>(x => racesInPacks.Contains(x.def));
            foreach (Pawn packmate in packMates)
            {
                //check that neither's pack size is too big, and that they aren't the same
                if (!(GetPackSize(packmate, relationDef) >= packLimit) && !(GetPackSize(pawn, relationDef) >= packLimit) && !(pawn == packmate))
                {
                    // adds the relation and evens out the pack.
                    pawn.relations.AddDirectRelation(relationDef, packmate);
                    CollectPackmates(packmate, pawn, relationDef);
                    TrimPack(pawn, packmate, relationDef, packmate.Faction, packLimit);
                }
                else
                {
                    //break if the pawn has reached the max pack limit.
                    if(GetPackSize(pawn, relationDef) == packLimit)
                    {
                        break;
                    }
                }
            }
        }
        public static void KeoBuildMakeBasePack(Pawn pawn, PawnRelationDef relationDef, IEnumerable<ThingDef> racesInPacks, int packLimit)
        {
            //Get all pawns in the allowed list.
            IEnumerable<Pawn> packMates = PawnsFinder.AllMaps_SpawnedPawnsInFaction(pawn.Faction).Where<Pawn>(x => racesInPacks.Contains(x.def));
            foreach (Pawn packmate in packMates)
            {
                //check that neither's pack size is too big, and that they aren't the same
                if (GetPackSize(packmate, relationDef) ==1 && GetPackSize(pawn, relationDef) == 1 && !(pawn == packmate))
                {
                    // adds the relation and evens out the pack.
                    pawn.relations.AddDirectRelation(relationDef, packmate);
                    CollectPackmates(packmate, pawn, relationDef);
                    TrimPack(pawn, packmate, relationDef, packmate.Faction, packLimit);
                }
                else
                {
                    //break if the pawn has reached the max pack limit.
                    if (GetPackSize(pawn, relationDef) == packLimit)
                    {
                        break;
                    }
                }
            }
        }
    }
}