﻿namespace PVM.Core.Definition.Exception
{
    public class ExecutionInactiveException : System.Exception
    {
        public ExecutionInactiveException(string message) : base(message)
        {
        }

        public ExecutionInactiveException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}