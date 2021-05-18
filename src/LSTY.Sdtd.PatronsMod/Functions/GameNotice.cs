using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Threading;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class GameNotice : FunctionBase
    {
        /// <summary>
        /// Welcome notice
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string WelcomeNotice { get; set; } = "Welcome to 7 Days to Die!";

        /// <summary>
        /// Alternate notice
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice { get; set; } = "Hello";

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

        private readonly Action<ClientInfo, RespawnType, Vector3i> _action;

        public GameNotice()
        {
            _action = new Action<ClientInfo, RespawnType, Vector3i>(PlayerSpawnedInWorld);

            _timer = new SubTimer(SendAlternateNotice, 300);
            GlobalTimer.RegisterSubTimer(_timer);
        }

        private void SendAlternateNotice()
        {
            ModHelper.SendGlobalMessage(AlternateNotice);
        }

        protected override void DisableFunction()
        {
            _timer.IsEnabled = false;
            ModEvents.PlayerSpawnedInWorld.UnregisterHandler(_action);
        }

        protected override void EnableFunction()
        {
            _timer.IsEnabled = true;
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(_action);
        }

        private void PlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            switch (respawnType)
            {
                // New player spawning
                case RespawnType.EnterMultiplayer:
                // Old player spawning
                case RespawnType.JoinMultiplayer:
                    ModHelper.SendMessage(clientInfo, null, FormatCmd(LiveDataContainer.OnlinePlayers[clientInfo.playerId], WelcomeNotice));
                    break;
            }
        }
    }
}
