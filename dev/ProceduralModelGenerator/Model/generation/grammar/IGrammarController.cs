using System.Collections.Generic;

namespace ProceduralBuildingsGeneration
{
    interface IGrammarController
    {
        ISet<GrammarRule> Rules { get; }
        GrammarNode CurrentWord { get; }
        GrammarNode TransformWordRepeatedly(GenerationParameters buildingParameters, int epochs, int depthLimit = 50);
    }
}
