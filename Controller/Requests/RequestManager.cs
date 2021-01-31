using System;
using System.Collections.Generic;
using System.Text;
using Controller;
using Controller.Actions;
using System.Timers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Controller.Requests
{
    public class RequestManager
    {
        private Task<object> Request;
        DispatcherTimer timer;
        public RequestManager()
        {
            var gameMode = GameModeContainer.Get();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            if (!(gameMode is GameModeServer))
            {
                timer.Tick += (o, e) =>
                {
                    var controller = GameModeContainer.Get();
                    if (controller.State == GameModeState.AwaitResponse)
                    {
                        timer.Stop();
                        controller.GetNewGameStates().ContinueWith(t =>
                        { 
                            timer.Start(); 
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                };
            }
            timer.Start();
        }
    }

    public class RequestSender
    {
        public int Player;
        public SenderType SenderType;
    }

    public enum SenderType 
    { 
        Client = 0,
        Server = 1,
        Observer = 2,
    }

}
