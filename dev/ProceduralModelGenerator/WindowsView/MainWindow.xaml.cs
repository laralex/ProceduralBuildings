using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GeneratorController;
using WindowsGeneratorView;

namespace WindowsView
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IProceduralGeneratorUi
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private InputController m_inputController;
        private CancellationTokenSource m_tokenSource;
        private static string m_rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        public string GenerateIconPath => System.IO.Path.Combine(m_rootDir, @"data/ui/generate-icon.png");
        public string ExportIconPath => System.IO.Path.Combine(m_rootDir, @"data/ui/export-icon.png");
        public string HelpIconPath => System.IO.Path.Combine(m_rootDir, @"data/ui/info-icon.png");
        public string VisualizeIconPath => System.IO.Path.Combine(m_rootDir, @"data/ui/visualize-icon.png");
        public string Description => "WPF Windows generation parameters GUI";
        public IViewModel ViewModel { get; set; }


        public MainWindow()
        {
            m_inputController = new InputController();
            m_tokenSource = new CancellationTokenSource();
            this.DataContext = m_inputController.ViewModel;
            InitializeComponent();
        }


        private string m_applicationStatus;
        public string ApplicationStatus
        {
            get => m_applicationStatus;
            set
            {
                m_applicationStatus = "Status: " + value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplicationStatus"));
            }
        }

        private void OnGenerateButtonFocused(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            //this.IsEnabled = false;
            RequestGeneration(m_tokenSource.Token);
            //this.IsEnabled = true;
        }

        private void OnGenerateNewSeedClick(object sender, RoutedEventArgs e)
        {
            OnNewSeedClick(sender, e);
            Keyboard.ClearFocus();
            OnGenerateClick(sender, e);
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            this.IsEnabled = false;
            RequestExport(m_tokenSource.Token);
            this.IsEnabled = true;
        }

        private void OnVisualizeClick(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            this.IsEnabled = false;
            RequestGenerationVisualization(m_tokenSource.Token);
            this.IsEnabled = true;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = m_inputController.ViewModel;
            vm.SelectedSideMeters = 15.0f;

            // application settings
            vm.SeedString = "FOOBAR";

            // assets 
            var assetsViewModel = new AssetsViewModel();
            vm.AssetsViewModel = assetsViewModel;
            assetsViewModel.DoorsAssetsGroupName = "Doors";
            assetsViewModel.WindowsAssetsGroupName = "Windows";
            assetsViewModel.AssetTrianglesLimit = 500;

            // basement settings
            AddPanel(new BasementProperties(vm));
            vm.BuildingMinHeight = 25.0f;
            vm.BuildingMaxHeight = 30.0f;

            // roof settings
            AddPanel(new RoofProperties(vm));
            vm.RoofMinHeight = 1.0f;
            vm.RoofMaxHeight = 2.0f;
            vm.RoofStyle = RoofStyle.Flat;
            vm.RoofEdgeMinOffsetPct = 5;
            vm.RoofEdgeMaxOffsetPct = 25;

            // segmenting splits
            AddPanel(new SegmentingProperties(vm));
            vm.MinNumberOfFloors = 6;
            vm.MaxNumberOfFloors = 8;
            vm.MinSelectedWallHorizontalSegments = 4;
            vm.MaxSelectedWallHorizontalSegments = 7;

            // windows
            AddPanel(new WindowsProperties(m_inputController));
            vm.IsVerticalSymmetryPreserved = true;
            vm.IsSingleStyleWindow = true;
            vm.MinWindowsOnSelectedWall = 3;
            vm.MaxWindowsOnSelectedWall = 6;
            vm.AssetsViewModel = assetsViewModel;
            vm.SelectedWindowStyleIdx = 0;

            // doors
            AddPanel(new DoorsProperties(m_inputController));
            vm.SelectedDoorStyleIdx = 1;
            vm.IsDoorOnSelectedWall = true;

            // performance
            AddPanel(new PerformanceProperties(assetsViewModel));


            StartupRegistrationService();
            ApplicationStatus = "Loaded and ready";
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_inputController.Dispose();
        }

        private void OnPreviewSeedInput(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextBoxCode(sender, e);
        }

        private void OnNewSeedClick(object sender, RoutedEventArgs e)
        {
            const int seedSize = 6;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var stringChars = new char[seedSize];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            m_inputController.ViewModel.SeedString = new String(stringChars);
        }

        private void StartupRegistrationService()
        {
            if (!m_inputController.StartService())
            {
                MessageBox.Show("Another instance of the program is already started! Cannot start a new one.");
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void AddPanel(UserControl panel)
        {
            c_panels.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 10) });
            c_panels.Children.Add(panel);
        }

        public bool RequestAssetVisualization(CancellationToken token)
        {
            return false;
        }

        public async void RequestGeneration(CancellationToken token)
        {
            ApplicationStatus = $"Generating ...";
            var beginTime = DateTime.Now;
            var generationEndTime = await m_inputController.RequestGenerateAsync(this.Dispatcher);
            var visualizationDeltaTime = DateTime.Now - generationEndTime;
            var generationDeltaTime = generationEndTime - beginTime;
            ApplicationStatus = $"Finished generating in {(int)generationDeltaTime.TotalMilliseconds} ms, sent to clients in {(int)visualizationDeltaTime.TotalMilliseconds} ms";
        }

        public bool RequestExport(CancellationToken token)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "OBJ model|*.obj|STL model|*.stl",
                AddExtension = true,
                Title = "Export model to file",
                RestoreDirectory = true,
                CheckPathExists = true,
                FileName = "generated_model"
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var exportResult = m_inputController.RequestExport(dialog.FileName);
                if (!exportResult)
                {
                    ApplicationStatus = "Nothing to export or export IO error";
                }
                else
                {
                    ApplicationStatus = "Export completed!";
                    return true;
                }
            }
            return false;
        }

        public bool RequestGenerationVisualization(CancellationToken token)
        {
            var visualizationResult = m_inputController.RequestVisualize();
            ApplicationStatus = visualizationResult ?
                "Model was sent to client visualizers!" :
                "An error during visualization";
            return visualizationResult;
        }
    }
}

