using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InterfaceOfObjects;

namespace Tokens
{
    public class Field : IListOfToken, INotifyPropertyChanged
    {
        public int width
        { set; get; }
        public int height
        { set; get; }
        

        [JsonConstructor]
        public Field()
        {
            grids = new List<ITokenData>();
            gridSize = 20;
        }

        public Field(int width, int height)
        {
            grids = new List<ITokenData>();
            gridSize = 20;
            this.width = width;
            this.height = height;
            fillByGrids(width, height);
        }

        public double gridSize
        {
            get { return _gridSize; }
            set
            {
                _gridSize = value;
                RaisePropertyChangedEvent("gridSize");
            }
        }

        private double _gridSize;

        private List<ITokenData> _grids;
        public List<ITokenData> grids
        {
            set { _grids = value; }
            get { return _grids; }
        }

        protected void RaisePropertyChangedEvent(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void fillByGrids( int rNumber, int hNumber)
        {
            
            for ( int i = 0; i < hNumber; i++)
            {
                for (int j = 0; j < rNumber; j++)
                {
                    TokenData ng = new TokenData((j, i));

                    grids.Add(ng);
                }
            }
        }

        public ITokenData getData((int X, int Y) fpos)
        {
           foreach ( var unit in grids)
            {
                if (unit.fieldPosition == fpos)
                {
                    return unit;
                }
            }
            return null;
        }
        public void save(FileInfo savefile)
        {
            using (var fs = File.Create(savefile.ToString()))
            using (var sw = new StreamWriter(fs))
            {
                var jsonstring = JsonConvert.SerializeObject(this,
                    new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                sw.WriteLine(jsonstring);
            }
        }

        public static Field load(FileInfo loadfile)
        {
            using (var fs = File.OpenRead(loadfile.ToString()))
            using (var sr = new StreamReader(fs))
            {
                var jsonstring = sr.ReadLine();
                return JsonConvert.DeserializeObject<Field>(jsonstring, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
