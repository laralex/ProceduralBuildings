using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.IO;
using System.ServiceModel;
using VisualizerLibrary;
using WcfVisualizerLibrary;
using System.Threading;

namespace WpfVisualizer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class MainWindow : Window, IVisualizerService, IVisualizer
    {
        public event EventHandler ShutdownRequested;
        public MainWindow()
        {
            InitializeComponent();
            m_replacementMaterial = MaterialHelper.CreateMaterial(Brushes.LightBlue, 0.0, 40);
            m_backReplacementMaterial = MaterialHelper.CreateMaterial(Brushes.Green, 0.0, 40);
            m_viewModel = new MainViewModel
            {
                ShownVisual3d = new ModelVisual3D()
            };
            this.DataContext = m_viewModel;

            m_visualizerPreparationStage = ServicePreparationStage.NotOpened;
            if (TryOpenVisualizerService())
            {
                TryReconnect();
            }

            //{
            //if (args[1] == "--external") System.Windows.Application.Current.Shutdown();
            //}
            
            OnResetCameraClick(this, null);

            ShutdownRequested += (s, e) =>
            {
                Thread.Sleep(200);
                OnWindowClosing(s, null);
                this.Close();
            };
        }

        private bool TryOpenVisualizerService()
        {
            try
            {
                m_visualizerService = new ServiceHost(this);
                const int MAX_DATA_BYTES_NUMBER = 104857600; // 100 Mb
                var httpBinding = new BasicHttpBinding
                {
                    TransferMode = TransferMode.Streamed,
                    MaxReceivedMessageSize = MAX_DATA_BYTES_NUMBER,
                };

                m_visualizerService.AddServiceEndpoint(typeof(IVisualizerService), httpBinding, VISUALIZATOR_SERVICE_URI);
                m_visualizerService.Open();
                //m_viewModel.ApplicationStatus = "HTTP visualizer service opened successfully";
            }
            catch (Exception e)
            {
                //m_viewModel.ApplicationStatus = $"HTTP service FAILED to start";
                m_viewModel.ApplicationStatus = "Couldn't open visualizer service (perhaps another instance had done it)";
                return false;
                //MessageBox.Show(e.Message);
            }
            m_visualizerPreparationStage = ServicePreparationStage.Opened;
            return true;
        }
        #region Service implementation
        public void VisualizeModel(Stream model, ModelMetaBase modelMeta, Stream materialLibrary, Stream[] materialFiles)
        {
            PrepareForModel(modelMeta);
            AcceptMaterialLib(materialLibrary);
            for (int i = 0; i < materialFiles.Length; ++i)
            {
                PrepareForMaterialFile(modelMeta.MaterialFileIds[i]);
                AcceptMaterialFile(materialFiles[i]);
            }
            AcceptModel(model);
        }

        public void Shutdown()
        {
            this.Close();
            //ShutdownRequested?.Invoke(this, null);
        }

        public string GetDescription()
        {
            return "C# WPF based simple model visualizer via Helix Toolkit Library capabilities";
        }

        public void PrepareForModel(ModelMetaBase modelMetadata)
        {
            m_currentModelMeta = modelMetadata;
        }

        public void AcceptMaterialLib(Stream model)
        {
            // no material
        }

        public void PrepareForMaterialFile(string materialFileId)
        {
            //m_nextMaterialFile = materialFileId;
        }

        public void AcceptMaterialFile(Stream materialFile)
        {
            // no material
        }

        public void AcceptModel(Stream model)
        {
            try
            {
                PrepareModel(model, m_currentModelMeta.ModelType);
                m_viewModel.ApplicationStatus = "Model from generator loaded successfully";
            }
            catch (Exception e)
            {
                m_viewModel.ApplicationStatus = $"Model from generator FAILED to load";
                //MessageBox.Show(e.Message);
            }
        }

        public void Visualize()
        {
            m_viewModel.ShownVisual3d.Content = m_currentModel;
            m_viewModel.ContourVisual3D = null;
            OnResetCameraClick(this, null);
        }

        #endregion

        #region Model loading from stream
        private Model3DGroup LoadModel(Stream model, ModelDataType type, 
            string textureFilePath = null, bool freeze = false)
        {
            ModelReader reader = null;
            switch (type)
            {
                case ModelDataType.OBJ: reader = new ObjReader(); break;
                case ModelDataType.STL: reader = new StLReader(); break;
                case ModelDataType.ThreeDS: reader = new StudioReader(); break;
            }
            if (reader == null) throw new NotSupportedException("Given model type is not supported");
            reader.TexturePath = textureFilePath != null ? textureFilePath : "";
            reader.Freeze = freeze;
            //using (model)
            return reader.Read(model);
        }
        private Model3DGroup LoadModel(string filePath, ModelDataType type,
            string textureFilePath = null, bool freeze = false)
        {
            return LoadModel(File.OpenRead(filePath), type, textureFilePath, freeze);
        }

        private async Task<Tuple<Model3DGroup,Dispatcher>> LoadModelAsync(Stream model, ModelDataType type,
            string textureFilePath = null, bool freeze = false)
        {
            return await Task.Run(() => Tuple.Create(LoadModel(model, type, textureFilePath, freeze), this.Dispatcher)).ConfigureAwait(true);
        }
        private async Task<Tuple<Model3DGroup, Dispatcher>> LoadModelAsync(string filePath, ModelDataType type,
            string textureFilePath = null, bool freeze = false)
        {
            return await LoadModelAsync(File.OpenRead(filePath), type, textureFilePath, freeze);
        }

        #endregion

        #region UI events

        private void TryReconnect()
        {
            try
            {
                m_visualizationContorllerClient = WcfServiceUtility.SpawnClient<IVisualizationControllerService>(VISUALIZATION_CONTROLLER_SERVICE_URI);
            }
            catch
            {
                m_viewModel.ApplicationStatus = "Couldn't make HTTP client, try later";
                return;
            }
            m_visualizerPreparationStage = ServicePreparationStage.CreatedRegistrationClient;
            try
            {
                ((IClientChannel)m_visualizationContorllerClient).Open();
            }
            catch
            {
                m_viewModel.ApplicationStatus = "Couldn't open HTTP client, try later";
                return;
            }
            m_visualizerPreparationStage = ServicePreparationStage.OpenedRegistrationClient;
            bool registered;
            try
            {
                registered =
                    m_visualizationContorllerClient.RegisterVisualizer(VISUALIZATOR_SERVICE_URI);
            }
            catch
            {
                m_viewModel.ApplicationStatus = "Couldn't register HTTP client, try later";
                return;
            }
            if (!registered)
            {
                m_viewModel.ApplicationStatus = "HTTP visualizer service is ready, but registered by another instance";
            }
            else
            {
                m_viewModel.ApplicationStatus = "HTTP visualizer service is ready and registered";
            }
            m_visualizerPreparationStage = ServicePreparationStage.Ready;
        }
        private void OnReconnectClick(object sender, RoutedEventArgs e)
        {
            TryOpenVisualizerService();
            TryReconnect();
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
                ModelDataType? fileType = null;
                switch (Path.GetExtension(dialog.FileName))
                {
                    case ".obj": fileType = ModelDataType.OBJ; break;
                    case ".stl": fileType = ModelDataType.STL; break;
                    case ".3ds": fileType = ModelDataType.ThreeDS; break;
                }
                if (fileType == null)
                {
                    throw new NotSupportedException("Given file type is not supported");
                }

                try
                {
                    PrepareModel(File.OpenRead(dialog.FileName), (ModelDataType)fileType, null, false);
                    m_viewModel.ApplicationStatus = "Model file loaded successfully: " +
                        System.IO.Path.GetFileName(dialog.FileName);
                }
                catch
                {
                    m_viewModel.ApplicationStatus = "Model file FAILED to load";
                }

                Visualize();
            }
        }

        private void OnContourEnabled(object sender, RoutedEventArgs e)
        {
            if (m_viewModel.ContourVisual3D == null) {
                m_viewModel.ContourVisual3D = 
                    ContourUtility.AddContours(s_shownModelVisual3D, 10, 10, 10, 0.07);
            }
            c_helixViewport.Children.Add(m_viewModel.ContourVisual3D);
            c_helixViewport.Children.Remove(s_shownModelVisual3D);
        }

        private void OnContourDisabled(object sender, RoutedEventArgs e)
        {
            c_helixViewport.Children.Remove(m_viewModel.ContourVisual3D);
            c_helixViewport.Children.Add(s_shownModelVisual3D);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_viewModel.ApplicationStatus = "Closing...";
            var client = (IClientChannel)m_visualizationContorllerClient;
            if (client.State == CommunicationState.Opened)
            {
                client.Close();
            }
            //if (m_visualizerService.State == CommunicationState.Opened)
            {
              //  m_visualizerService.Close();
            }
            client.Dispose();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            const int AUTO_CONNECT_TIMEOUT_MSEC = 60_000; 
            const int AUTO_CONNECT_RETRY_PERIOD_MSEC = 500;
            const int AUTO_CONNECT_RETRIES = AUTO_CONNECT_TIMEOUT_MSEC / AUTO_CONNECT_RETRY_PERIOD_MSEC;
            int autoConnectRetriesLeft = AUTO_CONNECT_RETRIES;
            if (m_visualizerPreparationStage != ServicePreparationStage.Opened) return;
            while (autoConnectRetriesLeft > 0 && m_visualizerPreparationStage != ServicePreparationStage.Ready)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(AUTO_CONNECT_RETRY_PERIOD_MSEC);
                    OnReconnectClick(sender, e);
                });
                autoConnectRetriesLeft--;
            }
        }

        #endregion

        private async void PrepareModel(Stream model, ModelDataType type,
            string texturePath = null, bool async = false, bool freezeModel = false)
        {
            // ui
            m_viewModel.ApplicationStatus = "Loading model file";
            c_contourToggle.IsChecked = false;

            // loading 
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

            // setting materials
            if (!freezeModel)
            {
                modelDispatcher.Invoke(() =>
                {
                    foreach (var m in modelContent.Children)
                    {
                        //(m as GeometryModel3D).BackMaterial = null;
                        (m as GeometryModel3D).Material = m_replacementMaterial; //m_replacementMaterial;
                        (m as GeometryModel3D).BackMaterial = m_replacementMaterial;
                    }
                });
            }
            m_currentModel = modelContent;
        }

        private MainViewModel m_viewModel;
        private Model3DGroup m_currentModel;
        private ModelMetaBase m_currentModelMeta;
        private Material m_replacementMaterial;
        private Material m_backReplacementMaterial;
        private ServiceHost m_visualizerService;
        private IVisualizationControllerService m_visualizationContorllerClient;
        private ServicePreparationStage m_visualizerPreparationStage;
        private const string VISUALIZATOR_SERVICE_URI = "http://localhost:64046/wpfVisualizerService";
        private const string VISUALIZATION_CONTROLLER_SERVICE_URI = "http://localhost:64046/visualizationControllerService";
    }

    enum ServicePreparationStage
    {
        NotOpened,
        Opened,
        CreatedRegistrationClient,
        OpenedRegistrationClient,
        Registered,
        Ready
    }
}