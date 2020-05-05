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
    /// Interaction logic for DoorsProperties.xaml
    /// </summary>
    public partial class DoorsProperties : UserControl
    {
        private InputController m_inputController;
        private BuildingsViewModel m_vm;

        /*
        private AssetsViewModel m_assetsViewModel;
        public AssetsViewModel AssetsViewModel
        {
            get => m_assetsViewModel;
            set
            {
                m_assetsViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AssetsViewModel"));
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
          */
        public DoorsProperties(InputController inputController)
        {
            InitializeComponent();
            m_inputController = inputController;
            this.DataContext = m_vm = inputController.ViewModel;
        }

        private void OnPreviewClick(object sender, RoutedEventArgs e)
        {
            m_inputController.RequestVisualizeAsset(m_vm.AssetsViewModel, "Doors", m_vm.SelectedDoorStyleIdx);
        }
    }
}
