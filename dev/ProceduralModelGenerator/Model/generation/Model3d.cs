using g3;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class Model3d
    {
        public DMesh3 Mesh { get; set; }
        public string Name { get; set; }

        public Model3d()
        {
            Mesh = new DMesh3();
            Name = "No name model";
        }
    }
}
