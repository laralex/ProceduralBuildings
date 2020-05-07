using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    public abstract class GrammarNode
    {
        public readonly string Code;
        //public IDictionary<GrammarNode, IList<GrammarNode>> SubnodesGraph { get; private set; }
        public IList<GrammarNode> Subnodes { get; private set; }

        public GrammarNode(IList<GrammarNode> subnodes = null)
        {
            Subnodes = subnodes != null ? subnodes : new List<GrammarNode>();
        }

        //public abstract bool ApplyRuleWith(GrammarNode interactingNode);
    }

    public class RootNode : GrammarNode
    {
        public new readonly string Code = "Initial node";
        public int FloorsNumber { get; private set; }
        public IList<int> SegmentsCountPerWall { get; private set; }
        public RootNode(IList<GrammarNode> subnodes = null) : base(subnodes)
        {
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
        }

        //public override bool ApplyRuleWith(GrammarNode interactingNode)
        //{
        //    return false;
        //}
    }

    public enum FloorMark
    {
        None, Ground, Top
    }

    public class FloorNode : GrammarNode
    {
        public new readonly string Code = "Floor";

        //public FloorMark FloorType { get; set; }
    }

    public class FloorMiddleNode : FloorNode
    {

    }
    public class FloorTopNode : FloorNode
    {
        public new readonly string Code = "Top floor";
    }

    public class FloorGroundNode : FloorNode
    {
        public new readonly string Code = "Ground floor";
    }

    public class FloorCollectionNode : GrammarNode
    {
        public new readonly string Code = "All floors";
        public IList<GrammarNode> Floors { get; private set; }
        public FloorCollectionNode(int capacity)
        {
            Floors = new List<GrammarNode>(capacity);
        }

        //public override bool ApplyRuleWith(GrammarNode interactingNode)
        //{
        //    if (interactingNode is WallCollectionNode)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public void AddFloor(FloorMark layerType)
        {
            switch (layerType)
            {
                case FloorMark.None: Floors.Add(new FloorMiddleNode()); break;
                case FloorMark.Ground: Floors.Add(new FloorGroundNode()); break;
                case FloorMark.Top: Floors.Add(new FloorTopNode()); break;
            }
        }

        public void FillFloors(int floorsNumber)
        {
            int i = 0;
            if (floorsNumber > 0)
            {
                AddFloor(FloorMark.Ground);
                ++i;
            }
            while (i < floorsNumber - 1)
            {
                AddFloor(FloorMark.None);
                ++i;
            }
            if (i < floorsNumber)
            {
                AddFloor(FloorMark.Top);
                ++i;
            }
        }
    }


    public enum WallMark
    {
        None, Parade
    }
    public class WallNode : GrammarNode
    {
        public new readonly string Code = "Wall";

        public WallMark WallType { get; set; }
        public int SegmentsNumber { get; private set; }
        public WallNode(int segmentsNumber)
        {
            SegmentsNumber = segmentsNumber;
        }

        //public override bool ApplyRuleWith(GrammarNode interactingNode)
        //{
        //    if (interactingNode is FloorNode)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }

    public class WallCollectionNode : GrammarNode
    {
        public new readonly string Code = "All walls";
        public IList<WallNode> Walls { get; private set; }
        public WallCollectionNode(int capacity)
        {
            Walls = new List<WallNode>(capacity);
        }

        //public override bool ApplyRuleWith(GrammarNode interactingNode)
        //{
        //    if (interactingNode is FloorCollectionNode)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public void FillWalls(IEnumerable<int> segmentsPerWall, IEnumerable<int> paradeWalls)
        {
            foreach(var segmentsNumber in segmentsPerWall)
            {
                Walls.Add(new WallNode(segmentsNumber) { WallType = WallMark.None });
            }
            foreach(var paradeIdx in paradeWalls)
            {
                Walls[paradeIdx].WallType = WallMark.Parade;
            }
        }
    }

    public class RoofNode : GrammarNode
    {
        //public override bool ApplyRuleWith(GrammarNode interactingNode)
        //{
        //    if (interactingNode is FloorNode)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public double RoofHeight { get; private set; }
        public RoofStyle RoofStyle { get; private set; }

        public RoofNode(double roofHeight, RoofStyle roofStyle)
        {
            this.RoofHeight = roofHeight;
            this.RoofStyle = roofStyle;
        }
    }

}
