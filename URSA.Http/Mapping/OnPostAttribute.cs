﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace URSA.Web.Http.Mapping
{
    /// <summary>Instructs the pipeline to map the method to a POST HTTP verb.</summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "No testable logic.")]
    [AttributeUsage(AttributeTargets.Method)]
    public class OnPostAttribute : OnVerbAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="OnPostAttribute" /> class.</summary>
        public OnPostAttribute() : base(Verb.POST)
        {
        }
    }
}