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
using System.ComponentModel;
using InterfaceOfObjects;
using System.Net.Mime.MediaTypeNames.Application;

namespace DrawField
{
    /// <summary>
    /// Interaction logic for UnitGUI.xaml
    /// </summary>
    public partial class UnitGUI : UserControl, ITokenGui

    {
        public IUnitPresset data;

        private void movingHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "fieldPosition" && sender is IUnitPresset tsender)
            {
                move(tsender.fieldPosition);
            }
        }

        public UnitGUI(IUnitPresset idata)
        {
            InitializeComponent();
            var gridSettings = HexagonalGrid.get();
            data = idata;
            PointCollection Vertexs = new PointCollection();
            CPoligon.Stroke = Brushes.Black;
            CPoligon.Fill = idata.getBackGround();
            var size = gridSettings.gridSize;
            Vector defalutCenter = new Vector(Math.Sqrt(3) * size, size);
            for (int i = 0; i < 6; i++)
            {
                var vertex = pointyHexCorner(i);
                Point PVertex = new Point((int)Math.Round(vertex.X), (int)Math.Round(vertex.Y));
                Vertexs.Add(PVertex + defalutCenter);
            }
            CPoligon.Points = Vertexs;
            var center = gridSettings.getShift(idata.fieldPosition);
            tranlateGrid.X = center.X;
            tranlateGrid.Y = center.Y;
            UnitName.Content = idata.getUnitName();
            DataContext = data;
            idata.PropertyChanged += movingHandler;
            idata.PropertyChanged += isSelectedTargetHandler;
            idata.PropertyChanged += iActionPointHandler;
        }

        public ITokenData getToken()
        {
            return data;
        }

        private Vector pointyHexCorner(int number)
        {
            var gridSize = HexagonalGrid.get().gridSize;
            var angleRadian = Math.PI / 180 * (60 * number - 30);
            return new Vector(gridSize * Math.Cos(angleRadian), gridSize * Math.Sin(angleRadian));
        }


        public void move((int X, int Y) fpos)
        {
            var center = HexagonalGrid.get().getShift(fpos);
            tranlateGrid.X = center.X;
            tranlateGrid.Y = center.Y;
        }


        private void isSelectedTargetHandler(object sender, PropertyChangedEventArgs e)
        {
            if ( e.PropertyName == "isSelected" && sender is IUnitPresset select)
            {
                if (select.isSelected)
                {
                    CPoligon.Stroke = Brushes.Yellow;
                }
                else
                {
                    CPoligon.Stroke = Brushes.Black;
                }
            }
            if ( e.PropertyName == "isTarget" && sender is IUnitPresset target)
            {
                if (target.isTarget)
                {
                    CPoligon.Stroke = Brushes.Orange;
                }
                else
                {
                    CPoligon.Stroke = Brushes.Black;
                }
            }
        }

        private void iActionPointHandler(object sender, PropertyChangedEventArgs e)
        {
            if (sender is List<int> tsender)
            {
                Attack1.Fill = data.getBackGround();
                Attack2.Fill = data.getBackGround();
                Move1.Fill = data.getBackGround();
                Move2.Fill = data.getBackGround();

                foreach (var ActionIndex in tsender)
                {
                    switch (ActionIndex)
                    {
                        case 0:
                            { Attack1.Fill = Brushes.Red; break; }
                        case 1:
                            { Attack2.Fill = Brushes.Red; break; }
                        case 2:
                            { Move1.Fill = Brushes.Green; break; }
                        case 3:
                            { Move2.Fill = Brushes.Green; break; }
                    }
     
                }
            }

            
        }
    }
}
