using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;
using InterfaceOfObjects;

namespace Controller
{
    public class PathField
    {

        public PathToken[,] grids;
        public List<ITokenData> _calculatedPath = new List<ITokenData>();
        private int width;
        private int height;
        private UnitPresset[,] units;
        private UnitPresset SelectedUnit;

        public PathField(IListOfToken field)
        {
            grids = new PathToken[field.width, field.height];
            width = field.width;
            height = field.height;
            foreach (var grid in field.grids)
            {
                grids[grid.fieldPosition.X, grid.fieldPosition.Y] = new PathToken(grid.fieldtype, grid.fieldPosition);
            }
        }

        public void findArea(float walkDistance, (int X, int Y) fpos, bool isbonuc)
        {
            _calculatedPath = new List<ITokenData>();
            List<PathToken> listPointToCheck = new List<PathToken>();
            listPointToCheck.Add(grids[fpos.X, fpos.Y]);
            List<PathToken> newlistPointToCheck = new List<PathToken>();
            bool trigger = true;
            while (listPointToCheck.Count != 0)
            {
                foreach (var grid in listPointToCheck)
                {
                    foreach (var pathgrid in checkNearestArea(grid.fieldPosition))
                    {
                        if (Step(grid, pathgrid, walkDistance, isbonuc))
                        {
                            newlistPointToCheck.Add(pathgrid);
                            if (!_calculatedPath.Contains(pathgrid))
                                _calculatedPath.Add(pathgrid);
                        }
                    }
                }
                listPointToCheck = newlistPointToCheck;
                newlistPointToCheck = new List<PathToken>();
            }
        }

        public List<ITokenData> getWalkArea(float walkDistance, UnitPresset unit, UnitPresset[,] allunits, bool isbonuc = true)
        {
            SelectedUnit = unit;
            var fpos = unit.fieldPosition;
            units = allunits;
            findArea(walkDistance, fpos, isbonuc);
            return _calculatedPath;
        }

        public void Refresh()
        {
            SelectedUnit = null;
            foreach (var pathgrid in _calculatedPath)
            {
                grids[pathgrid.fieldPosition.X, pathgrid.fieldPosition.Y].cleanState();
            }
        }

        public IEnumerable<PathToken> checkNearestArea((int X, int Y) fpos)
        {
            for (int i = fpos.X - 1; i < fpos.X + 2; i++)
            {
                for (int j = fpos.Y - 1; j < fpos.Y + 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < width && j < height)
                    {
                        var xyz = TransformToCube((i, j), fpos);
                        if (CubeNorm(xyz) == 2)
                            yield return grids[i, j];
                    }
                }
            }
            yield break;
        }

        public List<UnitPresset> getListOfTargets(UnitPresset unit, int Range, UnitPresset[,] allunits)
        {
            units = allunits;
            List<UnitPresset> result = new List<UnitPresset>();
            foreach (var pos in iterAxialNearestCoord(unit.fieldPosition, 1))
            {
                if (units[pos.X, pos.Y] == null)
                {
                    continue;
                }
                if (unit.owner != units[pos.X, pos.Y].owner)
                    result.Add(units[pos.X, pos.Y]);
            }
            List<(int X, int Y)> listOfTargets = new List<(int X, int Y)>();
            
            for (int w = Range; w > 1; w--)
            {
                foreach (var pos in iterAxialNearestCoord(unit.fieldPosition, w))
                {
                    if (units[pos.X, pos.Y] == null)
                    {
                        continue;
                    }
                    var target = units[pos.X, pos.Y];
                    if (unit.owner != target.owner)
                    {
                        var line = CreateLine(pos, unit.fieldPosition);
                        if (checkUnitsOnLine(line, unit.fieldPosition,
                            GetGroundType(unit.fieldPosition), GetGroundType(pos), w))
                            result.Add(target);
                    }
                }
            }
            return result;
        }

        private bool checkUnitsOnLine(List<(int X, int Y, int Z)> line, (int X, int Y) central, 
            GroundType tgroundType, GroundType cgroundType, int Range)
        {
            if (Range == 2)
            {
                if (line[0].X != 0 && line[0].Y != 0 && line[0].Z != 0)
                {
                    bool trigger = true;
                    var xy = TransformToAxial(line[1], central);
                    if (grids[xy.X, xy.Y].fieldtype == "Montain")
                    {
                        trigger = false;
                    }
                    if (units[xy.X, xy.Y] != null)
                    {
                        if ((cgroundType == GroundType.HighGround || tgroundType == GroundType.HighGround)
                            && GetGroundType(xy) == GroundType.HighGround)
                        {
                            trigger = false;
                        }
                        if (cgroundType == GetGroundType(xy))
                        {
                            trigger = false;
                        }
                    }
                    var xyz = line[1];
                    xyz = (line[0].X - xyz.X, line[0].Y - xyz.Y, line[0].Z - xyz.Z);
                    xy = TransformToAxial(xyz, central);
                    if (grids[xy.X, xy.Y].fieldtype == "Montain")
                    {
                        if (!trigger)
                            return false;
                    }
                    if (units[xy.X, xy.Y] != null)
                    {
                        if ((cgroundType == GroundType.HighGround || tgroundType == GroundType.HighGround)
                            && GetGroundType(xy) == GroundType.HighGround)
                        {
                            if (!trigger)
                                return false;
                        }
                        if (cgroundType == GetGroundType(xy))
                        {
                            if (!trigger)
                                return false;
                        }
                    }
                    return true;
                }
            }

            for ( int j = 1; j < line.Count-1; j++)
            {
                var xy = TransformToAxial(line[j], central);
                if (grids[xy.X, xy.Y].fieldtype == "Montain")
                {
                    return false;
                }
                if (units[xy.X, xy.Y] != null)
                {
                    if ((cgroundType == GroundType.HighGround || tgroundType == GroundType.HighGround)
                        && GetGroundType(xy) == GroundType.HighGround)
                    {
                        return false;
                    }
                    if (cgroundType == GetGroundType(xy))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        public int CubeNorm((int X, int Y, int Z) xyz)
        {
            return Math.Abs(xyz.X) + Math.Abs(xyz.Y) + Math.Abs(xyz.Z) ;
        }

        private GroundType GetGroundType((int X, int Y) fpos)
        {
            if (grids[fpos.X, fpos.Y].fieldtype == "Hill")
                return GroundType.HighGround;
            return GroundType.LowGround;
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

        public IEnumerable<(int X, int Y)> iterAxialNearestCoord((int X, int Y) fpos, int Range = 2)
        {
            for (int i = fpos.X - 1 - Range; i < fpos.X + 1 + Range; i++)
            {
                for (int j = fpos.Y - 1 - Range; j < fpos.Y + 2 + Range; j++)
                {
                    if (i >= 0 && j >= 0 && i < width && j < height)
                    {
                        var xyz = TransformToCube((i, j), fpos);
                        if (Math.Abs(xyz.X) + Math.Abs(xyz.Y) + Math.Abs(xyz.Z) == 2 * Range)
                            yield return (i, j);
                    }
                }
            }
            yield break;
        }

        public List<(int X, int Y, int Z)> CreateLine((int X, int Y) fpos, (int X, int Y) center)
        {
            List<(int X, int Y, int Z)> result = new List<(int X, int Y, int Z)>();
            (int X, int Y, int Z) xyzcenter = (0, 0, 0);
            var xyz = TransformToCube(fpos, center);
            var distance = CubeNorm(xyz);
            for (int i = 0 ; i <= distance; i+=2)
            {
                var d = (float) distance / 2;
                var ti = (float) i / 2;
                var pointxyz = CubeRound(cube_lerp(xyz, xyzcenter, ((1 / d) * ti)));
                result.Add(pointxyz);
            }
            return result;
        }

        private bool Step(PathToken prevStep, PathToken nextStep, float maxlenght, bool isbonus = true)
        {
            float steplenght = 1f;
            if (nextStep.fieldPosition == SelectedUnit.fieldPosition)
                return false;
            if (prevStep.fieldtype == nextStep.fieldtype)
                steplenght = GetStepValue(nextStep);
            if (!nextStep.isPassable)
                return false;
            if (units[nextStep.fieldPosition.X, nextStep.fieldPosition.Y] != null)
            {
                if (units[nextStep.fieldPosition.X, nextStep.fieldPosition.Y].owner != SelectedUnit.owner)
                    return false;
            }

            if (nextStep.isReachable)
            {
                if (prevStep.pathLeght + steplenght <= maxlenght 
                    && prevStep.pathLeght + steplenght < nextStep.pathLeght)
                {
                    nextStep.pathLeght = steplenght + prevStep.pathLeght;
                    return true;
                }
            }
            else
            {
                if (prevStep.pathLeght + steplenght <= maxlenght)
                {
                    nextStep.pathLeght = steplenght + prevStep.pathLeght;
                    nextStep.isReachable = true;
                    List<PathToken> newtracePath = new List<PathToken>();
                    newtracePath.Add(prevStep);
                    newtracePath.AddRange(prevStep.tracePath);
                    nextStep.tracePath = newtracePath;
                    return true;
                }
            }
            return false;
        }

        private float GetStepValue(PathToken nextStep, bool isbonus = true)
        {
            if (!isbonus)
                return 1f;
            if (nextStep.fieldtype == "Road")
            {
                return 0.5f;
            }
            else
            {
                return 1f;
            }
        }

        public (int X, int Y, int Z) CubeRound((float X, float Y, float Z) fxyz)
        {
            var rx = (int)Math.Round(fxyz.X);
            var ry = (int)Math.Round(fxyz.Y);
            var rz = (int)Math.Round(fxyz.Z);

            var x_diff = Math.Abs(rx - fxyz.X);
            var y_diff = Math.Abs(ry - fxyz.Y);
            var z_diff = Math.Abs(rz - fxyz.Z);

            if (x_diff > y_diff && y_diff > z_diff)
                rx = -ry - rz;
            else
                if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;
            return (rx, ry, rz);
        }

        public float lepr(int a, int b, float t) => a + (b - a) * t;

        public (float X, float Y, float Z) cube_lerp((int X, int Y, int Z) a, (int X, int Y, int Z) b, float t)
            => (lepr(a.X, b.X, t), lepr(a.Y, b.Y, t), lepr(a.Z, b.Z, t));
    }

    enum GroundType
    {
        LowGround = 0,
        HighGround = 1,
    }
}


