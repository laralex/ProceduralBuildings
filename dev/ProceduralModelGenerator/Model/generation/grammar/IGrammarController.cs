using System.Collections.Generic;

namespace ProceduralBuildingsGeneration
{
    interface IGrammarController
    {
        IList<GrammarRule> Rules { get; }
        GrammarNode CurrentWord { get; }
        GrammarNode TransformWordRepeatedly(GenerationParameters buildingParameters, int epochs, int depthLimit = 50);
    }
}
