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
        public bool ContinueRecursion { get; set; }
        public ISet<GrammarNode> ChangedNodes;
        public GrammarRule()
        {
            ChangedNodes = new HashSet<GrammarNode>(); 
        }

        public GrammarNode Apply(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes, int depthLimit)
        {
            if (IsTerminated || depthLimit == 0) return node;
            node = TransformGrammarTree(node, parameters, assetsMeshes);
            if (IsTerminated) return node;
            if (ContinueRecursion)
            {
                for (int c = 0; c < node.Subnodes.Count; ++c)
                {
                    node.Subnodes[c] = Apply(node.Subnodes[c], parameters, assetsMeshes, depthLimit - 1);
                }
            }
            else
            {
                ContinueRecursion = true;
            }
            return node;
        }
        public abstract GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes);
    }

    class RootSplitRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            if (!(node is RootNode) || ChangedNodes.Contains(node)) return node;
            var bp = parameters as BuildingsGenerationParameters;
            (node as RootNode).Parameters = bp;

            var basementPolygon = MakePolygon3d(bp.BasementPoints);

            var doorWallLength = bp.BasementPoints[bp.DoorWall.PointIdx1]
                .Distance(bp.BasementPoints[bp.DoorWall.PointIdx2]);
            var segmentLength = doorWallLength / bp.SelectedWallSegments;

            var segmentsPerWall = new List<int>();
            var windowsPerWall = new List<int>();
            for (int p = 0; p < bp.BasementPoints.Count - 1; ++p)
            {
                var p1 = bp.BasementPoints[p];
                var p2 = bp.BasementPoints[p + 1];
                var d = p1.Distance(p2);
                segmentsPerWall.Add((int)(d / segmentLength));
                windowsPerWall.Add((int)(segmentsPerWall.Last() * bp.WindowsToSegmentsFraction)); 
            }
            var dlast = bp.BasementPoints.Last().Distance(bp.BasementPoints[0]);
            segmentsPerWall.Add((int)(dlast / segmentLength));
            windowsPerWall.Add((int)(segmentsPerWall.Last() * bp.WindowsToSegmentsFraction)); 

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
                    WindowsPerWall = windowsPerWall,
                    FloorIdx = f,
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
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            if (!(node is FloorNode) || ChangedNodes.Contains(node)) return node;
            var floor = (node as FloorNode);
            var buildingsParams = parameters as BuildingsGenerationParameters;
            if (floor.FloorType != FloorMark.Top) return node;
            ChangedNodes.Add(node);
            return new RoofNode {
                RoofHeight = buildingsParams.RoofHeight,
                RoofStyle = buildingsParams.RoofStyle,
                BaseShape = floor.BaseShape,
                Normal = Vector3d.AxisY, // todo: universally
            };
        }
    }

    class FloorToWallStripRule : GrammarRule
    {
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            if (!(node is FloorNode) || ChangedNodes.Contains(node)) return node;
            var floor = (node as FloorNode);
            var buildingsParams = parameters as BuildingsGenerationParameters;
            var firstAddedIdx = floor.Subnodes.Count;
            for(int w = 0; w < floor.SegmentsPerWall.Count; ++w)
            {
                var nextWallPoint = floor.BaseShape[(w + 1) % floor.BaseShape.Count];
                var wallHorizontalSide = nextWallPoint - floor.BaseShape[w];
                var wallWidth = wallHorizontalSide.Length;
                var alongWallDirection = wallHorizontalSide.Normalized;
                node.Subnodes.Add(new WallStripNode {
                    WallType = WallMark.None,
                    FloorType = floor.FloorType,
                    SegmentsNumber = floor.SegmentsPerWall[w],
                    WindowsNumber = floor.WindowsPerWall[w],
                    Height = floor.Height,
                    Width = wallWidth,
                    Origin = floor.BaseShape[w],
                    FrontNormal = Vector3d.AxisY.UnitCross(alongWallDirection),
                    AlongWidthDirection = alongWallDirection,
                    FloorIdx = floor.FloorIdx,
                    WallIdx = w,
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
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
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
                    WallIdx = wallStrip.WallIdx,
                    SegmentIdx = s,
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
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            if (!(node is WallStripNode) || ChangedNodes.Contains(node)) return node;
            var wallStrip = node as WallStripNode;
            var isGroundParadeStrip = wallStrip.WallType == WallMark.Parade && wallStrip.FloorType == FloorMark.Ground;
            var isGroundParadeCandidate = isGroundParadeStrip && !m_isGroundDoorPlaced;
            if (!isGroundParadeCandidate) return node; // !segment.IsDoorRequired
            if (isGroundParadeStrip)
            {
                m_isGroundDoorPlaced = true;
            }

            //if (!(node is SegmentNode) || ChangedNodes.Contains(node)) return node;
            //var segment = node as SegmentNode;
            //var isGroundParadeSegment = segment.WallType == WallMark.Parade && segment.FloorType == FloorMark.Ground;

            //if (isGroundParadeStrip)
            //{
            //    m_isGroundDoorPlaced = true;
            //}
            var segmentIdx = parameters.RandomGenerator.Next(wallStrip.SegmentsNumber);
            var segment = wallStrip.Subnodes
                .Where(n => n is SegmentNode)
                .ToArray()[segmentIdx] as SegmentNode; 

            var bparams = parameters as BuildingsGenerationParameters;
            var randomDoorAssetIdx = bparams.RandomGenerator.Next(bparams.DoorsAssets.Count);

            var segmentBottomCenter = segment.Origin + segment.AlongWidthDirection * segment.Width / 2.0;
            segment.Subnodes.Add(new DoorNode
            {
                Mesh = assetsMeshes?[bparams.DoorsAssets[randomDoorAssetIdx]],
                FrontNormal = segment.FrontNormal,
                Origin = segmentBottomCenter,
                HeightLimit = segment.Height * 0.8, // todo: why hardcoded
                WidthLimit = segment.Width, // todo: why hardcoded
            });
            ChangedNodes.Add(node);
            ChangedNodes.Add(segment);
            return node;
        }
    }

    class SegmentsToWindowsRule : GrammarRule
    {
        private Dictionary<int, ISet<int>> m_verticalSymmetryRegistry = new Dictionary<int, ISet<int>>();
        public override GrammarNode TransformGrammarTree(GrammarNode node, GenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            if (!(node is WallStripNode) || ChangedNodes.Contains(node)) return node;
            var bparams = parameters as BuildingsGenerationParameters;
            var wall = node as WallStripNode;

            // add wall in registry
            if (!m_verticalSymmetryRegistry.ContainsKey(wall.WallIdx))
            {
                m_verticalSymmetryRegistry[wall.WallIdx] = new HashSet<int>();
            }

            IEnumerable<int> slotsIndices;
            if (bparams.IsVerticalWindowSymmetryPreserved)
            {
                // if no columns selected, select them randomly
                if (m_verticalSymmetryRegistry[wall.WallIdx].Count == 0)
                {
                    m_verticalSymmetryRegistry[wall.WallIdx] = new HashSet<int>(
                        SelectRandomSegments(wall.SegmentsNumber, wall.WindowsNumber, bparams.RandomGenerator)    
                    );
                }
                // take columns which have to be filled
                slotsIndices = m_verticalSymmetryRegistry[wall.WallIdx];
            }
            else
            {
                // take columns randomly for this strip
                slotsIndices = SelectRandomSegments(wall.SegmentsNumber, wall.WindowsNumber, bparams.RandomGenerator);
            }

            var segmentsInWall = wall.Subnodes.Where(n => n is SegmentNode).ToArray();
            foreach (var slotIdx in slotsIndices)
            {
                if (slotIdx < segmentsInWall.Length)
                {
                    AddWindow(segmentsInWall[slotIdx] as SegmentNode, bparams, assetsMeshes);
                }
            }

            ChangedNodes.Add(node);
            ContinueRecursion = false;
            return node;
        }

        public void AddWindow(SegmentNode segment, BuildingsGenerationParameters parameters, Dictionary<Asset, DMesh3> assetsMeshes)
        {
            var randomWindowAssetIdx = parameters.RandomGenerator.Next(parameters.WindowsAssets.Count);
            var segmentBottomCenter = segment.Origin + segment.AlongWidthDirection * segment.Width / 2.0;
            var segmentCenter = segmentBottomCenter + Vector3d.AxisY * segment.Height * 0.5; // todo: hardcode
            segment.Subnodes.Add(new WindowNode
            {
                Mesh = assetsMeshes?[parameters.WindowsAssets[randomWindowAssetIdx]],
                FrontNormal = segment.FrontNormal,
                Origin = segmentCenter, // todo: hardcode
                HeightLimit = segment.Height * 0.75,  // todo: hardcode 
                WidthLimit = segment.Width,
            });
            ChangedNodes.Add(segment);
        }

        public IEnumerable<int> SelectRandomSegments(int segmentsNumber, int sampleSize, Random rng)
        {
            return Enumerable.Range(0, segmentsNumber)
                .OrderBy(e => rng.Next())
                .Take(sampleSize);
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