using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    public struct Point2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Distance(Point2d other)
        {
           return Math.Sqrt(
               Math.Pow(this.X - other.X, 2) +
               Math.Pow(this.Y - other.Y, 2)
           );
        }
    } 

}
