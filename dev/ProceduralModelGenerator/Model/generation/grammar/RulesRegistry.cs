using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    interface IGrammarController
    {
        IList<Rule> Rules { get; }
        GrammarNode CurrentWord { get; }
        GrammarNode TransformWordUntilTermination(GenerationParameters buildingParameters, int depthLimit = 10);
    }
    class BuildingsGrammarController : IGrammarController
    {
        public IList<Rule> Rules { get; private set; }
        public GrammarNode CurrentWord { get; private set; }
        private Random m_rng { get; set; }
        public BuildingsGrammarController(Random rng)
        {
            CurrentWord = new RootNode();
            m_rng = rng;
        }

        public GrammarNode TransformWordUntilTermination(GenerationParameters buildingParameters, int depthLimit = 10)
        {
            Rules = new List<Rule>
            {
                new RootSplitRule(),
                new TopFloorToRoofRule(),
                new FloorToWallStripRule(buildingParameters as BuildingsGenerationParameters),
                new WallStripToSegmentsRule(),
                new SegmentsToDoorsRule(buildingParameters.RandomGenerator),
            };

            foreach (var rule in Rules)
            {
                CurrentWord = rule.TransformGrammarTree(CurrentWord, buildingParameters, depthLimit);
            }
            return CurrentWord;    
        }
    }

    abstract class Rule
    {
        public ISet<Type> Antedecent { get; protected set; }
        public Rule(params Type[] antedecentTypes)
        {
            Antedecent = new HashSet<Type>(antedecentTypes);
        }
        public abstract GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit);
        
        protected GrammarNode ApplyRecursively(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            for(int c = 0; c < node.Subnodes.Count; ++c)
            {
                node.Subnodes[c] = TransformGrammarTree(node.Subnodes[c], parameters, depthLimit - 1);
            }
            return node;
        }
    }

    class RootSplitRule : Rule
    {
        public RootSplitRule() : base(typeof(RootNode))
        {

        }
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            var buildingsParams = parameters as BuildingsGenerationParameters;
            bool isApplicable = Antedecent.Any(t => t == node.GetType());
            if (!isApplicable)
            {
                return ApplyRecursively(node, buildingsParams, depthLimit);
            }

            int f = 0;
            if (buildingsParams.FloorsNumber > 0)
            {
                AddFloor(node, FloorMark.Ground);
                ++f;
            }
            while (f < buildingsParams.FloorsNumber - 1)
            {
                AddFloor(node, FloorMark.None);
                ++f;
            }
            if (f < buildingsParams.FloorsNumber)
            {
                AddFloor(node, FloorMark.Top);
                ++f;
            }
            return ApplyRecursively(node, buildingsParams, depthLimit);
        }

        public void AddFloor(GrammarNode destination, FloorMark layerType)
        {
            destination.Subnodes.Add(new FloorNode(layerType));
        }
    }

    class TopFloorToRoofRule : Rule
    {
        public TopFloorToRoofRule() : base(typeof(FloorNode))
        {

        }

        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            var buildingsParams = parameters as BuildingsGenerationParameters;
            bool isApplicable = Antedecent.Any(t => t == node.GetType());
            if (!isApplicable || (node as FloorNode).FloorType != FloorMark.Top)
            {
                return ApplyRecursively(node, buildingsParams, depthLimit);
            }
            return new RoofNode(buildingsParams.RoofHeight, buildingsParams.RoofStyle);
        }
    }

    class FloorToWallStripRule : Rule
    {
        private List<int> m_segmentsPerWall;
        public FloorToWallStripRule(BuildingsGenerationParameters parameters) : base(typeof(FloorNode))
        {
            // get segments number per wall
            var doorWallLength = parameters.BasementPoints[parameters.DoorWall.PointIdx1]
                .DistanceTo(parameters.BasementPoints[parameters.DoorWall.PointIdx2]);

            var segmentLength = doorWallLength / parameters.SegmentsOnSelectedWall;

            m_segmentsPerWall = new List<int>();
            for (int p = 0; p < parameters.BasementPoints.Count - 1; ++p)
            {
                var p1 = parameters.BasementPoints[p];
                var p2 = parameters.BasementPoints[p + 1];
                var d = p1.DistanceTo(p2);
                m_segmentsPerWall.Add((int)(d / segmentLength));
            }
            var dlast = parameters.BasementPoints.Last().DistanceTo(parameters.BasementPoints[0]);
            m_segmentsPerWall.Add((int)(dlast / segmentLength));
        }

        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            var bparams = parameters as BuildingsGenerationParameters;
            bool isApplicable = Antedecent.Any(t => t == node.GetType());
            if (!isApplicable) {
                return ApplyRecursively(node, parameters, depthLimit);
            }
            var floorNode = node as FloorNode;
            var firstAddedIdx = node.Subnodes.Count;
            foreach (var segmentsNumber in m_segmentsPerWall)
            {
                node.Subnodes.Add(new WallStripNode(segmentsNumber) { WallType = WallMark.None });
            }
            (node.Subnodes[firstAddedIdx + bparams.DoorWall.PointIdx1] as WallStripNode)
                .WallType = WallMark.Parade;
            return ApplyRecursively(node, bparams, depthLimit);
        }
    }

    class WallStripToSegmentsRule : Rule
    {
        public WallStripToSegmentsRule() : base(typeof(WallStripNode))
        {

        }
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            var bparams = parameters as BuildingsGenerationParameters;
            bool isApplicable = Antedecent.Any(t => t == node.GetType());
            if (!isApplicable)
            {
                return ApplyRecursively(node, parameters, depthLimit);
            }
            var wallStrip = node as WallStripNode;
            for(int s = 0; s < wallStrip.SegmentsNumber; ++s)
            {
                wallStrip.Subnodes.Add(new SegmentNode());
            }
            return ApplyRecursively(node, parameters, depthLimit);
        }
    }

    class SegmentsToDoorsRule : Rule
    {
        private Random m_rng;
        public SegmentsToDoorsRule(Random rng) : base(typeof(SegmentNode))
        {
            m_rng = rng;
        }
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            var bparams = parameters as BuildingsGenerationParameters;
            var paradeGroundWallStrips = new List<WallStripNode>();
            TraverseForDoorsMetadata(node, paradeGroundWallStrips, null);
            if (paradeGroundWallStrips.Count == 0) return node;
            var randomWallStrip = paradeGroundWallStrips[m_rng.Next(paradeGroundWallStrips.Count)];
            var candidateSegments = randomWallStrip.Subnodes
                .Select((n, i) => new { Segment = n, Idx = i})
                .Where(m => m.Segment.GetType() == typeof(SegmentNode))
                .ToArray();
            var doorSegment = candidateSegments[m_rng.Next(candidateSegments.Length)];
            randomWallStrip.Subnodes[doorSegment.Idx].Subnodes.Add(new DoorNode()); // todo style
            return node;
        }

        private void TraverseForDoorsMetadata(GrammarNode node, List<WallStripNode> doorCandidateStrips, FloorNode groundFloor)
        {
            if (groundFloor == null)
            {
                if (node is FloorNode)
                {
                    var floor = node as FloorNode;
                    if (floor.FloorType == FloorMark.Ground)
                    {
                        groundFloor = floor;
                    }
                }
            } 
            else
            {
                if (node is WallStripNode)
                {
                    var wallStrip = node as WallStripNode;
                    if (wallStrip.WallType == WallMark.Parade)
                    {
                        doorCandidateStrips.Add(wallStrip);
                    }
                }
            }
            foreach(var child in node.Subnodes)
            {
                TraverseForDoorsMetadata(child, doorCandidateStrips, groundFloor);
            }
        }

        private struct WallStripInfo
        {
            public GrammarNode FloorNode;
            public int ChildIdx;
        }
    }

    class SegmentToWindowsRule : Rule
    {
        private BuildingsGenerationParameters m_parameters;
        public SegmentToWindowsRule(BuildingsGenerationParameters parameters) : base(typeof(SegmentNode))
        {
            m_parameters = parameters;
        }
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            throw new NotImplementedException();
        }
    }
}
