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
            var generationController = new GenerationController();
            var generationData = new BuildingsViewModel { Seed = 42 };
            generationController.Generate(generationData);
            InitializeComponent();
        }

        public string Description => "WPF Windows generation parameters GUI";
    }
}
