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
        private int m_minNumberOfFloors;
        public int MinNumberOfFloors
        {
            get => m_minNumberOfFloors;
            set
            {
                m_minNumberOfFloors = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinNumberOfFloors"));
            }
        }

        private int m_maxNumberOfFloors;
        public int MaxNumberOfFloors
        {
            get => m_maxNumberOfFloors;
            set
            {
                m_maxNumberOfFloors = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxNumberOfFloors"));
            }
        } 

        private int m_minVerticalSplitsNumber;
        public int MinSelectedWallHorizontalSegments
        {
            get => m_minVerticalSplitsNumber;
            set
            {
                m_minVerticalSplitsNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinSelectedWallHorizontalSegments"));
            }
        }

        private int m_maxHorizontalSegmentsNumber;
        public int MaxSelectedWallHorizontalSegments
        {
            get => m_maxHorizontalSegmentsNumber;
            set
            {
                m_maxHorizontalSegmentsNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxSelectedWallHorizontalSegments"));
            }
        }
        public SegmentingProperties()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
