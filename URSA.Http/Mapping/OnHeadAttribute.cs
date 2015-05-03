﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace URSA.Web.Http.Mapping
{
    /// <summary>Instructs the pipeline to map the method to a HEAD HTTP verb.</summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method)]
    public class OnHeadAttribute : OnVerbAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="OnHeadAttribute" /> class.</summary>
        public OnHeadAttribute() : base(Verb.HEAD)
        {
        }
    }
}