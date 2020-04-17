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

using HelixToolkit.Wpf;

namespace ModelWpfVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Create3DViewPort()
        {
            var viewport = new HelixViewport3D();
            var lights = new DefaultLights();
            //var teaPot = new Teapot();
            
            viewport.Children.Add(lights);
            //viewport.Children.Add(teaPot);
            //MeshElement3D.L
        }

        private void LoadModel()
        {
            //Model3DGroup MyModel = CurrentHelixObjReader.Read(@"\data\dinosaur.obj");
        }
        private MeshElement3D ViewingModel
        {
            get
            {
                return m_model;
            }
            set
            {
                viewport.Children.Remove(m_model);
                viewport.Children.Add(value);
                m_model = value;
            }
            
        }

        private MeshElement3D m_model;
    }
}
