using IceCoffee.Common.Xml;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Primitives
{
    abstract class SubFunctionBase
    {
        private bool _isEnabled;

        /// <summary>
        /// Function is or no enabled.
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public bool IsEnabled
        {
            get
            {
                if (parent.IsEnabled == false)
                {
                    return false;
                }

                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }

        protected IFunction parent;

        public SubFunctionBase(IFunction parent)
        {
            this.parent = parent;
        }
    }
}
