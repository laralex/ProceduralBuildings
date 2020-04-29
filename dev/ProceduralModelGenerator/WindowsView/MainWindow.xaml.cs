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
using WindowsGeneratorView;

namespace WindowsView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IParametersView
    {
        public MainWindow()
        {
            m_inputController = new InputController();
            this.DataContext = m_inputController.ViewModel;

            //var m1 = new GrammarNode("MY1");
            //m_viewModel.Grammar.Children.Add(m1);
            //m1.Children.Add(new GrammarNode("SUB1"));
            //m1.Children.Add(new GrammarNode("SUB2"));
            //var m2 = new GrammarNode("MY2");
            //m2.Children.Add(new GrammarNode("LOL2"));
            //m2.Children.Add(new GrammarNode("LOL1"));
            //generationController.Generate(generationData);
            InitializeComponent();
        }

        public string Description => "WPF Windows generation parameters GUI";

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            m_inputController.RequestGenerate();
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
                    System.Windows.MessageBox.Show("Nothing to export or export IO error");
                }
            }

        }

        private void OnVisualizeClick(object sender, RoutedEventArgs e)
        {
            var visualizationResult = m_inputController.RequestVisualize();
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var basementViewModel = new BasementPropertiesViewModel();
            m_inputController.ViewModel.PropertiesPanel = new BasementProperties(basementViewModel);
            m_inputController.ViewModel.BasementOptions = basementViewModel;
            if (!m_inputController.StartService())
            {
                MessageBox.Show("Another instance of the program is already started! Cannot start a new one.");
                System.Windows.Application.Current.Shutdown();
            }
            // fixme
            var basementProps = (BasementProperties)(m_inputController.ViewModel.PropertiesPanel);
        }

        private void OnPreviewSeedInput(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextBoxCode(sender, e);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_inputController.Dispose();
        }

        private InputController m_inputController;
    }
}
