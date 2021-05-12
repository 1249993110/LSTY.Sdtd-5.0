using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class CommonConfig : IFunction
    {
        public bool IsEnabled { get; set; } = true;

        public string FunctionName { get; } = nameof(CommonConfig);


        [ConfigNode(XmlNodeType.Element)]
        public WebConfig WebConfig { get; set; } = new WebConfig();

        /// <summary>
        /// Message sender name.
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string ServerName { get; set; } = "[FF0000]Server[FFFFFF]";

        [ConfigNode(XmlNodeType.Attribute)]
        public string ChatCommandPrefix { get; set; } = "/";

        [ConfigNode(XmlNodeType.Attribute)]
        public string HandleChatMessageErrorTips { get; set; } = "[FF0000]An error occurred, please contact the server administrator";

        [ConfigNode(XmlNodeType.Attribute)]
        public int ChatCommandCacheMaxCount { get; set; } = 100;


    }
}
