using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneratorController
{
    public class BasementPropertiesViewModel : INotifyPropertyChanged, IViewModel
    {
        public BasementPropertiesViewModel()
        {
            SelectedSideMeters = 5.0f;
            BuildingMinHeight = 6.0f;
            PolygonPoints = new List<Point>();
            m_selectedSideEndpoint1 = -1;
            m_selectedSideEndpoint2 = -1;
        }

        private float m_selectedSideLength;
        public float SelectedSideMeters
        {
            get => m_selectedSideLength;
            set
            {
                m_selectedSideLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSideLength"));
            }
        }

        private float m_buildingMinHeight;
        public float BuildingMinHeight
        {
            get => m_buildingMinHeight;
            set
            {
                m_buildingMinHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuildingMinHeight"));
            }
        }

        private float m_buildingMaxHeight;
        public float BuildingMaxHeight
        {
            get => m_buildingMaxHeight;
            set
            {
                m_buildingMaxHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuildingMaxHeight"));
            }
        }

        private IList<Point> m_polygonPoints;
        public IList<Point> PolygonPoints
        {
            get => m_polygonPoints;
            set
            {
                m_polygonPoints = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PolygonPoints"));
            }
        }

        private int m_selectedSideEndpoint1;
        public int SelectedSideEndpoint1
        {
            get => m_selectedSideEndpoint1;
            set
            {
                m_selectedSideEndpoint1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSideEndpoint1"));
            }
        }

        private int m_selectedSideEndpoint2;
        public int SelectedSideEndpoint2
        {
            get => m_selectedSideEndpoint2;
            set
            {
                m_selectedSideEndpoint2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSideEndpoint2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
