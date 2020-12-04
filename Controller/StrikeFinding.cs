using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;
using InterfaceOfObjects;

namespace Controller
{
    public class StrikeFinding
    {
        private int width;
        private int height;
        public StrikeFinding(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public List<UnitPresset> getListOfTargets(UnitPresset unit, int Range, UnitPresset[,] units)
        {
            List<UnitPresset> result = new List<UnitPresset>();
            foreach (var pos in checkNearestArea(unit.fieldPosition, 1))
            {
                if (units[pos.X, pos.Y] != null)
                {
                    if (unit.owner != units[pos.X, pos.Y].owner)
                        result.Add(units[pos.X, pos.Y]);
                }
            }
            return result;
        }

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center)
        {
            if (center.Y % 2 == 0)
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (int)(col + (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
            else
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (col - (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
        }

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center)
        {
            (int X, int Y) result;
            if (center.Y % 2 == 0)
            {
                result.X = fpos.X + (fpos.Z + (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            else
            {
                result.X = fpos.X + (fpos.Z - (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            return (result.X + center.X, result.Y + center.Y);
        }

        public IEnumerable<(int X, int Y)> checkNearestArea((int X, int Y) fpos, int Range)
        {
            for (int i = fpos.X - 1; i < fpos.X + 2; i++)
            {
                for (int j = fpos.Y - 1; j < fpos.Y + 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < width && j < height)
                    {
                        var xyz = TransformToCube((i,j), fpos);
                        if (Math.Abs(xyz.X) + Math.Abs(xyz.Y) + Math.Abs(xyz.Z) == 2 * Range)
                            yield return (i, j);
                    }
                }
            }
            yield break;
        }
    }
}
