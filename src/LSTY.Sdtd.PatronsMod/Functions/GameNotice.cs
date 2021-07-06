using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Linq;
using LSTY.Sdtd.PatronsMod.Internal;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class GameNotice : FunctionBase
    {
        /// <summary>
        /// Welcome notice
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string WelcomeNotice { get; set; } = "[00FF00]Welcome to 7 Days to Die!";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice { get; set; } = "[00FF00]Hello, this is a alternate notice1";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice1 { get; set; } = "[00FF00]Hello, this is a alternate notice2";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice2 { get; set; } = "[00FF00]Hello, this is a alternate notice3";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice3 { get; set; } = "[00FF00]Hello, this is a alternate notice4";

        [ConfigNode(XmlNodeType.Attribute)]
        public string AlternateNotice4 { get; set; } = "[00FF00]Hello, this is a alternate notice5";

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
            try
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
            catch (Exception ex)
            {
                CustomLogger.Warn(ex, "Error in GameNotice.PlayerSpawnedInWorld");
            }
        }
    }
}
