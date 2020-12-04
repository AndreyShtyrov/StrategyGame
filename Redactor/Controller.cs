using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Text;
using System.Windows.Input;
using DrawField;
using System.IO;
using InterfaceOfObjects;
using Tokens;
using Tokens.TokenFields;

namespace Redactor
{

    class Controller
    {
        public string selected="Empty";
        private FieldGUI fieldgui;
        private Field field;

        public Controller(Field field, FieldGUI fieldgui)
        {
            this.field = field;
            this.fieldgui = fieldgui;
            
        }

        public void btn_pressed(object sender, MouseEventArgs e)
        {

            if (sender is SimpleTokenGui  tsender)
            {
                var data = tsender.getToken();
                if (data.fieldtype != selected)
                {
                    fieldgui.removeField(data);
                    var ntoken = generate_new_token(selected, data.fieldPosition);
                    fieldgui.addField(ntoken);
                    field.grids.Remove(data);
                    field.grids.Add(ntoken);
                }
            }
        }

        public  void save(FileInfo savefile)
        {
            field.save(savefile);
        }

        private ITokenData generate_new_token(string name,(int X, int Y) fpos)
        {
            return name switch
            {
                "Empty"   => new TokenData(fpos),
                "Grass"   => new Grass(fpos),
                "Montain" => new Montain(fpos),
                "Water"   => new Water(fpos),
                "Hill"    => new Hill(fpos),
                "Road"    => new Road(fpos),
                "Forest"  => new Forest(fpos),
                _         => null,
            };
        }
    }
}
