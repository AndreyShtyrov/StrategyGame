using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InterfaceOfObjects;

namespace DrawField
{
    /// <summary>
    /// Interaction logic for MultipleSelectionWindow.xaml
    /// </summary>
    public partial class MultipleSelectionWindow : Window
    {
        private IMultiSelectContainer data;
        public MultipleSelectionWindow(IMultiSelectContainer idata)
        {
            InitializeComponent();
            data = idata;
            data.PropertyChanged += Update;
        }

        private UnitIcon Selected;

        public void Update(object sednder, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "Packs")
                return;
            foreach ( var item in  Enemies.Children)
            {
                if (item is UnitIcon delet_item)
                    delet_item.MouseDown -= ClickOnUnitIcon;
            }
            foreach ( var item in Allies.Children)
            {
                if (item is UnitIcon delet_item)
                    delet_item.MouseDown -= ClickOnUnitIcon;
            }
            Enemies.Children.Clear();
            Allies.Children.Clear();
            foreach (var pack in data.Packs)
            {
                foreach (var enemy in pack.Enemyies)
                {
                    UnitIcon icon = new UnitIcon(enemy);
                    icon.MouseDown += ClickOnUnitIcon;
                    Enemies.Children.Add(icon);
                }
                foreach(var ally in pack.Attackers)
                {
                    UnitIcon icon = new UnitIcon(ally);
                    icon.MouseDown += ClickOnUnitIcon;
                    Allies.Children.Add(icon);
                }
                if (data.Packs.IndexOf(pack) + 1 < data.Packs.Count)
                {
                    Enemies.Children.Add(new Separator());
                    Allies.Children.Add(new Separator());
                }
            }
        }

        private void RemoveUnitButton(object sender, RoutedEventArgs e)
        {
            if (Selected != null)
            {
                data.RemoveUnit(Selected.Data);
            }
        }
        
        private void ClickOnUnitIcon(object sender, MouseButtonEventArgs args)
        {
            if (args.RightButton is MouseButtonState.Pressed)
            {
                Selected.IsSelected = false;
                Selected = null;
                return;
            }
            if (sender is UnitIcon  unitIcon)
            {
                if (Selected != null)
                    Selected.IsSelected = false;
                Selected = unitIcon;
                unitIcon.IsSelected = true;
            }
            
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (Selected == null)
                return;
            Selected.IsSelected = false;
            Selected = null;
            return;
        }

        private void RemovePack(object sender, RoutedEventArgs e)
        {
            data.RemoveLastPack();
        }

        private void AddPack(object sender, RoutedEventArgs e)
        {
            data.AddNewPack();
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            data.Apply();
        }
    }
}
