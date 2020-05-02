using GeneratorController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsGeneratorView
{
    /// <summary>
    /// Interaction logic for RoofProperties.xaml
    /// </summary>
    public partial class RoofProperties : UserControl, IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private float m_roofMinHeight;
        public float RoofMinHeight
        {
            get => m_roofMinHeight;
            set
            {
                m_roofMinHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoofMinHeight"));
            }
        }

        private float m_roofMaxHeight;
        public float RoofMaxHeight
        {
            get => m_roofMaxHeight;
            set
            {
                m_roofMaxHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoofMaxHeight"));
            }
        }
        public RoofProperties()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
