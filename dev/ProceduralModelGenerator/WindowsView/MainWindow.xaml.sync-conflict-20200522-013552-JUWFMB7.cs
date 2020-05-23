using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
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
            // this is just a comment
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
            this.IsEnabled = false;
            RequestGeneration(m_tokenSource.Token);
            this.IsEnabled = true;
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

            // application settings
            vm.SeedString = "FOOBAR";

            // assets 
            var assetsViewModel = new AssetsViewModel();
            m_inputController.ViewModel.AssetsViewModel = assetsViewModel;
            assetsViewModel.DoorsAssetsGroupName = "Doors";
            assetsViewModel.WindowsAssetsGroupName = "Windows";

            // basement settings
            AddPanel(new BasementProperties(vm));
            vm.BuildingMinHeight = 4.0f;
            vm.BuildingMaxHeight = 15.0f;

            // roof settings
            AddPanel(new RoofProperties(vm));
            vm.RoofMinHeight = 0.5f;
            vm.RoofMaxHeight = 1.0f;
            vm.RoofStyle = RoofStyle.Flat;

            // segmenting splits
            AddPanel(new SegmentingProperties(vm));
            vm.MinNumberOfFloors = 1;
            vm.MaxNumberOfFloors = 3;
            vm.MinSelectedWallHorizontalSegments = 1;
            vm.MaxSelectedWallHorizontalSegments = 3;

            // windows
            AddPanel(new WindowsProperties(m_inputController));
            vm.IsVerticalSymmetryPreserved = true;
            vm.IsSingleStyleWindow = true;
            vm.MinWindowsOnSelectedWall = 0;
            vm.MaxWindowsOnSelectedWall = 3;
            vm.AssetsViewModel = assetsViewModel;
            vm.SelectedWindowStyleIdx = 0;

            // doors
            AddPanel(new DoorsProperties(m_inputController));
            vm.SelectedDoorStyleIdx = 1;
            vm.IsDoorOnSelectedWall = true;

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

        public bool RequestGeneration(CancellationToken token)
        {
            var beginTime = DateTime.Now; 
            m_inputController.RequestGenerate();
            var deltaTime = DateTime.Now - beginTime;
            ApplicationStatus = $"Model was generated! {deltaTime.Milliseconds} msec";
            return true;
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

