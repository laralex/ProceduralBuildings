using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace WpfVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //public event EventHandler WireframeEnabled;
        //public event EventHandler WireframeDisabled;
        //public event EventHandler HollowEnabled;
        //public event EventHandler HollowDisabled;
        //public event EventHandler ModelChanged;
        public MainWindow()
        {
            InitializeComponent();
            OnResetCameraClick(this, null);
        }

        private string StatusMessage
        {
            get => c_helixViewport.Title;
            set => c_helixViewport.Title = value;
        }
        private void ListenerLoop()
        {

        }
        private Model3DGroup LoadModelsFile(string modelPath, Dispatcher dispatcher = null)
        {
            var content = new ModelImporter().Load(modelPath, dispatcher);
            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
            foreach(var model in content.Children)
            {
                (model as GeometryModel3D).Material = greenMaterial;
            }
            return content;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            StatusMessage = "Closing...";
            this.Close();
        }

        private void OnResetCameraClick(object sender, RoutedEventArgs e)
        {
            c_helixViewport.Camera.Reset();
            c_helixViewport.ZoomExtents();
        }

        private void OnOpenModelClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter="3D Model File|*.obj;*.stl" };
            if (dialog.ShowDialog() == true && dialog.FileName != null)
            {
                StatusMessage = "Loading model file";
                m_model.ShownModel = LoadModelsFile(dialog.FileName);
                OnResetCameraClick(this, null);
                StatusMessage = "Model file loaded";
            }
        }

       

        private void OnHollowChecked(object sender, RoutedEventArgs e)
        {

        }
        private void OnHollowUnchecked(object sender, RoutedEventArgs e) //=> HollowDisabled(sender, e);
        {

        }

        private void OnWireframeChecked(object sender, RoutedEventArgs e) //=> WireframeEnabled(sender, e);
        {
            
        }
        private void OnWireframeUnchecked(object sender, RoutedEventArgs e) // => WireframeDisabled(sender, e);
        {
            //IsWireframeEnabled = false;
        }

        MainWindowModel m_model;
    }

    public class MainWindowModel
    {
        public Model3D ShownModel { get; set; }
    }
}

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