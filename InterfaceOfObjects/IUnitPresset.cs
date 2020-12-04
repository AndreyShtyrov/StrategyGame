using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace InterfaceOfObjects
{

    public interface IUnitPresset : ITokenData
    {
        public int currentHp
        { get; set; }

        public bool isSelected
        { get; set; }
        public bool isTarget
        { get; set; }
        public string getUnitName();

        public (int X, int Y) fieldPosition
        { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
