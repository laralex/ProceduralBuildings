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

        private ModelVisual3D m_shownModel;
        public ModelVisual3D ShownVisual3d {
            get => m_shownModel;
            set
            {
                m_shownModel = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs("ShownVisual3d"));
            }
        }


        private string m_status;
        public string ApplicationStatus
        {
            get => m_status;
            set
            {
                m_status = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs("ApplicationStatus"));
            }
        }

        private ModelVisual3D m_contour;
        public ModelVisual3D ContourVisual3D
        {
            get => m_contour;
            set
            {
                m_contour = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs("ContourVisual3D"));
            }
        }
    }
}
