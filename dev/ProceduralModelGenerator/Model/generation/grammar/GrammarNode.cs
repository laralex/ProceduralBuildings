using g3;
using System.Collections.Generic;
using System.Linq;

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

        public abstract bool BuildOnMesh(DMesh3 mesh);

    }

    public class RootNode : GrammarNode
    {
        public new readonly string Code = "Initial node";
        public BuildingsGenerationParameters Parameters;

        public override bool BuildOnMesh(DMesh3 mesh)
        {
            MeshUtility.FillPolygon(mesh, Parameters.BasementPoints
                .Select(p => new Vector3d(p.X, 0.0, p.Y)).ToList(),
                -Vector3f.AxisY);
            //for (int v = 0; v < Parameters.BasementPoints.Count; ++v)
            //{
            //    newVertices[v] = mesh.AppendVertex(new Vector3d(
            //        Parameters.BasementPoints[v].X,
            //        0.0,
            //        Parameters.BasementPoints[v].Y
            //    ));
            //}

            //var base2d = BaseShape.Select(p => p.xz).ToList();
            
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
        public override bool BuildOnMesh(DMesh3 mesh)
        {
            int[] newVertices = new int[BaseShape.Count];
            for (int v = 0; v < BaseShape.Count; ++v)
            {
                newVertices[v] = mesh.AppendVertex(BaseShape[v]);
            }
            MeshUtility.AddTriangleStripBetweenPolygons(mesh, newVertices,
                BaseShape.Select(v => v + Vector3d.AxisY * Height).ToList());

            //AddTriangleStripBetweenPolygons(mesh, )
            return true;
            //var heightExtruder = new MeshExtrudeMesh(mesh);
            //heightExtruder.ExtrudedPositionF = (pos, normal, idx) =>
            //{
            //    return pos + Vector3d.AxisY * buildingParams.BasementExtrudeHeight;
            //};
            //heightExtruder.Extrude();

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
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public Vector3d AlongWidthDirection { get; set; }
        public int FloorIdx { get; set; }
        public int WallIdx { get; set; }
        public override bool BuildOnMesh(DMesh3 mesh)
        {
            return false;
        }
    }

    public class RoofNode : GrammarNode
    {
        public new readonly string Code = "Roof";
        public IList<Vector3d> BaseShape { get; set; }
        public Vector3d Normal { get; set; }
        public double RoofHeight { get; set; }
        public RoofStyle RoofStyle { get; set; }

        public override bool BuildOnMesh(DMesh3 mesh)
        {
            MeshUtility.FillPolygon(mesh, BaseShape, Vector3f.AxisY); // todo: bad move
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
        public override bool BuildOnMesh(DMesh3 mesh)
        {
            return false;
        }
    }

    public class DoorNode : GrammarNode
    {
        public new readonly string Code = "Door";
        public double Height { get; set; }
        // width is defined from height to prevent distortion
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public Asset Asset { get; set; }
        public override bool BuildOnMesh(DMesh3 mesh)
        {
            return false;
        }
    }

    public class WindowNode : GrammarNode
    {
        public new readonly string Code = "Window";
        public double Height { get; set; }
        // width is defined from height to prevent distortion
        public Vector3d Origin { get; set; }
        public Vector3d FrontNormal { get; set; }
        public Asset Asset { get; set; }
        public override bool BuildOnMesh(DMesh3 mesh)
        {
            return false;
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