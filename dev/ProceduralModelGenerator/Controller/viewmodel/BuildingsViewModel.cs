using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GeneratorController
{
    public class BuildingsViewModel : IViewModel
    {
        //public IViewModel BasementSettings { get; set; }
        //public IViewModel RoofSettings { get; set; }
        //public IViewModel SegmentingSettings { get; set; }
        //public IViewModel WindowsSettings { get; set; }
        //public IViewModel DoorsSettings { get; set; }
        //public IViewModel AssetsViewModel { get; set; }

        public BuildingsViewModel()
        {
            SelectedSideMeters = 5.0f;
            BuildingMinHeight = 6.0f;
            PolygonPoints = new List<Point>();
            SelectedSideEndpoint1 = -1;
            SelectedSideEndpoint2 = -1;
        }

        private string m_seedString;
        public string SeedString
        {
            get => m_seedString;
            set { m_seedString = value; NotifyChange("SeedString"); }
        }

        private float m_selectedSideLength;
        public float SelectedSideMeters
        {
            get => m_selectedSideLength;
            set { m_selectedSideLength = value; NotifyChange("SelectedSideLength"); }
        }

        private float m_buildingMinHeight;
        public float BuildingMinHeight
        {
            get => m_buildingMinHeight;
            set {m_buildingMinHeight = value; NotifyChange("BuildingMinHeight");}
        }

        private float m_buildingMaxHeight;
        public float BuildingMaxHeight
        {
            get => m_buildingMaxHeight;
            set { m_buildingMaxHeight = value; NotifyChange("BuildingMaxHeight"); }
        }

        private IList<Point> m_polygonPoints;
        public IList<Point> PolygonPoints
        {
            get => m_polygonPoints;
            set { m_polygonPoints = value;NotifyChange("PolygonPoints");}
        }

        private int m_selectedSideEndpoint1;
        public int SelectedSideEndpoint1
        {
            get => m_selectedSideEndpoint1;
            set{ m_selectedSideEndpoint1 = value;NotifyChange("SelectedSideEndpoint1");}
        }

        private int m_selectedSideEndpoint2;
        public int SelectedSideEndpoint2
        {
            get => m_selectedSideEndpoint2;
            set{ m_selectedSideEndpoint2 = value; NotifyChange("SelectedSideEndpoint2");}
        }

        private AssetsViewModel m_assetsViewModel;
        public AssetsViewModel AssetsViewModel
        {
            get => m_assetsViewModel;
            set { m_assetsViewModel = value;NotifyChange("AssetsViewModel");}
        }

        private int m_selectedDoorStyleIdx;
        public int SelectedDoorStyleIdx
        {
            get => m_selectedDoorStyleIdx;
            set { m_selectedDoorStyleIdx = value; NotifyChange("SelectedDoorStyleIdx");}
        }

        private bool m_isDoorOnSelectedWall;
        public bool IsDoorOnSelectedWall
        {
            get => m_isDoorOnSelectedWall;
            set { m_isDoorOnSelectedWall = value; NotifyChange("IsDoorOnSelectedWall"); }
        }

        private float m_roofMinHeight;
        public float RoofMinHeight
        {
            get => m_roofMinHeight;
            set { m_roofMinHeight = value; NotifyChange("RoofMinHeight");}
        }

        private float m_roofMaxHeight;
        public float RoofMaxHeight
        {
            get => m_roofMaxHeight;
            set{ m_roofMaxHeight = value;NotifyChange("RoofMaxHeight"); }
        }

        private RoofStyle m_roofStyle;
        public RoofStyle RoofStyle
        {
            get => m_roofStyle;
            set { m_roofStyle = value;NotifyChange("RoofStyle"); }
        }

        private int m_minNumberOfFloors;
        public int MinNumberOfFloors
        {
            get => m_minNumberOfFloors;
            set {m_minNumberOfFloors = value;NotifyChange("MinNumberOfFloors");}
        }

        private int m_maxNumberOfFloors;
        public int MaxNumberOfFloors
        {
            get => m_maxNumberOfFloors;
            set {m_maxNumberOfFloors = value;NotifyChange("MaxNumberOfFloors");}
        }

        private int m_minVerticalSplitsNumber;
        public int MinSelectedWallHorizontalSegments
        {
            get => m_minVerticalSplitsNumber;
            set {m_minVerticalSplitsNumber = value;NotifyChange("MinSelectedWallHorizontalSegments");}
        }

        private int m_maxHorizontalSegmentsNumber;
        public int MaxSelectedWallHorizontalSegments
        {
            get => m_maxHorizontalSegmentsNumber;
            set {m_maxHorizontalSegmentsNumber = value; NotifyChange("MaxSelectedWallHorizontalSegments");}
        }


        private int m_minWindows;
        public int MinWindowsOnSelectedWall
        {
            get => m_minWindows;
            set { m_minWindows = value; NotifyChange("MinWindowsOnSelectedWall");}
        }

        private int m_maxWindows;
        public int MaxWindowsOnSelectedWall
        {
            get => m_maxWindows;
            set { m_maxWindows = value; NotifyChange("MaxWindowsOnSelectedWall");}
        }

        private bool m_isSymmetryPreserved;
        public bool IsVerticalSymmetryPreserved
        {
            get => m_isSymmetryPreserved;
            set { m_isSymmetryPreserved = value; NotifyChange("IsVerticalSymmetryPreserved"); }
        }

        private bool m_isSingleStyle;
        public bool IsSingleStyleWindow
        {
            get => m_isSingleStyle;
            set { m_isSingleStyle = value; NotifyChange("IsSingleStyleWindow");}
        }

        private int m_selectedWindowStyleIdx;
        public int SelectedWindowStyleIdx
        {
            get => m_selectedWindowStyleIdx;
            set { m_selectedWindowStyleIdx = value; NotifyChange("SelectedWindowStyleIdx");}
        }

        private void NotifyChange(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

public enum RoofStyle
{
    Flat, SlopeFlat, Slope
}
