using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{

    abstract class GrammarRule
    {
        public bool IsTerminated { get; set; }
        public ISet<GrammarNode> ChangedNodes;
        public GrammarRule()
        {
            ChangedNodes = new HashSet<GrammarNode>(); 
        }

        public GrammarNode Apply(GrammarNode node, GenerationParameters parameters, int depthLimit)
        {
            if (IsTerminated || depthLimit == 0) return node;
            node = TransformGrammarTree(node, parameters);
            for (int c = 0; c < node.Subnodes.Count; ++c)
            {
                node.Subnodes[c] = Apply(node.Subnodes[c], parameters, depthLimit - 1);
            }
            return node;
        }
        public abstract GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters);
    }

    class RootSplitRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is RootNode) || ChangedNodes.Contains(node)) return node;
            var bp = parameters as BuildingsGenerationParameters;
            (node as RootNode).Parameters = bp;

            var basementPolygon = MakePolygon3d(bp.BasementPoints);

            var doorWallLength = bp.BasementPoints[bp.DoorWall.PointIdx1]
                .Distance(bp.BasementPoints[bp.DoorWall.PointIdx2]);
            var segmentLength = doorWallLength / bp.SelectedWallSegments;

            var segmentsPerWall = new List<int>();
            for (int p = 0; p < bp.BasementPoints.Count - 1; ++p)
            {
                var p1 = bp.BasementPoints[p];
                var p2 = bp.BasementPoints[p + 1];
                var d = p1.Distance(p2);
                segmentsPerWall.Add((int)(d / segmentLength));
            }
            var dlast = bp.BasementPoints.Last().Distance(bp.BasementPoints[0]);
            segmentsPerWall.Add((int)(dlast / segmentLength));

            var regularFloorHeight = bp.BasementExtrudeHeight / bp.FloorsNumber;
            for (int f = 0; f < bp.FloorsNumber + 1; ++f)
            {
                var type = f == 0 ? FloorMark.Ground :
                    (f == bp.FloorsNumber ? FloorMark.Top : FloorMark.None);
                var height = type == FloorMark.Top ? bp.RoofHeight : regularFloorHeight;
                node.Subnodes.Add(new FloorNode {
                    FloorType = type,
                    Height = height,
                    BaseShape = basementPolygon,
                    SegmentWidth = segmentLength,
                    SegmentsPerWall = segmentsPerWall,
                });
                basementPolygon = Geometry.OffsetPolygon(basementPolygon, height);
            }
            ChangedNodes.Add(node);
            //this.IsTerminated = true;
            return node;
        }

        private IList<Vector3d> MakePolygon3d(IList<Point2d> polygon)
        {
            return polygon.Select(p => new Vector3d(p.X, 0.0, p.Y)).ToList();
        }
        
    }

    class TopFloorToRoofRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is FloorNode) || ChangedNodes.Contains(node)) return node;
            var floor = (node as FloorNode);
            var buildingsParams = parameters as BuildingsGenerationParameters;
            if (floor.FloorType != FloorMark.Top) return node;
            ChangedNodes.Add(node);
            return new RoofNode {
                RoofHeight = buildingsParams.RoofHeight,
                RoofStyle = buildingsParams.RoofStyle,
                BaseShape = Geometry.OffsetPolygon(floor.BaseShape, floor.Height),
                Normal = Vector3d.AxisY, // todo: universally
            };
        }
    }

    class FloorToWallStripRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is FloorNode) || ChangedNodes.Contains(node)) return node;
            var floor = (node as FloorNode);
            var buildingsParams = parameters as BuildingsGenerationParameters;
            var firstAddedIdx = floor.Subnodes.Count;
            for(int w = 0; w < floor.SegmentsPerWall.Count; ++w)
            {
                var nextWallPoint = floor.BaseShape[(w + 1) % floor.BaseShape.Count];
                var alongWallDirection = nextWallPoint - floor.BaseShape[w];
                node.Subnodes.Add(new WallStripNode {
                    WallType = WallMark.None,
                    FloorType = floor.FloorType,
                    SegmentsNumber = floor.SegmentsPerWall[w],
                    Height = floor.Height,
                    Width = floor.SegmentWidth,
                    Origin = floor.BaseShape[w],
                    FrontNormal = alongWallDirection.UnitCross(Vector3d.AxisY),
                    AlongWidthDirection = alongWallDirection,
                });
            }
            var doorWallIdx = firstAddedIdx + buildingsParams.DoorWall.PointIdx1;
            var doorWallNode = node.Subnodes[doorWallIdx] as WallStripNode;
            doorWallNode.WallType = WallMark.Parade;
            ChangedNodes.Add(node);
            return node;
        }
    }

    class WallStripToSegmentRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is WallStripNode) || ChangedNodes.Contains(node)) return node;
            var wallStrip = node as WallStripNode;
            var bparams = parameters as BuildingsGenerationParameters;

            var segmentOrigin = wallStrip.Origin;
            var segmentWidth = wallStrip.Width / wallStrip.SegmentsNumber;
            for (int s = 0; s < wallStrip.SegmentsNumber; ++s)
            {
                wallStrip.Subnodes.Add(new SegmentNode
                {
                    Width = segmentWidth,
                    Height = wallStrip.Height,
                    Origin = segmentOrigin,
                    FrontNormal = wallStrip.FrontNormal,
                    FloorType = wallStrip.FloorType,
                    WallType = wallStrip.WallType,
                    AlongWidthDirection = wallStrip.AlongWidthDirection,
                });
                segmentOrigin += wallStrip.AlongWidthDirection * segmentWidth;
            }
            ChangedNodes.Add(node);
            return node;
        }
    }

    // todo: AssetsScaleModifier
    class SegmentToDoorsRule : GrammarRule
    {
        private bool m_isGroundDoorPlaced = false;
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is SegmentNode) || ChangedNodes.Contains(node)) return node;
            var segment = node as SegmentNode;
            var isGroundParadeSegment = segment.WallType == WallMark.Parade && segment.FloorType == FloorMark.Ground;
            var isGroundParadeCandidate = isGroundParadeSegment && !m_isGroundDoorPlaced;
            if (!isGroundParadeCandidate && !segment.IsDoorRequired) return node;

            if (isGroundParadeSegment)
            {
                m_isGroundDoorPlaced = true;
            }

            var bparams = parameters as BuildingsGenerationParameters;
            var randomDoorAssetIdx = bparams.RandomGenerator.Next(bparams.DoorsAssets.Count);

            var segmentBottomCenter = segment.Origin + segment.AlongWidthDirection * segment.Width / 2.0;
            segment.Subnodes.Add(new DoorNode
            {
                Asset = bparams.DoorsAssets[randomDoorAssetIdx],
                FrontNormal = segment.FrontNormal,
                Origin = segmentBottomCenter,
                Height = segment.Height * 0.8, // todo: why hardcoded
            });
            ChangedNodes.Add(node);
            return node;
        }
    }

    class SegmentsToWindowsRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters)
        {
            if (!(node is SegmentNode) || ChangedNodes.Contains(node)) return node;
            var segment = node as SegmentNode;
            var bparams = parameters as BuildingsGenerationParameters;
            //bparams.IsVerticalWindowSymmetryPreserved
            //bparams.WindowsToSegmentsFraction
            var randomWindowAssetIdx = bparams.RandomGenerator.Next(bparams.WindowsAssets.Count);
            var segmentBottomCenter = segment.Origin + segment.AlongWidthDirection * segment.Width / 2.0;
            var segmentCenter = segmentBottomCenter + Vector3d.AxisY * segment.Height * 0.5; // todo: hardcode
            segment.Subnodes.Add(new WindowNode
            {
                Asset = bparams.WindowsAssets[randomWindowAssetIdx],
                FrontNormal = segment.FrontNormal,
                Origin = segmentCenter, // todo: hardcode
                Height = segment.Height * 0.6,  // todo: hardcode 
            });
            ChangedNodes.Add(node);
            return node;
        }
    }
}

    //var paradeGroundWallStrips = new List<WallStripNode>();
    //TraverseForDoorsMetadata(node, paradeGroundWallStrips, null);
    //if (paradeGroundWallStrips.Count == 0) return node;
    //var randomWallStrip = paradeGroundWallStrips[m_rng.Next(paradeGroundWallStrips.Count)];
    //var candidateSegments = randomWallStrip.Subnodes
    //    .Select((n, i) => new { Segment = n, Idx = i})
    //    .Where(m => m.Segment.GetType() == typeof(SegmentNode))
    //    .ToArray();
    //var doorSegment = candidateSegments[m_rng.Next(candidateSegments.Length)];
    //randomWallStrip.Subnodes[doorSegment.Idx].Subnodes.Add(new DoorNode()); // todo style
    //return node;
//}

//private void TraverseForDoorsMetadata(GrammarNode node, List<WallStripNode> doorCandidateStrips, FloorNode groundFloor)
//{
//    if (groundFloor == null)
//    {
//        if (node is FloorNode)
//        {
//            var floor = node as FloorNode;
//            if (floor.FloorType == FloorMark.Ground)
//            {
//                groundFloor = floor;
//            }
//        }
//    }
//    else
//    {
//        if (node is WallStripNode)
//        {
//            var wallStrip = node as WallStripNode;
//            if (wallStrip.WallType == WallMark.Parade)
//            {
//                doorCandidateStrips.Add(wallStrip);
//            }
//        }
//    }
//    foreach (var child in node.Subnodes)
//    {
//        TraverseForDoorsMetadata(child, doorCandidateStrips, groundFloor);
//    }
//}

//private struct WallStripInfo
//{
//    public GrammarNode FloorNode;
//    public int ChildIdx;
//}