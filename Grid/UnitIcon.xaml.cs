using InterfaceOfObjects;
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

namespace DrawField
{
    /// <summary>
    /// Interaction logic for UnitIcon.xaml
    /// </summary>
    public partial class UnitIcon : UserControl
    {
        public UnitIcon(IUnitPresset data)
        {
            InitializeComponent();
            Data = data;
            UnitName.Content = data.getUnitName();
        }

        public bool IsSelected
        {
            get
            {
                return Data.isSelected;
            }
            set
            {
                Data.isSelected = value;
            }
        }

        public IUnitPresset Data 
        { get; set; }
    }
}
