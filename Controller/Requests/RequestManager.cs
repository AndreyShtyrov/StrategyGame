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
        readonly Timer timer = new Timer();
        private bool InRequest = false;
        private Task<object> Request;
        public RequestManager(IGameMode gameMode)
        {
            timer.Interval = 2000;
            timer.Enabled = true;
            if (!(gameMode is GameModeServer))
            {
                timer.Elapsed += (o, e) =>
                {
                    var controller = GameModeContainer.Get();
                    if (controller.State == GameModeState.AwaitResponse)
                    {
                        timer.Stop();
                        Request = controller.GetNewGameStates();
                        Request.Wait();
                        var response = Request.Result;
                        Application.Current.Dispatcher(() => controller.ProcessRequset(response));
                        timer.Start();
                    }
                };
            }
        }
        public bool IsNeedNewRequest()
        {
            if (!InRequest)
            {
                InRequest = true;
                return true;
            }
            else
            {
                InRequest = false;
                return false;
            }
            
        }
        public void  CloseReuqest()
        {
            InRequest = false;
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
