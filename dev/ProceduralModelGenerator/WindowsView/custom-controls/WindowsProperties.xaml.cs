using GeneratorController;
using System.Windows;
using System.Windows.Controls;

namespace WindowsGeneratorView
{
    public partial class WindowsProperties : UserControl
    {
        private BuildingsViewModel m_viewModel;
        private InputController m_inputController;
        public WindowsProperties(InputController inputController)
        {
            InitializeComponent();
            this.DataContext = m_viewModel = inputController.ViewModel;
            m_inputController = inputController;
        }

        private void OnPreviewClick(object sender, RoutedEventArgs e)
        {
            m_inputController.RequestVisualizeAsset(m_viewModel.AssetsViewModel, "Windows", m_viewModel.SelectedWindowStyleIdx);
        }
    }
}
