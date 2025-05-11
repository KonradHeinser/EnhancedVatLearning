using Verse;

namespace EnhancedVatLearning
{
    public class HediffCompProperties_EnhancedLearning : HediffCompProperties
    {
        public HediffCompProperties_EnhancedLearning()
        {
            compClass = typeof(HediffComp_EnhancedLearning);
        }

        public float neurostimBoost = 4000;
        public float vrBoost = 1200;
        public float vrBoostAdditional = 1200;
        public float cognitionEngineBoost = 2000;
        public int maxVRBoost = 8;
        public int maxCogBoost = 4;
    }
}
