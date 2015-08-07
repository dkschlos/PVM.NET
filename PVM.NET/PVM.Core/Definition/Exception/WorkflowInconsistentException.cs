﻿namespace PVM.Core.Definition.Exception
{
    public class WorkflowInconsistentException : System.Exception
    {
        public WorkflowInconsistentException(string message) : base(message)
        {
        }

        public WorkflowInconsistentException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}