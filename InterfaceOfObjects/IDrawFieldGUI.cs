using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace InterfaceOfObjects
{
    public interface IDrawFieldGUI
    {
        public void removeField(ITokenData item);

        public void addField(ITokenData item);

        public void addUnit(IUnitPresset item);

        public void removeUnit(IUnitPresset item);

        public void addBuilding(ITokenData item);

        public void removeBuilding(ITokenData item);

        public void mouseBtnClicked(object sender, MouseEventArgs args);

        public void moveUnit(IUnitPresset unit);

        public void addWalkedArea(IEnumerable<ITokenData> list);

        public void clearWalkedArea();

        public void clearField();

        public Window generateMultipleSelectWindow(IMultiSelectContainer item);
    }
}
