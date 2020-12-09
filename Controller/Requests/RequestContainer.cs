using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;

namespace Controller.Requests
{
    public class RequestContainer
    {
        public string Name
        { get; set; }

        public RequestType Type
        { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public int Player
        { get; set; }

        public (int X, int Y) Selected
        { get; set; }

        public (int X, int Y) Target
        { get; set; }

        public List<(int X, int Y)> Targets
        { get; set; }

        public List<IActions> Actions
        { get; set; }

        public int AbilityIdx
        { get; set; }

        public RequestContainer(RequestType requestType)
        {
            Type = requestType;
        }
    }

    public enum RequestType 
    {
        CreateUnit = 0,
        MoveUnit = 1,
        UseAbility = 2,
        SelectTargest = 3,
        ApplyChanges = 4,
    }

}
