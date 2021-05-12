using IceCoffee.Common.Extensions;
using PostSharp.Aspects;
using System;

namespace LSTY.Sdtd.PatronsMod.ExceptionCatch
{
    /// <summary>
    /// Catch exception by aspect
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CatchExceptionAttribute : OnMethodBoundaryAspect
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Throw new PatronsModException or not
        /// </summary>
        public bool ThrowNew { get; set; }

        /// <summary>
        /// Construct CatchExceptionAttribute
        /// </summary>
        /// <param name="errorMessage"></param>
        public CatchExceptionAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Construct CatchExceptionAttribute
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="throwNew"></param>
        public CatchExceptionAttribute(string errorMessage, bool throwNew)
        {
            ErrorMessage = errorMessage;
            ThrowNew = throwNew;
        }

        /// <inheritdoc />
        public override void OnException(MethodExecutionArgs args)
        {
            if (ThrowNew)
            {
                throw new PatronsModException(ErrorMessage, args.Exception);
            }
            else
            {
                CustomLogger.Error(args.Exception, ErrorMessage);
#if DEBUG
                foreach (var item in args.Arguments)
                {
                    string message = null;
                    try
                    {
                        message = item.ToJson();
                    }
                    catch
                    {
                        message = "Argument serialize failed: " + item.GetType().Name;
                    }

                    CustomLogger.Error(message);
                }
#endif
            }
        }

#if DEBUG
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
            CustomLogger.Warn("***** Entry: " + args.Method.Name + " *****");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);
            CustomLogger.Warn("***** Exit: " + args.Method.Name + " *****");
        }
#endif
    }
}