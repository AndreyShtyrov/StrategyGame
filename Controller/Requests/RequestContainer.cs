using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;
using Newtonsoft.Json;

namespace Controller.Requests
{
    public class RequestContainer
    {
        public string Name
        { get; set; }

        public RequestSender RequestSender;

        public RequestType Type
        { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public int CurrentActionIndex
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

        [JsonConstructor]
        public RequestContainer()
        { }

        public RequestContainer(RequestType requestType)
        {
            Type = requestType;
            this.RequestSender = GameModeContainer.Get().RequestSender;
        }

    }

    public enum RequestType 
    {
        CreateUnit = 0,
        MoveUnit = 1,
        UseAbility = 2,
        SelectTargest = 3,
        ApplyChanges = 4,
        GetNewStates = 5,
    }

}
