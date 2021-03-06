﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="DataMapper.cs" company="PVM.NET Project Contributors">
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using PVM.Core.Data.Attributes;

namespace PVM.Core.Data.Proxy
{
    public class DataMapper
    {
        public static object CreateProxyFor(Type type, IDictionary<string, object> data)
        {
            var generator = new ProxyGenerator();
            return generator.CreateClassProxyWithTarget(type, Activator.CreateInstance(type), new DataInterceptor(data));
        }

        public static IDictionary<string, object> ExtractData([CanBeNull] object data)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            if (data == null)
            {
                return result;
            }

            var type = data.GetType();

            if (type.HasAttribute<WorkflowDataAttribute>())
            {
                foreach (
                    PropertyInfo property in
                        type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                            p => p.GetCustomAttributes<OutAttribute>(true).Any()))
                {
                    string name = property.GetOutMappingName();

                    if (property.GetGetMethod() == null)
                    {
                        throw new DataMappingNotSatisfiedException(
                            string.Format("Property '{0}' of '{1}' does not have a public getter", name,
                                data.GetType().FullName));
                    }
                    result.Add(name, property.GetValue(data));
                }
            }


            return result;
        }
    }
}