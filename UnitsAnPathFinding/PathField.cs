using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using InterfaceOfObjects;

namespace UnitsAnPathFinding
{
    public class PathToken: ITokenData
    {
        private string _gridType;
        public bool isPassable = true;
        private (int X, int Y) _position;

        public (int X, int Y) fieldPosition => _position;
        public List<PathToken> tracePath = new List<PathToken>();
        public float pathLeght
        { get; set; }
        public bool isReachable
        { get; set; }
        public string fieldtype => _gridType;


        public PathToken(string gridType, (int X, int Y) position)
        {
            isReachable = false;
            this._gridType = gridType;
            if (gridType == "Montain")
            {
                this.isPassable = false;
            }
            else
            {
                isPassable = true;
            }
            _position = position;
            pathLeght = 0;
        }

        public void cleanState()
        {
            pathLeght = 0;
            isReachable = false;
            tracePath = new List<PathToken>();
        }

        public SolidColorBrush getBackGround()
        {
            var template = Brushes.Blue;
            SolidColorBrush brash = new SolidColorBrush(template.Color);
            brash.Opacity = 0.2;
            return brash;
        }
    }
}
