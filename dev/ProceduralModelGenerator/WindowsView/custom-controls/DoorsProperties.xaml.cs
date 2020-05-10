using GeneratorController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindowsGeneratorView
{
    public partial class DoorsProperties : UserControl
    {
        private InputController m_inputController;
        private BuildingsViewModel m_vm;
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
