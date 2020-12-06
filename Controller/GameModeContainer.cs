using System;
using System.Collections.Generic;
using System.Text;
using Tokens;

namespace Controller
{
    public static class GameModeContainer
    {
        static private IGameMode instance;

        public static IGameMode Get(bool IsServer, Field field)
        {
            if (IsServer)
            {
                instance = new GameModeServer(field);
            }
            else
            {
                instance = new GameMode(field);
            }
            return instance;
        }

        public static IGameMode Get()
        {
            return instance;
        }
        
    }
}
