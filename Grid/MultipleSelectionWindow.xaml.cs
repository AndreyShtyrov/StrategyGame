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
        }

        private UnitIcon Selected;

        public void Update(object sednder, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "Packs")
                return;

            int numberEnemy = 0;
            int numberAttacker = 0;
            foreach (var pack in data.Packs)
            {
                foreach (var enemy in pack.Enemyies)
                {
                    if (Enemies.Children[numberEnemy] is UnitIcon enemyIcon)
                    {
                        if (enemy != enemyIcon.Data)
                        {
                            Enemies.Children.RemoveAt(numberEnemy);
                            UnitIcon icon = new UnitIcon(enemy);
                            Enemies.Children.Insert(numberEnemy, icon);
                        }
                    }
                    else
                    {
                        Enemies.Children.RemoveAt(numberEnemy);
                        UnitIcon icon = new UnitIcon(enemy);
                        Enemies.Children.Insert(numberEnemy, icon);
                    }
                    numberEnemy++;
                }

                foreach (var attacker in pack.Attackers)
                {
                    if (Allies.Children[numberAttacker] is UnitIcon allyIcon)
                    {
                        if (attacker != allyIcon.Data)
                        {
                            Allies.Children.RemoveAt(numberAttacker);
                            UnitIcon icon = new UnitIcon(attacker);
                            Allies.Children.Insert(numberAttacker, icon);
                        }
                    }
                    else
                    {
                        Allies.Children.RemoveAt(numberAttacker);
                        UnitIcon icon = new UnitIcon(attacker);
                        Allies.Children.Insert(numberAttacker, icon);
                    }
                    numberAttacker++;
                }
                if ( !(Allies.Children[numberAttacker] is Separator))
                {
                    Allies.Children.Add(new Separator());
                }
                if ( !(Enemies.Children[numberEnemy] is Separator))
                {
                    Allies.Children.Add(new Separator());
                }
                numberAttacker++;
                numberEnemy++;
            }
        }


        public void RemoveUnit()
        {
            
        }

        public void RemovePack()
        {

        }

    }
}
