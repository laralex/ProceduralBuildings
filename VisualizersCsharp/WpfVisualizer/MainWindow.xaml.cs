using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using VisualizerLibrary;
using System.IO;

namespace WpfVisualizer
{
    public partial class MainWindow : Window, IVisualizer
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

        public string Description { get => "C# WPF based simple model visualizer via Helix Toolkit Library capabilities"; }
        public void VisualizeModel(Stream model, ModelBinaryType type = ModelBinaryType.OBJ)
        {
            LoadAndVisualizeModel(model, type);
            m_viewModel.ApplicationStatus = "Model from generator loaded.";
        }
        public void Shutdown()
        {
            OnCloseClick(this, null);
        }

        private async void LoadAndVisualizeModel(Stream model, ModelBinaryType type, 
            string texturePath = null, bool async = false, bool freezeModel = false)
        {
            m_viewModel.ApplicationStatus = "Loading model file";
            c_contourToggle.IsChecked = false;

            Dispatcher modelDispatcher;
            Model3DGroup modelContent;
            if (async)
            {
                var modelAndDispatcher = await LoadModelAsync(model, type, texturePath, freezeModel);
                modelContent = modelAndDispatcher.Item1;
                modelDispatcher = modelAndDispatcher.Item2;
            }
            else
            {
                modelContent = LoadModel(model, type, texturePath, freezeModel);
                modelDispatcher = this.Dispatcher;
            }
                
            
            if (!freezeModel)
            {
                modelDispatcher.Invoke(() =>
                {
                    foreach (var m in modelContent.Children)
                    {
                        (m as GeometryModel3D).Material = m_modelMaterial;
                    }
                });
            }
            m_viewModel.ShownVisual3d.Content = modelContent;
            m_viewModel.ContourVisual3D = null;

            OnResetCameraClick(this, null);
        }

        private Model3DGroup LoadModel(Stream model, ModelBinaryType type, 
            string textureFilePath = null, bool freeze = false)
        {
            ModelReader reader = null;
            switch (type)
            {
                case ModelBinaryType.OBJ: reader = new ObjReader(); break;
                case ModelBinaryType.STL: reader = new StLReader(); break;
                case ModelBinaryType.ThreeDS: reader = new StudioReader(); break;
            }
            if (reader == null) throw new NotSupportedException("Given model type is not supported");
            reader.TexturePath = textureFilePath != null ? textureFilePath : "";
            reader.Freeze = freeze;
            using (model)
            {
                return reader.Read(model);
            }
        }
        private Model3DGroup LoadModel(string filePath, ModelBinaryType type,
            string textureFilePath = null, bool freeze = false)
        {
            return LoadModel(File.OpenRead(filePath), type, textureFilePath, freeze);
        }

        private async Task<Tuple<Model3DGroup,Dispatcher>> LoadModelAsync(Stream model, ModelBinaryType type,
            string textureFilePath = null, bool freeze = false)
        {
            return await Task.Run(() => Tuple.Create(LoadModel(model, type, textureFilePath, freeze), this.Dispatcher)).ConfigureAwait(true);
        }
        private async Task<Tuple<Model3DGroup, Dispatcher>> LoadModelAsync(string filePath, ModelBinaryType type,
            string textureFilePath = null, bool freeze = false)
        {
            return await LoadModelAsync(File.OpenRead(filePath), type, textureFilePath, freeze);
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
            var dialog = new OpenFileDialog { Filter="3D Model File|*.obj;*.stl;*.3ds" };
            if (dialog.ShowDialog() == true && dialog.FileName != null)
            {
                ModelBinaryType? fileType = null;
                switch (Path.GetExtension(dialog.FileName))
                {
                    case ".obj": fileType = ModelBinaryType.OBJ; break;
                    case ".stl": fileType = ModelBinaryType.STL; break;
                    case ".3ds": fileType = ModelBinaryType.ThreeDS; break;
                }
                if (fileType == null)
                {
                    throw new NotSupportedException("Given file type is not supported");
                }
                LoadAndVisualizeModel(File.OpenRead(dialog.FileName), (ModelBinaryType)fileType, null, false);
                m_viewModel.ApplicationStatus = "Model file loaded: " +
                    System.IO.Path.GetFileName(dialog.FileName);
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