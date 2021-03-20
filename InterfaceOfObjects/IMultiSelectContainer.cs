using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InterfaceOfObjects
{
    public interface IMultiSelectContainer : INotifyPropertyChanged
    {
        public List<IUnitsInBattle> Packs
        { get; }
    }

    public interface IUnitsInBattle
    {
        public List<IUnitPresset> Attackers
        { get; set; }

        public List<IUnitPresset> Enemyies
        { get; set; }
    }
}
