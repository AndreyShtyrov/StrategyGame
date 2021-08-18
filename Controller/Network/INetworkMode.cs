using Controller.Actions;
using Controller.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Network
{
    public interface INetworkMode
    {
        public List<IActions> Response
        { get; set; }

        public object ProcessRequset(object sender);

        public void RequestUserInput(RequestContainer container);

        public void SendUserResponse(UnitPresset unit, (int X, int Y) targetPosition);

    }
}
