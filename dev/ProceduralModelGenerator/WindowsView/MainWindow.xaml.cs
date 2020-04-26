using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WindowsGeneratorView;

namespace WindowsView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IParametersView
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindow()
        {
            m_generationControler = new BuildingsGenerationController();
            this.DataContext = m_viewModel = new BuildingsViewModel();
            m_viewModel.Grammar = new ViewModelGrammar();
            var m1 = new GrammarNode("MY1");
            m_viewModel.Grammar.Children.Add(m1);
            m1.Children.Add(new GrammarNode("SUB1"));
            m1.Children.Add(new GrammarNode("SUB2"));
            var m2 = new GrammarNode("MY2");
            m2.Children.Add(new GrammarNode("LOL2"));
            m2.Children.Add(new GrammarNode("LOL1"));
            //generationController.Generate(generationData);
            InitializeComponent();
        }

        public string Description => "WPF Windows generation parameters GUI";

        private IGenerationController m_generationControler;
        private BuildingsViewModel m_viewModel;
        private VisualizationController m_visualizationController;

        

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            m_generationControler.Generate(this.DataContext as BuildingsViewModel);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_viewModel.PropertiesPanel = new BasementProperties();
        }

        private void OnPreviewSeedInput(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextBoxCode(sender, e);
        }
    }
}
