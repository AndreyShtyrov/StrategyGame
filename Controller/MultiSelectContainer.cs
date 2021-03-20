using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InterfaceOfObjects;

namespace Controller
{
    public class MultiSelectContainer : IMultiSelectContainer
    {
        private Player owner;

        public MultiSelectContainer(Player owner)
        {
            this.owner = owner;
            Packs = new List<IUnitsInBattle>();
            Packs.Add(new UnitsInBattle());
        }
        public List<IUnitsInBattle> Packs
        { get ; }
        
        public void AddUnit(UnitPresset unit)
        {
            if (!checkUnitInPacks(unit))
            {
                if (owner == unit.owner)
                {
                    Packs[Packs.Count - 1].Attackers.Add(unit);
                }
                else
                {
                    if (Packs[Packs.Count - 1].Enemyies.Count == 0)
                        Packs[Packs.Count - 1].Enemyies.Add(unit);
                }
                OnPropertyChanged("Packs");
            }
        }

        private bool checkUnitInPacks(UnitPresset unit)
        {
            foreach (var pack in Packs)
            {
                if (owner == unit.owner)
                {
                    if (pack.Attackers.Contains(unit))
                        return true;
                }
                else
                {
                    if (pack.Enemyies.Contains(unit))
                        return true;
                }
            }
            return false;
        }

        public void RemoveUnit(UnitPresset unit)
        {
            if (checkUnitInPacks(unit))
            {
                if (unit.owner == owner)
                {
                    Packs[Packs.Count - 1].Attackers.Remove(unit);
                }
                else
                {
                    Packs[Packs.Count - 1].Enemyies.Remove(unit);
                }
                OnPropertyChanged("Packs");
            }
        }

        public void RemoveLastPack()
        {
            if (Packs.Count > 1)
                Packs.RemoveAt(Packs.Count - 1);
            OnPropertyChanged("Packs");
        }

        public void AddNewPack()
        {
            Packs.Add(new UnitsInBattle());
            OnPropertyChanged("Packs");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class UnitsInBattle : IUnitsInBattle
    {
        public List<IUnitPresset> Attackers
        { get; set; }
        public List<IUnitPresset> Enemyies
        { get; set; }

        public UnitsInBattle()
        {
            Attackers = new List<IUnitPresset>();
            Enemyies = new List<IUnitPresset>();
        }
    }
}
