using GeneratorController;
using GeneratorController.viewmodel;
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
    /// Interaction logic for WindowsProperties.xaml
    /// </summary>
    public partial class WindowsProperties : UserControl, IViewModel
    {

        private InputController m_inputController;
        public event PropertyChangedEventHandler PropertyChanged;
        public WindowsProperties(InputController inputController)
        {
            InitializeComponent();
            this.DataContext = this;
            m_inputController = inputController;
        }

        private AssetsViewModel m_assetsViewModel;
        public AssetsViewModel AssetsViewModel {
            get => m_assetsViewModel;
            set
            {
                m_assetsViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AssetsViewModel"));
            }
        }

        private int m_minWindows;
        public int MinWindowsOnSelectedWall
        {
            get => m_minWindows;
            set
            {
                m_minWindows = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinWindowsOnSelectedWall"));
            }
        }

        private int m_maxWindows;
        public int MaxWindowsOnSelectedWall
        {
            get => m_maxWindows;
            set
            {
                m_maxWindows = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxWindowsOnSelectedWall"));
            }
        }

        private bool m_isSymmetryPreserved;
        public bool IsVerticalSymmetryPreserved
        {
            get => m_isSymmetryPreserved;
            set
            {
                m_isSymmetryPreserved = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVerticalSymmetryPreserved"));
            }
        }

        private bool m_isSingleStyle;
        public bool IsSingleStyleWindow
        {
            get => m_isSingleStyle;
            set
            {
                m_isSingleStyle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSingleStyleWindow"));
            }
        }

        private int m_selectedStyleIdx;
        public int SelectedStyleIdx
        {
            get => m_selectedStyleIdx;
            set
            {
                m_selectedStyleIdx = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStyleIdx"));
            }
        }

        private void OnPreviewClick(object sender, RoutedEventArgs e)
        {
            m_inputController.RequestVisualizeAsset(AssetsViewModel, "Windows", SelectedStyleIdx);
        }
    }
}
