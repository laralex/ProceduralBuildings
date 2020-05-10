using System.ComponentModel;

namespace GeneratorController.viewmodel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private string m_applicationStatus;
        public string ApplicationStatus
        {
            get => m_applicationStatus;
            set
            {
                m_applicationStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplicationStatus"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
