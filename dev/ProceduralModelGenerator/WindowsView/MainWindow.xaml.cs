using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GeneratorController;

namespace WindowsView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IParametersView
    {
        public MainWindow()
        {
            var generationData = new BuildingsViewModel();
            var generationController = new BuildingsGenerationController();
            this.DataContext = generationData;
            generationController.Generate(generationData);
            InitializeComponent();
        }

        public string Description => "WPF Windows generation parameters GUI";

        private IGenerationController m_generationControler;
        private VisualizationController m_visualizationController;
    }
}
