using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfVisualizer
{
    public class ContourUtility
    {
        public static double Diameter = 0.5; 
        private static Plane3D ContourPlane;
        private static ModelVisual3D ContourModel;
        public static ModelVisual3D AddContours(Visual3D model1, int o, int m, int n)
        {
            ContourModel = new ModelVisual3D();
            var bounds = Visual3DHelper.FindBounds(model1, Transform3D.Identity);
            for (int i = 1; i < n; i++)
            {
                ContourPlane = new Plane3D(new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i / n), new Vector3D(0, 0, 1));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            for (int i = 1; i < m; i++)
            {
                ContourPlane = new Plane3D(new Point3D(0, bounds.Location.Y + bounds.Size.Y * i / m, 0), new Vector3D(0, 1, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            for (int i = 1; i < o; i++)
            {
                ContourPlane = new Plane3D(new Point3D(bounds.Location.X + bounds.Size.X * i / o, 0, 0), new Vector3D(1, 0, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            return ContourModel;
        }

        public static void AddContours(GeometryModel3D model, Transform3D transform)
        {
            var p = ContourPlane.Position;
            var n = ContourPlane.Normal;
            var segments = MeshGeometryHelper.GetContourSegments(model.Geometry as MeshGeometry3D, p, n).ToList();
            
            foreach (var contour in MeshGeometryHelper.CombineSegments(segments, 1e-6).ToList())
            {
                if (contour.Count == 0)
                    continue;
                ContourModel.Children.Add(new TubeVisual3D { Diameter = Diameter, Path = new Point3DCollection(contour), Fill = Brushes.White, ThetaDiv = 4 });
            }
        }
    }
}
