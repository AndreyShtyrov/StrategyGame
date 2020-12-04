using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grid;

namespace UnitsAnPathFinding
{
    /// <summary>
    /// Interaction logic for UnitGUI.xaml
    /// </summary>
    public partial class UnitGUI : UserControl, ITokenGui
    {
        private IUnitData data;
        private double gridSize;

        public UnitGUI(IUnitData idata, Vector center, double size)
        {
            InitializeComponent();
            data = idata;
            gridSize = size;
            PointCollection Vertexs = new PointCollection();
            CPoligon.Stroke = Brushes.Black;
            CPoligon.Fill = idata.getBackGround();
            Vector defalutCenter = new Vector(Math.Sqrt(3) * size, size);
            for (int i = 0; i < 6; i++)
            {
                var vertex = pointyHexCorner(i);
                Point PVertex = new Point((int)Math.Round(vertex.X), (int)Math.Round(vertex.Y));
                Vertexs.Add(PVertex + defalutCenter);
            }
            CPoligon.Points = Vertexs;
            tranlateGrid.X = center.X;
            tranlateGrid.Y = center.Y;

        }

        public ITokenData getToken()
        {
            return data;
        }

        private Vector pointyHexCorner(int number)
        {
            var angleRadian = Math.PI / 180 * (60 * number - 30);
            return new Vector(gridSize * Math.Cos(angleRadian), gridSize * Math.Sin(angleRadian));
        }
    
    }
}
