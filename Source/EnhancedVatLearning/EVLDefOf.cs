using RimWorld;
using Verse;

namespace EnhancedVatLearning
{
    [DefOf]
    public static class EVLDefOf
    {
        static EVLDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EVLDefOf));
        }

        public static ThingDef EVL_Neurostimulator;

        public static ThingDef EVL_VR_Simulator;

        public static ThingDef EVL_Cognition_Engine;
    }
}
