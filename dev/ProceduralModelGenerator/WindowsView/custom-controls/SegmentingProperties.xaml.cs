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
    /// Interaction logic for SectioningProperties.xaml
    /// </summary>
    public partial class SegmentingProperties : UserControl, IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private float m_minNumberOfFloors;
        public float MinNumberOfFloors
        {
            get => m_minNumberOfFloors;
            set
            {
                m_minNumberOfFloors = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinNumberOfFloors"));
            }
        }

        private float m_maxNumberOfFloors;
        public float MaxNumberOfFloors
        {
            get => m_minNumberOfFloors;
            set
            {
                m_minNumberOfFloors = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxNumberOfFloors"));
            }
        } 

        private float m_minVerticalSplitsNumber;
        public float MinSelectedWallHorizontalSegments
        {
            get => m_minVerticalSplitsNumber;
            set
            {
                m_minVerticalSplitsNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinSelectedWallHorizontalSegments"));
            }
        }

        private float m_maxHorizontalSegmentsNumber;
        public float MaxSelectedWallHorizontalSegments
        {
            get => m_minVerticalSplitsNumber;
            set
            {
                m_minVerticalSplitsNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxSelectedWallHorizontalSegments"));
            }
        }
        public SegmentingProperties()
        {
            InitializeComponent();
        }
    }
}
