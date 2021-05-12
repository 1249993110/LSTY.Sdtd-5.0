using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.LiveData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Primitives
{
    abstract class FunctionBase : IFunction
    {
        private string _functionTag;

        private bool _isEnabled;

        // Current function state
        private bool _isRunning;

        /// <summary>
        /// Function is or no enabled.
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (value)
                {
                    PrivateEnableFunction();
                }
                else
                {
                    PrivateDisableFunction();
                }
            }
        }

        /// <summary>
        /// Function tag.
        /// </summary>
        public string FunctionName => _functionTag;

        private readonly ChatHook _onPlayerChatHooked;

        private static readonly IChatLogRepository _chatLogRepository;
        private static readonly List<ChatHook> _chatHooks;

        static FunctionBase()
        {
            _chatLogRepository = IocContainer.Resolve<IChatLogRepository>();

            _chatHooks = new List<ChatHook>();
            ModEvents.ChatMessage.RegisterHandler(ChatMessage);
        }

        public FunctionBase()
        {
            _functionTag = this.GetType().Name;

            _onPlayerChatHooked = new ChatHook(this.OnPlayerChatHooked);

            LiveDataContainer.ServerNonePlayer += PrivateDisableFunction;
            LiveDataContainer.ServerHavePlayerAgain += PrivateEnableFunction;
        }

        /// <summary>
        /// Prevent duplicate settings 
        /// </summary>
        private void PrivateDisableFunction()
        {
            // If the function is not disabled 
            if (_isRunning)
            {
                _isRunning = false;
                DisableFunction();
            }
        }

        /// <summary>
        /// Prevent duplicate settings 
        /// </summary>
        private void PrivateEnableFunction()
        {
            // If the function is disabled
            if (_isRunning == false && _isEnabled && LiveDataContainer.OnlinePlayers.Count > 0)
            {
                _isRunning = true;
                EnableFunction();
            }
        }

        /// <summary>
        /// Disabled function, the default implementation ChatHook of the base class
        /// </summary>
        protected virtual void DisableFunction()
        {
            _chatHooks.Remove(_onPlayerChatHooked);
        }

        /// <summary>
        /// Enabled function, the default implementation ChatHook of the base class
        /// </summary>
        protected virtual void EnableFunction()
        {
            if (_chatHooks.Contains(_onPlayerChatHooked) == false)
            {
                _chatHooks.Add(_onPlayerChatHooked);
            }
        }

        private static bool ChatMessage(ClientInfo clientInfo, EChatType eChatType, int senderId, string message, string mainName,
            bool localizeMain, List<int> recipientEntityIds)
        {
            try
            {
                if (message.StartsWith(FunctionManager.CommonConfig.ChatCommandPrefix) == false)
                {
                    return true;
                }

                if (clientInfo == null || senderId == -1 || eChatType != EChatType.Global)
                {
                    return true;
                }

                string _message = message.Remove(0, FunctionManager.CommonConfig.ChatCommandPrefix.Length);
                
                ChatHook chatHook = ChatCommandCache.Get(_message);

                // If key in the command cache 
                if (chatHook != null)
                {
                    // If function is enabled
                    if ((chatHook.Target as IFunction).IsEnabled)
                    {
                        bool isHandled = HandleChatMessage(chatHook, clientInfo, _message);

                        if (isHandled)
                        {
                            return false;
                        }
                        else
                        {
                            ChatCommandCache.Remove(_message);
                        }
                    }
                }

                // If the command is not handled
                foreach (var hook in _chatHooks)
                {
                    if(hook == chatHook)
                    {
                        continue;
                    }

                    bool isHandled = HandleChatMessage(hook, clientInfo, _message);
                    // If the command is accepted by other function
                    if (isHandled)
                    {
                        ChatCommandCache.Set(_message, hook);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ChatMessage");
                return true;
            }
            finally
            {
                string steamId = clientInfo == null ? "-non-player-" : clientInfo.playerId;
                
                var chatLog = new T_ChatLog()
                {
                    SteamId = steamId,
                    EntityId = senderId,
                    ChatType = (int)eChatType,
                    Message = message
                };

                _chatLogRepository.Insert(chatLog);
            }
        }

        /// <summary>
        /// Call when capturing player chat message, return true mean this message were handled by current function
        /// </summary>
        protected virtual bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            return false;
        }

        private static bool HandleChatMessage(ChatHook chatHook, ClientInfo clientInfo, string message)
        {
            try
            {
                return chatHook.Invoke(LiveDataContainer.OnlinePlayers[clientInfo.playerId], message);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in chathook");
                ModHelper.SendMessage(clientInfo, null, FunctionManager.CommonConfig.HandleChatMessageErrorTips);

                return false;
            }
        }


        /// <summary>
        /// Available variables, the base class default add {playerName} {steamId}、{entityId}
        /// </summary>
        public IReadOnlyList<string> AvailableVariables => availableVariables;
        /// <summary>
        /// Available variables, the base class default add {playerName} {steamId}、{entityId}
        /// </summary>
        protected readonly List<string> availableVariables = new List<string>()
        {
            "{playerName}",
            "{steamId}",
            "{entityId}"
        };

        protected virtual string FormatCmd(OnlinePlayer player, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            if (player == null)
            {
                return message;
            }

            StringBuilder builder = new StringBuilder(message);

            return builder
                .Replace("{playerName}", player.Name)
                .Replace("{steamId}", player.SteamId)
                .Replace("{entityId}", player.EntityId.ToString()).ToString();
        }
    }
}
