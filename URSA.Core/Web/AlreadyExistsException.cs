﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace URSA.Web
{
    /// <summary>Describes an exception when a given entity does not exist.</summary>
    [ExcludeFromCodeCoverage]
    [SuppressMessage("Microsoft.Design", "CA0000:ExcludeFromCodeCoverage", Justification = "No testable logic.")]
    [Serializable]
    public class AlreadyExistsException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="AlreadyExistsException"/> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        public AlreadyExistsException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AlreadyExistsException"/> class.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AlreadyExistsException"/> class.</summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        public AlreadyExistsException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}