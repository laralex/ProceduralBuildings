using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralBuildingsGeneration
{
    class BuildingsGrammarController : IGrammarController
    {
        public IList<GrammarRule> Rules { get; private set; }
        public GrammarNode CurrentWord { get; private set; }
        private readonly Random m_rng;
        private readonly Dictionary<Asset, DMesh3> m_assetsMeshes;
        public BuildingsGrammarController(Random rng, Dictionary<Asset, DMesh3> assets)
        {
            CurrentWord = new RootNode();
            m_rng = rng;
            m_assetsMeshes = assets;
        }

        public GrammarNode TransformWordRepeatedly(GenerationParameters buildingParameters, int epochs, int depthLimit = 50)
        {
            Rules = new List<GrammarRule>
            {
                new RootSplitRule(),
                new TopFloorToRoofRule(),
                new FloorToWallStripRule(),
                new WallStripToSegmentRule(),
                new SegmentToDoorsRule(),
                new SegmentsToWindowsRule(),
            };

            while (epochs > 0 || epochs < 0 && Rules.Any(r => !r.IsTerminated))
            {
                foreach (var rule in Rules)
                {
                    CurrentWord = rule.Apply(CurrentWord, buildingParameters, m_assetsMeshes, depthLimit);
                }
                --epochs;
            }
            return CurrentWord;
        }
    }
}
