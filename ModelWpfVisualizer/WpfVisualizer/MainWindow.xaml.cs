using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace WpfVisualizer
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            m_modelMaterial = MaterialHelper.CreateMaterial(Brushes.LightBlue, 0.0, 40);

            m_viewModel = new MainViewModel
            {
                ShownVisual3d = new ModelVisual3D()
            };
            this.DataContext = m_viewModel;

            OnResetCameraClick(this, null);
        }

        private Model3DGroup LoadModelsFile(string modelPath, Dispatcher dispatcher = null)
        {
            var content = new ModelImporter().Load(modelPath, dispatcher);
            
            foreach(var model in content.Children)
            {
                (model as GeometryModel3D).Material = m_modelMaterial;
            }
            return content;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            m_viewModel.ApplicationStatus = "Closing...";
            this.Close();
        }

        private void OnResetCameraClick(object sender, RoutedEventArgs e)
        {
            c_helixViewport.Camera.Reset();
            c_helixViewport.ZoomExtents();
        }

        private void OnOpenModelClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter="3D Model File|*.obj;*.stl;*.3ds;*.collada" };
            if (dialog.ShowDialog() == true && dialog.FileName != null)
            {
                m_viewModel.ApplicationStatus = "Loading model file";
                m_viewModel.ShownVisual3d.Content = LoadModelsFile(dialog.FileName);
                c_contourToggle.IsChecked = false;
                m_viewModel.ContourVisual3D = null;
                m_viewModel.ApplicationStatus = "Model file loaded: " + 
                    System.IO.Path.GetFileName(dialog.FileName);
                OnResetCameraClick(this, null);
            }
        }

        private void OnContourEnabled(object sender, RoutedEventArgs e)
        {
            if (m_viewModel.ContourVisual3D == null) {
                m_viewModel.ContourVisual3D = 
                    ContourUtility.AddContours(s_shownModelVisual3D, 10, 10, 10, 0.5);
            }
            c_helixViewport.Children.Add(m_viewModel.ContourVisual3D);
            c_helixViewport.Children.Remove(s_shownModelVisual3D);
        }

        private void OnContourDisabled(object sender, RoutedEventArgs e)
        {
            c_helixViewport.Children.Remove(m_viewModel.ContourVisual3D);
            c_helixViewport.Children.Add(s_shownModelVisual3D);
        }

        private MainViewModel m_viewModel;
        private Material m_modelMaterial;
    }
}

//public event EventHandler WireframeEnabled;
//public event EventHandler WireframeDisabled;
//public event EventHandler HollowEnabled;
//public event EventHandler HollowDisabled;
//public event EventHandler ModelChanged;

//private void OnHollowChecked(object sender, RoutedEventArgs e)
//{

//}
//private void OnHollowUnchecked(object sender, RoutedEventArgs e) //=> HollowDisabled(sender, e);
//{

//}

//private void OnWireframeChecked(object sender, RoutedEventArgs e) //=> WireframeEnabled(sender, e);
//{

//}
//private void OnWireframeUnchecked(object sender, RoutedEventArgs e) // => WireframeDisabled(sender, e);
//{
//    //IsWireframeEnabled = false;
//}
//var wireframes = new Model3DGroup();
//var mb = new MeshBuilder();
//foreach(var model3d in content.Children)
//{
//    if (model3d is GeometryModel3D)
//    {
//        var w = ((model3d as GeometryModel3D).Geometry as MeshGeometry3D).ToWireframe(0.4);
//        var g = new GeometryModel3D { Geometry = w, Material = (model3d as GeometryModel3D).Material };
//        wireframes.Children.Add(g);
//    }
//} 

/*private bool IsWireframeEnabled
{
get => m_isWireframeEnabled;
set
{
    m_isWireframeEnabled = value;
    if (m_isWireframeEnabled)
    {
        if (m_cachedWireframe == null)
        {
            m_cachedWireframe = GenerateWireframe(ShownModel);
        }
        c_helixViewport.Children.Add(m_cachedWireframe);
    }
    else
    {
        c_helixViewport.Children.Remove(m_cachedWireframe);
    }
}
} */
//var allWireframes = new ModelVisual3D
//{
//    Content = wireframes,
//};