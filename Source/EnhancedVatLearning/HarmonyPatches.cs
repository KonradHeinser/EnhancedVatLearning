using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace EnhancedVatLearning
{
    [StaticConstructorOnStartup]

    public class HarmonyPatches
    {
        public static readonly Type patchType = typeof(HarmonyPatches);

        public static bool enableCompat = false;

        static HarmonyPatches()
        {
            Harmony harmony = new Harmony(id: "rimworld.alite.enhancedvatlearning");
            harmony.PatchAll();

            if (ModLister.GetActiveModWithIdentifier("com.makeitso.enhancedgrowthvatlearning") != null)
            {
                enableCompat = true;
            }

            harmony.Patch(AccessTools.Method(typeof(Hediff_VatLearning), "Learn"),
                postfix: new HarmonyMethod(patchType, nameof(VatLearnPatch)));
            harmony.Patch(AccessTools.Method(typeof(ChoiceLetter_GrowthMoment), "CacheLetterText"),
                new HarmonyMethod(patchType, nameof(GrowthMomentPatch))); 
            // Prefixing this moment to ensure any mod that alters stuff in the configure has already taken effect
        }

        public static void VatLearnPatch(Hediff_VatLearning __instance)
        {
            __instance.TryGetComp<HediffComp_EnhancedLearning>()?.Learn();
        }

        public static void GrowthMomentPatch(Pawn ___pawn, ref int ___passionGainsCount,
            ref int ___passionChoiceCount, ref int ___traitChoiceCount)
        {
            List<HediffComp_EnhancedLearning> enhancers = ___pawn.health.hediffSet.hediffs.OfType<HediffWithComps>().SelectMany((HediffWithComps x) => x.comps).OfType<HediffComp_EnhancedLearning>().ToList();
            if (enhancers.NullOrEmpty())
                return;

            int passionsLeft = ___pawn.skills.skills.Where(arg => arg.passion != Passion.Major).Count();

            foreach (HediffComp_EnhancedLearning comp in enhancers)
            {
                ___traitChoiceCount += comp.additionalTraits;

                ___passionGainsCount = Math.Min(___passionGainsCount + comp.additionalPassions, passionsLeft);
                ___passionGainsCount = Math.Min(___passionGainsCount + comp.additionalPassions * 2, passionsLeft);

                comp.additionalPassions = 0;
                comp.additionalTraits = 0;
            }
        }
    }
}
