﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="SqlTaskRepository.cs" company="PVM.NET Project Contributors">
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

using NHibernate;
using PVM.Core.Tasks;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql
{
    public class SqlTaskRepository : ITaskRepository
    {
        private readonly ISessionFactory sessionFactory;

        public SqlTaskRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public void Add(UserTask userTask)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var entity = new UserTaskModel
                {
                    TaskIdentifier = userTask.TaskIdentifier,
                    ExecutionIdentifier = userTask.ExecutionIdentifier,
                    WorkflowInstanceIdentifier = userTask.WorkflowInstanceIdentifier
                };

                session.SaveOrUpdate(entity);
                session.Flush();
            }
        }

        public UserTask FindTask(string taskName, string workflowInstanceIdentifier)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var model =
                    session.QueryOver<UserTaskModel>()
                           .Where(w => w.TaskIdentifier == taskName)
                           .And(w => w.WorkflowInstanceIdentifier == workflowInstanceIdentifier)
                           .SingleOrDefault();

                if (model == null)
                {
                    return null;
                }

                return new UserTask(model.TaskIdentifier, model.ExecutionIdentifier, model.WorkflowInstanceIdentifier);
            }
        }

        public void Remove(UserTask userTask)
        {
            using (var session = sessionFactory.OpenSession())
            {
                session.Delete(FindTask(userTask.TaskIdentifier, userTask.WorkflowInstanceIdentifier));
            }
        }
    }
}