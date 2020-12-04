using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DrawField
{
    public class HexagonalGrid
    {
        static HexagonalGrid instance;

        public double gridSize
        {
            set
            {
                _gridSize = value;
                gridHeight = 2 * value;
                gridWidth = Math.Sqrt(3) * value;
            }
            get
            {
                return _gridSize;
            }
        }
        private double _gridSize;
        private double gridHeight;
        private double gridWidth;

        static public HexagonalGrid get()
        {
            if (instance == null)
            {
                instance = new HexagonalGrid();
                instance.gridSize = 30;
            }
            return instance;
        }

        public Vector getStartedShift(int yshift)
        {
            if (yshift % 2 == 0)
                return new Vector(gridWidth, 0.75 * gridHeight * yshift + 0.5 * gridHeight);
            else
                return new Vector(0.5 * gridWidth, 0.75 * gridHeight * yshift + 0.5 * gridHeight);
        }

        public Vector getShift((int X, int Y) fpos)
        {
            var shift = getStartedShift(fpos.Y);
            Vector center = new Vector(fpos.X * gridWidth + shift.X, shift.Y);
            return center;
        }
    }
}
