﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="ExecutionPlan.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

#endregion

using System;
using System.Linq;
using Castle.Core.Internal;
using log4net;
using PVM.Core.Data.Attributes;
using PVM.Core.Data.Proxy;
using PVM.Core.Definition;
using PVM.Core.Inject;
using PVM.Core.Persistence;
using PVM.Core.Runtime.Operations.Base;

namespace PVM.Core.Runtime.Plan
{
    public class DefaultExecutionPlan : IExecutionPlan
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (DefaultExecutionPlan));
        private readonly IPersistenceProvider persistenceProvider;
        private readonly IOperationResolver operationResolver;

        public DefaultExecutionPlan(IPersistenceProvider persistenceProvider, IOperationResolver operationResolver)
        {
            this.persistenceProvider = persistenceProvider;
            this.operationResolver = operationResolver;
        }

        public void OnExecutionStarting(IExecution execution)
        {
        }

        public void OnExecutionStopped(IExecution execution)
        {
            if (!execution.CurrentNode.OutgoingTransitions.Any())
            {
                KillExecution(execution);
            }
        }

        public void OnOutgoingTransitionIsNull(IExecution execution, string transitionIdentifier)
        {
            KillExecution(execution);
        }

        public void OnExecutionResuming(IExecution execution)
        {
        }

        public void OnExecutionReachesWaitState(IExecution execution)
        {
            persistenceProvider.Persist(execution);
        }

        public void OnExecutionSignaled(IExecution execution)
        {
            execution.Resume();
        }

        public void Proceed(IExecution execution, INode node)
        {
            if (!execution.CurrentNode.OutgoingTransitions.Any())
            {
                KillExecution(execution);
                return;
            }

            var operation = operationResolver.Resolve(node.Operation);

            if (operation == null)
            {
                throw new InvalidOperationException(string.Format("'{0}' is not an operation",
                    node.Operation.FullName));
            }

            Type genericOperationInterface = operation.GetType()
                                                      .GetInterfaces()
                                                      .FirstOrDefault(
                                                          i =>
                                                              i.IsGenericType &&
                                                              i.GetGenericTypeDefinition() == typeof (IOperation<>));
            if (
                genericOperationInterface != null)
            {
                Type genericType =
                    genericOperationInterface.GetGenericArguments()
                                             .First(t => t.HasAttribute<WorkflowDataAttribute>());
                object dataContext = DataMapper.CreateProxyFor(genericType, execution.Data);

                operation.GetType().GetMethod("Execute", new[] {typeof (IExecution), genericType})
                         .Invoke(operation, new[] {execution, dataContext});
            }
            else
            {
                operation.Execute(execution);
            }

        }

        private void KillExecution(IExecution execution)
        {
            Logger.InfoFormat("Execution '{0}' ended", execution.Identifier);
            execution.Kill();

            if (execution.Parent != null && execution.Children.All(c => c.IsFinished))
            {
                KillExecution(execution.Parent);
            }
            persistenceProvider.Persist(execution);
        }
    }
}