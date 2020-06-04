using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    public abstract class GrammarNode
    {
        public readonly string Code;
        public IList<GrammarNode> Subnodes { get; private set; }

        public GrammarNode(IList<GrammarNode> subnodes = null)
        {
            Subnodes = subnodes ?? new List<GrammarNode>();
        }

        public void RemoveSubnodes(Func<GrammarNode, bool> predicate)
        {
            Subnodes = Subnodes.Where(n => !predicate(n)).ToList();
        }
        public abstract bool BuildOnMesh(DMesh3Builder meshBuilder);

    }

    public class AbstractNode : GrammarNode
    {
        public new readonly string Code = "Abstract 'marker' node";
        public sealed override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            return false;
        }
    }

    public class LadderEndSegment : AbstractNode
    {
        public new readonly string Code = "Marks a segment as a ladder end";
    }

    public class RootNode : GrammarNode
    {
        public new readonly string Code = "Initial node";
        public BuildingsGenerationParameters Parameters;

        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            MeshUtility.FillPolygon(meshBuilder, Parameters.BasementPoints
                .Select(p => new Vector3d(p.X, 0.0, p.Y)).ToList(),
                                    -Vector3f.AxisY);
            return true;
        }
    }

    public enum FloorMark
    {
        None, Ground, Top
    }

    public class FloorNode : GrammarNode
    {
        public new readonly string Code = "Floor";
        public FloorMark FloorType { get; set; }
        public double Height { get; set; }
        public double SegmentWidth { get; set; }
        public IList<int> SegmentsPerWall { get; set; }
        public IList<int> WindowsPerWall { get; set; }
        public IList<Vector3d> BaseShape { get; set; }
        public int FloorIdx { get; set; }
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            return false;
        }
    }

    public enum WallMark
    {
        None, Parade
    }
    public class WallStripNode : GrammarNode
    {
        public new readonly string Code = "Wall side of a floor";

        public WallMark WallType { get; set; }
        public FloorMark FloorType { get; set; }
        public int SegmentsNumber { get; set; }
        public int WindowsNumber { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double SegmentWidth { get; set; }
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public Vector3d AlongWidthDirection { get; set; }
        public int FloorIdx { get; set; }
        public int WallIdx { get; set; }
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            Vector3d v00 = Origin, v01 = Origin, v10 = Origin;
            v01.y += Height;
            v10 += AlongWidthDirection * Width;
            Vector3d v11 = v10;
            v11.y += Height;

            var vInfo = new NewVertexInfo
            {
                bHaveN = true,
                n = new Vector3f(FrontNormal),
            };

            int i00 = AppentVertex(meshBuilder, vInfo, v00);
            int i01 = AppentVertex(meshBuilder, vInfo, v01);
            int i10 = AppentVertex(meshBuilder, vInfo, v10);
            int i11 = AppentVertex(meshBuilder, vInfo, v11);

            meshBuilder.AppendTriangle(i00, i11, i01);
            meshBuilder.AppendTriangle(i00, i10, i11);
            return true;
        }

        public int AppentVertex(DMesh3Builder meshBuilder, NewVertexInfo vInfo, Vector3d vertex)
        {
            vInfo.v = vertex;
            return meshBuilder.AppendVertex(vInfo);
        }
    }

    public abstract class RoofNode : GrammarNode
    {
        public new readonly string Code = "Roof";
        public IList<Vector3d> BaseShape { get; set; }
        public Vector3d Normal { get; set; }
        public double RoofHeight { get; set; }
        public float RoofEdgeOffset { get; set; }
        public static IList<Vector3d> CompressAndOffsetPolygon(IList<Vector3d> polygon, double compressionCoef, Vector3d offset)
        {
            var compressedPolygon = Geometry.CompressPolygon(polygon, compressionCoef);
            return compressedPolygon.Select(p => p + offset).ToList();
            //var shapeProjection = polygon.Select(p => new Point2d { X = p.x, Y = p.z }).ToList();
            //var smallerPolygon = Geometry2d.ScaleCenteredPolygon(Geometry2d.CenterPolygon(shapeProjection, out var centroid), scale);
            //offset.y += polygon[0].y;
            //return smallerPolygon.Select(p => new Vector3d(p.X, 0, p.Y) + offset).ToList();
        }

        public static void FillBetweenPolygons(DMesh3Builder meshBuilder, IList<Vector3d> polygon1, IList<Vector3d> polygon2)
        {
            for (int p = 0; p < polygon1.Count; ++p)
            {
                var nextP = ((p + 1) + polygon1.Count) % polygon1.Count;
                var sideBase = polygon1[nextP] - polygon1[p];
                var sideTop = polygon2[nextP] - polygon2[p];
                var normal = new Vector3f(sideTop.UnitCross(sideBase));
                MeshUtility.FillBetweenEdges(meshBuilder,
                    new MeshUtility.Edge(polygon1[p], polygon1[nextP]),
                    new MeshUtility.Edge(polygon2[p], polygon2[nextP]),
                    normal);
            }
        }
    }

    public class FlatRoofNode : RoofNode
    {
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            var elevation = Vector3d.AxisY * RoofHeight;
            var topOuterPolygon = BaseShape.Select(p => p + elevation).ToList();
            for (int p = 0; p < BaseShape.Count; ++p)
            {
                var nextP = ((p + 1) + BaseShape.Count) % BaseShape.Count;
                var side = BaseShape[nextP] - BaseShape[p];
                var normal = new Vector3f(side.UnitCross(Vector3d.AxisY));
                MeshUtility.FillBetweenEdges(meshBuilder,
                    new MeshUtility.Edge(BaseShape[p], BaseShape[nextP]),
                    new MeshUtility.Edge(topOuterPolygon[p], topOuterPolygon[nextP]),
                    normal);
            }

            var topInnerPolygon = CompressAndOffsetPolygon(topOuterPolygon, RoofEdgeOffset, Vector3d.Zero);
            //var splitPolygon = Geometry.BreakPolygonSelfIntersection(topInnerPolygon);
            FillBetweenPolygons(meshBuilder, topOuterPolygon, topInnerPolygon);

            var innerPolygon = CompressAndOffsetPolygon(BaseShape, RoofEdgeOffset, elevation * 0.4);

            for (int p = 0; p < topInnerPolygon.Count; ++p)
            {
                var nextP = ((p + 1) + BaseShape.Count) % BaseShape.Count;
                var side = topInnerPolygon[nextP] - topInnerPolygon[p];
                var normal = new Vector3f(Vector3d.AxisY.UnitCross(side));
                MeshUtility.FillBetweenEdges(meshBuilder,
                    new MeshUtility.Edge(topInnerPolygon[p], topInnerPolygon[nextP]),
                    new MeshUtility.Edge(innerPolygon[p], innerPolygon[nextP]),
                    normal);
            }
            MeshUtility.FillPolygon(meshBuilder, innerPolygon, Vector3f.AxisY); // todo: bad move
            return true;
        }
    }

    public class SlopeRoofNode : RoofNode
    {
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            
            return true;
        }
    }

    public class FlatSlopeRoofNode : RoofNode
    {
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            var intrude = double.MaxValue;
            for(int p = 0; p < BaseShape.Count; ++p)
            {
                var nextP = ((p + 1) + BaseShape.Count) % BaseShape.Count;
                //intrude += (BaseShape[nextP] - BaseShape[p]).Length;
                intrude = Math.Min(intrude, (BaseShape[nextP] - BaseShape[p]).Length);
            }
            //intrude /= BaseShape.Count;
            intrude /= 5;

            // scale up base shape and connect to base
            var newBasePolygon = CompressAndOffsetPolygon(BaseShape, -intrude/2, Vector3d.Zero);
            FillBetweenPolygons(meshBuilder, BaseShape, newBasePolygon);
            
            // elevale big shape a bit up and connect to big shape
            var elevation = Vector3d.AxisY * RoofHeight * 0.15;
            var topOuterPolygon = newBasePolygon.Select(p => p + elevation).ToList();
            for (int p = 0; p < newBasePolygon.Count; ++p)
            {
                var nextP = ((p + 1) + newBasePolygon.Count) % newBasePolygon.Count;
                var side = newBasePolygon[nextP] - newBasePolygon[p];
                var normal = new Vector3f(side.UnitCross(Vector3d.AxisY));
                MeshUtility.FillBetweenEdges(meshBuilder,
                    new MeshUtility.Edge(newBasePolygon[p], newBasePolygon[nextP]),
                    new MeshUtility.Edge(topOuterPolygon[p], topOuterPolygon[nextP]),
                    normal);
            }

            // find small polygons on top
            var topPolygon = CompressAndOffsetPolygon(BaseShape, intrude, Vector3d.AxisY * RoofHeight * 0.85);
            var brokenSelfIntersections = Geometry.BreakPolygonSelfIntersection(topPolygon);
            //brokenSelfIntersections = Geometry.BreakPolygonSelfIntersection(brokenSelfIntersections);
            //brokenSelfIntersections = Geometry.BreakPolygonSelfIntersection(brokenSelfIntersections);

            // fill some edges of base and top
            for (int p = 0; p < brokenSelfIntersections.Count; ++p)
            {
                var nextP = ((p + 1) + topOuterPolygon.Count) % topOuterPolygon.Count;
                var sideBase = topPolygon[nextP] - topPolygon[p];
                var sideTop = brokenSelfIntersections[nextP].First() - brokenSelfIntersections[p].Last();
                var normal = new Vector3f(sideTop.UnitCross(sideBase));
                MeshUtility.FillBetweenEdges(meshBuilder,
                    new MeshUtility.Edge(topOuterPolygon[p], topOuterPolygon[nextP]),
                    new MeshUtility.Edge(brokenSelfIntersections[p].Last(), brokenSelfIntersections[nextP].First()),
                    normal);
            }

            // fill triangle fan for multi-point
            var subTopPolygon = new List<Vector3d>();
            for(int p = 0; p < brokenSelfIntersections.Count; ++p)
            {
                subTopPolygon.Add(brokenSelfIntersections[p].First());
                if (brokenSelfIntersections[p].Count > 1)
                {
                    MeshUtility.FillPolygon(meshBuilder, subTopPolygon, Vector3f.AxisY);
                    subTopPolygon.Clear();
                    var firstP = topOuterPolygon[p];
                   
                    var limit = brokenSelfIntersections[p].Count;
                    for (int i = 0; i < limit - 1; ++i)
                    {
                        var curP = brokenSelfIntersections[p][i];
                        var nextP = brokenSelfIntersections[p][(i + 1) % limit];
                        var normal = (Vector3f)(firstP - curP).UnitCross(nextP - curP);
                        var firstV = meshBuilder.AppendVertex(new NewVertexInfo
                        {
                            v = firstP,
                            n = normal,
                        });
                        var curV = meshBuilder.AppendVertex(new NewVertexInfo
                        {
                            v = curP,
                            n = normal,
                        });
                        var nextV = meshBuilder.AppendVertex(new NewVertexInfo
                        {
                            v = nextP,
                            n = normal,
                        });
                        meshBuilder.AppendTriangle(firstV, nextV, curV);
                    }
                    subTopPolygon.Add(brokenSelfIntersections[p].Last());
                }
            }
            subTopPolygon.Add(brokenSelfIntersections[0].First());
            MeshUtility.FillPolygon(meshBuilder, subTopPolygon, Vector3f.AxisY);

            return true;
        }

    }

    public class SegmentNode : GrammarNode
    {
        public new readonly string Code = "Part of wall on a floor";
        public double Height { get; set; }
        public double Width { get; set; }
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public Vector3d AlongWidthDirection { get; set; }
        public WallMark WallType { get; set; }
        public FloorMark FloorType { get; set; }
        public bool IsDoorRequired { get; set; }
        public int WallIdx { get; set; }
        public int SegmentIdx { get; set; }
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            return false;
        }
    }

    public class DoorNode : GrammarNode
    {
        public new readonly string Code = "Door";
        public double HeightLimit { get; set; }
        public double WidthLimit { get; set; }
        // width is defined from height to prevent distortion
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public DMesh3 Mesh { get; set; }
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            var doorCopy = new DMesh3(Mesh, bCompact:true);
            if (FrontNormal == -Vector3d.AxisZ)
            {
                // trick to prevent 180 rotation
                FrontNormal += new Vector3d(0.0000001, 0.0, 0.0);
            }

            var meshWidth = doorCopy.GetBounds().Width;
            var meshHeight = doorCopy.GetBounds().Height;

            var widthScale = WidthLimit / meshWidth;
            var heightScale = HeightLimit / meshHeight;

            Quaterniond orientingQuaternion = new Quaterniond(Vector3d.AxisZ, FrontNormal);

            MeshTransforms.Rotate(doorCopy, Vector3d.Zero, orientingQuaternion);
            MeshTransforms.Scale(doorCopy, Math.Min(widthScale, heightScale));
            MeshTransforms.Translate(doorCopy, Origin);

            meshBuilder.AppendNewMesh(doorCopy);
            meshBuilder.SetActiveMesh(0);
            return true;
        }
    }

    public class WindowNode : GrammarNode
    {
        public new readonly string Code = "Window";
        public double HeightLimit { get; set; }
        public double WidthLimit { get; set; }
        // width is defined from height to prevent distortion
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public DMesh3 Mesh { get; set; }
        public Task BuildingTask { get; private set; }
        public override bool BuildOnMesh(DMesh3Builder meshBuilder)
        {
            DMesh3 windowCopy = null;
            BuildingTask = Task.Run(() =>
            {
                windowCopy = new DMesh3(Mesh, bCompact: true);
                //var windowCopy = Mesh;
                if (FrontNormal == -Vector3d.AxisZ)
                {
                    // trick to prevent 180 rotation
                    FrontNormal += new Vector3d(0.0000001, 0.0, 0.0);
                }

                var meshWidth = windowCopy.GetBounds().Width;
                var meshHeight = windowCopy.GetBounds().Height;

                var widthScale = WidthLimit / meshWidth;
                var heightScale = HeightLimit / meshHeight;
                var selectedScale = Math.Min(widthScale, heightScale);

                Quaterniond orientingQuaternion = new Quaterniond(Vector3d.AxisZ, FrontNormal);
                MeshTransforms.Rotate(windowCopy, Vector3d.Zero, orientingQuaternion);

                MeshTransforms.Scale(windowCopy, selectedScale);

                MeshTransforms.Translate(windowCopy, Origin);
                //MeshTransforms.Translate(windowCopy, Origin + Vector3d.AxisY * meshHeight * selectedScale * 0.6);
            }).ContinueWith(t =>
            {
                lock (meshBuilder)
                {
                    meshBuilder.AppendNewMesh(windowCopy);
                    meshBuilder.SetActiveMesh(0);
                }
            });
            
            return true;
        }
    }
}



//public class WallCollectionNode : GrammarNode
//{
//    public new readonly string Code = "All walls";
//    public IList<WallNode> Walls { get; private set; }
//    public WallCollectionNode(int capacity)
//    {
//        Walls = new List<WallNode>(capacity);
//    }

//    public void FillWalls(IEnumerable<int> segmentsPerWall, IEnumerable<int> paradeWalls)
//    {
//        foreach(var segmentsNumber in segmentsPerWall)
//        {
//            Walls.Add(new WallNode(segmentsNumber) { WallType = WallMark.None });
//        }
//        foreach(var paradeIdx in paradeWalls)
//        {
//            Walls[paradeIdx].WallType = WallMark.Parade;
//        }
//    }

//    public override bool BuildOnMesh(DMesh3 mesh, BuildingsGenerationParameters parameters)
//    {
//        return false;
//        //throw new NotImplementedException();
//    }
//}

//int floorsNumber, IList< int > segmentsPerWall, int paradeWall
//FloorsNumber = floorsNumber;
//SegmentsCountPerWall = segmentsPerWall;
//var walls = new WallCollectionNode(segmentsPerWall.Count);
//walls.FillWalls(segmentsPerWall, new List<int> { paradeWall });
//var floors = new FloorCollectionNode(floorsNumber);
//floors.FillFloors(floorsNumber);

//if (!floors.ApplyRuleWith(walls)) throw new Exception("Floors and walls are not compatible");

//Subnodes.Add(floors);
//Subnodes.Add(walls);
//public override bool ApplyRuleWith(GrammarNode interactingNode)
//{
//    return false;
//}

//public override bool ApplyRuleWith(GrammarNode interactingNode)
//{
//    if (interactingNode is FloorNode)
//    {
//        return true;
//    }
//    return false;
//}

//public override bool ApplyRuleWith(GrammarNode interactingNode)
//{
//    if (interactingNode is FloorCollectionNode)
//    {
//        return true;
//    }
//    return false;
//}


//public override bool ApplyRuleWith(GrammarNode interactingNode)
//{
//    if (interactingNode is FloorNode)
//    {
//        return true;
//    }
//    return false;
//}

//public class FloorMiddleNode : FloorNode
//{

//}
//public class FloorTopNode : FloorNode
//{
//    public new readonly string Code = "Top floor";
//}

//public class FloorGroundNode : FloorNode
//{
//    public new readonly string Code = "Ground floor";
//}

//public class FloorCollectionNode : GrammarNode
//{
//    public new readonly string Code = "All floors";
//    public IList<GrammarNode> Floors { get; private set; }
//    public FloorCollectionNode(int capacity)
//    {
//        Floors = new List<GrammarNode>(capacity);
//    }

//    public void AddFloor(FloorMark layerType)
//    {
//        switch (layerType)
//        {
//            case FloorMark.None: Floors.Add(new FloorMiddleNode()); break;
//            case FloorMark.Ground: Floors.Add(new FloorGroundNode()); break;
//            case FloorMark.Top: Floors.Add(new FloorTopNode()); break;
//        }
//    }

//    public void FillFloors(int floorsNumber)
//    {
//        int i = 0;
//        if (floorsNumber > 0)
//        {
//            AddFloor(FloorMark.Ground);
//            ++i;
//        }
//        while (i < floorsNumber - 1)
//        {
//            AddFloor(FloorMark.None);
//            ++i;
//        }
//        if (i < floorsNumber)
//        {
//            AddFloor(FloorMark.Top);
//            ++i;
//        }
//    }
//}
