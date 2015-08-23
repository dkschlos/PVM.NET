﻿using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public interface IOperation<in TDataMappingDefinition> : IOperation
    {
        void Execute(IExecution execution, TDataMappingDefinition dataContext);
    }

    public interface IOperation
    {
        void Execute(IExecution execution);
    }
}