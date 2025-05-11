using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace EnhancedVatLearning
{
    public class HediffComp_EnhancedLearning : HediffComp
    {
        private HediffCompProperties_EnhancedLearning Props => props as HediffCompProperties_EnhancedLearning;
        public int passionLearningCycles = 0;
        public int traitLearningCycles = 0;

        public int additionalTraits = 0;
        public int additionalPassions = 0;

        public void Learn()
        {
            if (Pawn.skills == null || Pawn.ParentHolder == null || 
                Pawn.ParentHolder is not Building_GrowthVat vat)
            {
                return;
            }

            CompAffectedByFacilities facilityComp = vat.TryGetComp<CompAffectedByFacilities>();
            if (facilityComp.LinkedFacilitiesListForReading.NullOrEmpty())
                return;

            float additionalBoost = 0;
            int linkedVRPods = 0;
            int linkedCognitionPods = 0;

            foreach (Thing facility in facilityComp.LinkedFacilitiesListForReading)
            {
                if (facility.def == EVLDefOf.EVL_Neurostimulator)
                {
                    additionalBoost += Props.neurostimBoost;
                }
                else if (facility.def == EVLDefOf.EVL_VR_Simulator)
                {
                    CompFacility comp = facility.TryGetComp<CompFacility>();
                    additionalBoost += Props.vrBoost;

                    foreach (Thing linked in comp.LinkedBuildings)
                    {
                        if (linked == vat || linked is not Building_GrowthVat linkedVat ||
                            linkedVat.SelectedPawn == null)
                        {
                            continue;
                        }

                        additionalBoost += Props.vrBoostAdditional;
                        linkedVRPods += 1;

                        if (linkedVRPods >= Props.maxVRBoost)
                        {
                            break;
                        }
                    }
                }
                else if (facility.def == EVLDefOf.EVL_Cognition_Engine)
                {
                    additionalBoost += Props.cognitionEngineBoost;
                    CompFacility comp = facility.TryGetComp<CompFacility>();

                    foreach (Thing linked in comp.LinkedBuildings)
                    {
                        if (linked == vat || linked is not Building_GrowthVat linkedVat ||
                            linkedVat.SelectedPawn == null)
                        {
                            continue;
                        }

                        linkedCognitionPods += 1;

                        if (linkedCognitionPods >= Props.maxCogBoost)
                        {
                            break;
                        }
                    }
                }
            }

            List<SkillRecord> skillRecords = Pawn.skills?.skills?.Where((SkillRecord x) => !x.TotallyDisabled && x.Level < 20)?.ToList();

            if (skillRecords.NullOrEmpty())
            {
                return;
            }

            float divider = 1f;

            if (HarmonyPatches.enableCompat)
            {
                divider = 9f;
            }

            skillRecords.RandomElementByWeight((SkillRecord x) => (float)Math.Sqrt(x.Level) * x.LearnRateFactor(true)).Learn(additionalBoost / divider, true);

            if (linkedVRPods > 0)
            {
                passionLearningCycles += 1;

                if (linkedVRPods <= 2)
                {
                    if (passionLearningCycles >= 3 * divider)
                    {
                        passionLearningCycles = 0;
                        additionalPassions += 1;
                    }
                }
                else if (linkedVRPods <= 5)
                {
                    if (passionLearningCycles >= 2 * divider)
                    {
                        passionLearningCycles = 0;
                        additionalPassions += 1;
                    }
                }
                else
                {
                    if (passionLearningCycles >= 1 * divider)
                    {
                        passionLearningCycles = 0;
                        additionalPassions += 1;
                    }
                }
            }

            if (linkedCognitionPods > 0)
            {
                traitLearningCycles += 1;

                if (linkedCognitionPods <= 2)
                {
                    if (traitLearningCycles >= 2 * divider)
                    {
                        traitLearningCycles = 0;
                        additionalTraits += 1;
                    }
                }
                else
                {
                    if (traitLearningCycles >= 1 * divider)
                    {
                        traitLearningCycles = 0;
                        additionalTraits += 1;
                    }
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref passionLearningCycles, "passionLearningCycles");
            Scribe_Values.Look(ref traitLearningCycles, "traitLearningCycles");
            Scribe_Values.Look(ref additionalTraits, "additionalTraits");
            Scribe_Values.Look(ref additionalPassions, "additionalPassions");
        }
    }
}
