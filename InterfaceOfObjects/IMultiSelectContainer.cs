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


        public void AddUnit(IUnitPresset iUnit);

        public void RemoveUnit(IUnitPresset iUnit);
    }

    public interface IUnitsInBattle
    {
        public List<IUnitPresset> Attackers
        { get; set; }

        public List<IUnitPresset> Enemyies
        { get; set; }

    }
}
