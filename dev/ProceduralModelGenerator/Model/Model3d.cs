using g3;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProceduralBuildingsGeneration
{
    public class Model3D
    {
        public DMesh3 Mesh { get; set; }
        public string Name { get; set; }

        public Model3D()
        {
            Mesh = new DMesh3();
            Name = "No name model";
        }
    }
}
