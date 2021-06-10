using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class GameNotice : FunctionBase
    {
        /// <summary>
        /// Welcome notice
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string WelcomeNotice { get; set; } = "Welcome to 7 Days to Die!";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice { get; set; } = "Hello";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice1 { get; set; } = "Hello1";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice2 { get; set; } = "Hello2";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice3 { get; set; } = "Hello3";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice4 { get; set; } = "Hello4";

        /// <summary>
        /// Alternate interval
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int AlternateInterval 
        { 
            get => _timer.Interval; 
            set 
            { 
                _timer.Interval = value; 
            }
        }

        private SubTimer _timer;

        private readonly Action<ClientInfo, RespawnType, Vector3i> _playerSpawnedInWorldHook;

        public GameNotice()
        {
            _playerSpawnedInWorldHook = new Action<ClientInfo, RespawnType, Vector3i>(PlayerSpawnedInWorld);

            _timer = new SubTimer(SendAlternateNotice, 300);
        }

        private void SendAlternateNotice()
        {
            var notices = new string[]
            {
                AlternateNotice,
                AlternateNotice1,
                AlternateNotice2,
                AlternateNotice3,
                AlternateNotice4
            }.Where(p => string.IsNullOrEmpty(p) == false).ToArray();

            string message = string.Empty;
            if (notices.Length != 0)
            {
                Random rd = new Random();
                int index = rd.Next(notices.Length);

                message = notices[index];
            }

            ModHelper.SendGlobalMessage(message);
        }

        protected override void DisableFunction()
        {
            _timer.IsEnabled = false;
            GlobalTimer.UnregisterSubTimer(_timer);
            ModEvents.PlayerSpawnedInWorld.UnregisterHandler(_playerSpawnedInWorldHook);
        }

        protected override void EnableFunction()
        {
            _timer.IsEnabled = true;
            GlobalTimer.RegisterSubTimer(_timer);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(_playerSpawnedInWorldHook);
        }

        private void PlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            switch (respawnType)
            {
                // New player spawning
                case RespawnType.EnterMultiplayer:
                // Old player spawning
                case RespawnType.JoinMultiplayer:
                    ModHelper.SendMessage(clientInfo, null, FormatCmd(WelcomeNotice, LiveDataContainer.OnlinePlayers[clientInfo.playerId]));
                    break;
            }
        }
    }
}
