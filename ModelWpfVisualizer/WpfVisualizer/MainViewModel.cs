using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfVisualizer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ModelVisual3D m_model;
        public ModelVisual3D ShownModel {
            get => m_model;
            set
            {
                m_model = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShownModel"));
            }
        }

        private Model3DGroup m_light;
        public Model3DGroup Light
        {
            get => m_light;
            set
            {
                m_light = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Light"));
            }
        }

        private string m_status;
        public string ApplicationStatus
        {
            get => m_status;
            set
            {
                m_status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplicationStatus"));
            }
        }

        private Brush m_brush;
        public Brush Brush
        {
            get => m_brush;
            set
            {
                m_brush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Brush"));
            }
        }

        private ModelVisual3D m_hiddenModel;
        public ModelVisual3D HiddenModel
        {
            get => m_hiddenModel;
            set
            {
                m_hiddenModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HiddenModel"));
            }
        }
    }
}
