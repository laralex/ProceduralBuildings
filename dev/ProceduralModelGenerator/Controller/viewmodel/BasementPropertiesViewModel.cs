using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneratorController
{
    public class BasementPropertiesViewModel : INotifyPropertyChanged
    {
        public BasementPropertiesViewModel()
        {
            SelectedSideLength = 1.0f;
            BuildingHeight = 5.0f;
            PolygonPoints = new List<Point>();
            m_selectedSideEndpoint1 = -1;
            m_selectedSideEndpoint2 = -1;
        }

        private float m_selectedSideLength;
        public float SelectedSideLength
        {
            get => m_selectedSideLength;
            set
            {
                m_selectedSideLength = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSideLength"));
            }
        }

        private float m_buildingHeight;
        public float BuildingHeight
        {
            get => m_buildingHeight;
            set
            {
                m_buildingHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuildingHeight"));
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
