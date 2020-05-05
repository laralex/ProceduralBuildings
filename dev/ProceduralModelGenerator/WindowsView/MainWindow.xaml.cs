using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GeneratorController;
using GeneratorController.viewmodel;
using WindowsGeneratorView;

namespace WindowsView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static string rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        public string GenerateIconPath => System.IO.Path.Combine(rootDir, @"data/ui/generate-icon.png");
        public string ExportIconPath => System.IO.Path.Combine(rootDir, @"data/ui/export-icon.png");
        public string HelpIconPath => System.IO.Path.Combine(rootDir, @"data/ui/info-icon.png");
        public string VisualizeIconPath => System.IO.Path.Combine(rootDir, @"data/ui/visualize-icon.png");


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



        public MainWindow()
        {
            m_inputController = new InputController();
            this.DataContext = m_inputController.ViewModel;

            InitializeComponent();
        }

        public string Description => "WPF Windows generation parameters GUI";

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            m_inputController.RequestGenerate();
            ApplicationStatus = "Model was generated!";
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
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
                    //System.Windows.MessageBox.Show("Nothing to export or export IO error");
                }
                else
                {
                    ApplicationStatus = "Export completed!";
                }
            }

        }

        private void OnVisualizeClick(object sender, RoutedEventArgs e)
        {
            var visualizationResult = m_inputController.RequestVisualize();
            ApplicationStatus = visualizationResult ? 
                "Model was sent to client visualizers!" :
                "An error during visualization";
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
            vm.SelectedWindowStyleIdx = 3;

            // doors
            AddPanel(new DoorsProperties(m_inputController));
            vm.SelectedDoorStyleIdx = 1;
            vm.IsDoorOnSelectedWall = true;

            if (!m_inputController.StartService())
            {
                MessageBox.Show("Another instance of the program is already started! Cannot start a new one.");
                System.Windows.Application.Current.Shutdown();
            }
            ApplicationStatus = "Loaded and ready";
        }

        private void OnPreviewSeedInput(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextBoxCode(sender, e);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_inputController.Dispose();
        }

        private void AddPanel(UserControl panel)
        {
            c_panels.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 10) });
            c_panels.Children.Add(panel);
        }

        private InputController m_inputController;

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
    }
}

//private UserControl m_propertyPanel;
//public UserControl PropertiesPanel
//{
//    get => m_propertyPanel;
//    set
//    {
//        m_propertyPanel = value;
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PropertiesPanel"));
//    }
//}
