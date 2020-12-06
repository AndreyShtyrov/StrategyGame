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
using InterfaceOfObjects;
using Tokens;


namespace DrawField
{
    /// <summary>
    /// Interaction logic for FieldGUI.xaml
    /// </summary>
    public partial class FieldGUI : UserControl
    {
        private HexagonalGrid gridSettings;
        public Field data;
        public UnitGUI selected;
        public GameModeHandler gameModeHandler;
        private List<PathTokenGUI> walkedArea = new List<PathTokenGUI>();
        public FieldGUI()
        {

            InitializeComponent();
            gridSettings = HexagonalGrid.get();
        }

        public void drawGrid(Field field)
        {
            data = field;
            foreach (var grid in data.grids)
            {
                SimpleTokenGui ng = new SimpleTokenGui(grid);
                ng.MouseDown += mouseBtnClicked;
                FieldMap.Children.Add(ng);
            }
        }

        public void removeField(ITokenData item)
        {
            UIElement deletItem = null; 
            foreach (UIElement grid in FieldMap.Children)
            {
                if (grid is ITokenGui tgrid)
                {
                    if (item == tgrid.getToken())
                    {
                        deletItem = grid;
                        break;
                    }
                }
            }
            FieldMap.Children.Remove(deletItem);
        }

        public void addField(ITokenData item)
        {
            SimpleTokenGui ng = new SimpleTokenGui(item);
            ng.MouseDown += mouseBtnClicked;
            FieldMap.Children.Add(ng);
        }

        public void addUnit(IUnitPresset item)
        {
            UnitGUI ng = new UnitGUI(item);
            ng.MouseDown += mouseBtnClicked;
            FieldMap.Children.Add(ng);
        }

        public void mouseBtnClicked(object sender, MouseEventArgs e)
        {
            if (sender is UnitGUI unit)
            {
                var data = unit.data;
                gameModeHandler(data, e);
            }
            if (sender is SimpleTokenGui grid)
            {
                gameModeHandler(grid.getToken(), e);
            }
            if (sender is PathTokenGUI pathPoint)
            {
                gameModeHandler(pathPoint.getToken(), e);
            }
        }

        public void moveUnit(IUnitPresset unit)
        {
            foreach (UIElement grid in FieldMap.Children)
            {
                if (grid is UnitGUI tgrid)
                {
                    if (unit == tgrid.data)
                    {
                        tgrid.move(unit.fieldPosition);
                    }
                }
            }
        }

        public void addWalkedArea(IEnumerable<ITokenData> list)
        {
            foreach (var item in list)
            {
                PathTokenGUI ng = new PathTokenGUI(item);
                ng.MouseDown += mouseBtnClicked;
                walkedArea.Add(ng);
                FieldMap.Children.Add(ng);
            }
        }

        public void clearWalkedArea()
        {
            foreach (var pathtoken in walkedArea)
            {
                FieldMap.Children.Remove(pathtoken);
            }
            walkedArea.Clear();
        }

        public void clearField()
        {
            FieldMap.Children.Clear();
        }

    }
    public delegate void GameModeHandler(ITokenData sender, MouseEventArgs e);
}
