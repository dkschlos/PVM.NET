using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;
using System.Collections.Generic;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan
    {
        void Proceed(IInternalExecution execution, IOperation operation);
        void Start(INode startNode, IDictionary<string, object> data);
        void OnExecutionStarting(Execution execution);
        void OnExecutionStopped(Execution execution);
        void OnOutgoingTransitionIsNull(Execution execution, string transitionIdentifier);
        bool IsFinished { get; }
        void OnExecutionResuming(Execution execution);
    }
}