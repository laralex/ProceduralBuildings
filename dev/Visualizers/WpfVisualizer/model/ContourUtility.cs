using HelixToolkit.Wpf;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfVisualizer
{
    public class ContourUtility
    {
        public static ModelVisual3D AddContours(Visual3D model1, int d1, int d2, int d3, double diameter = 0.5)
        {
            var ContourModel = new ModelVisual3D();
            Plane3D ContourPlane = null;
            Action<GeometryModel3D, Transform3D> AddContours = (model, transform) =>
            {
                var p = ContourPlane.Position;
                var n = ContourPlane.Normal;
                var segments = MeshGeometryHelper
                    .GetContourSegments(model.Geometry as MeshGeometry3D, p, n).ToList();
                var contours = MeshGeometryHelper.CombineSegments(segments, 1e-6).ToList();
                foreach (var contour in contours)
                {
                    if (contour.Count == 0)
                        continue;
                    ContourModel.Children.Add(new TubeVisual3D {
                        Diameter = diameter,
                        Path = new Point3DCollection(contour),
                        Fill = Brushes.White, ThetaDiv = 4
                    });
                }
            };
            var bounds = Visual3DHelper.FindBounds(model1, Transform3D.Identity);
            for (int i = 1; i < d3; i++)
            {
                ContourPlane = new Plane3D(
                    new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i / d3), 
                    new Vector3D(0, 0, 1));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            for (int i = 1; i < d2; i++)
            {
                ContourPlane = new Plane3D(
                    new Point3D(0, bounds.Location.Y + bounds.Size.Y * i / d2, 0), 
                    new Vector3D(0, 1, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            for (int i = 1; i < d1; i++)
            {
                ContourPlane = new Plane3D(
                    new Point3D(bounds.Location.X + bounds.Size.X * i / d1, 0, 0), 
                    new Vector3D(1, 0, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, AddContours);
            }
            return ContourModel;
        }
    }
}
