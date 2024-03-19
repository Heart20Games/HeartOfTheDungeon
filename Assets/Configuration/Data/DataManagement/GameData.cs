using System;
using System.Collections.Generic;

namespace DataManagement
{
    [Serializable]
    public class GameData
    {
        public int lastId = -1;
        public List<StatBlockData> statBlocks = new();
        public List<CharacterData> characterBlocks = new();
        public SessionData session;

        public void Initialize()
        {
            statBlocks ??= new();
            characterBlocks ??= new();
            session ??= new("Session");
        }
    }
}