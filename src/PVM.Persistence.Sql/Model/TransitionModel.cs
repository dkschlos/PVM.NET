﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="TransitionModel.cs" company="PVM.NET Project Contributors">
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

using System.ComponentModel.DataAnnotations;
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class TransitionModel
    {
        [Key]
        public string Identifier { get; set; }

        public string Source { get; set; }
        public string Destination { get; set; }
        public bool Executed { get; set; }
        public bool IsDefault { get; set; }

        public static TransitionModel FromTransition(Transition transition)
        {
            return new TransitionModel
            {
                Identifier = transition.Identifier,
                Source = transition.Source.Identifier,
                Destination = transition.Destination == null ? null : transition.Destination.Identifier,
                Executed = transition.Executed,
                IsDefault = transition.IsDefault
            };
        }
    }
}