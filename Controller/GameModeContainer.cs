using System;
using System.Collections.Generic;
using System.Text;
using Tokens;

namespace Controller
{
    public static class GameModeContainer
    {
        static public IGameMode instance;

        public static IGameMode Get()
        {
            return instance;
        }
    }
}
