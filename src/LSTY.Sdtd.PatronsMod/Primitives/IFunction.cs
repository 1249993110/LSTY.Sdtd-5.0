namespace LSTY.Sdtd.PatronsMod.Primitives
{
    interface IFunction
    {
        /// <summary>
        /// Function is or no enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Function name
        /// </summary>
        string FunctionName { get; }
    }
}
