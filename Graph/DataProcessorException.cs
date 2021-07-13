using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Graph
{
    /// <summary>
    /// Exception that occurs within a <see cref="DataProcessor{TData}"/>.
    /// </summary>
    [Serializable]
    public sealed class DataProcessorException : Exception
    {
        /// <summary>
        /// The raising <see cref="DataProcessor{TData}"/>
        /// </summary>
        public object DataProcessor { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessorException"/> class.
        /// </summary>
        /// <param name="dataProcessor">The data processor.</param>
        /// <remarks></remarks>
        internal DataProcessorException(object dataProcessor)
        {
            Contract.Requires(dataProcessor is DataProcessorBase);
            DataProcessor = dataProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessorException"/> class.
        /// </summary>
        /// <param name="dataProcessor">The data processor.</param>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        internal DataProcessorException(object dataProcessor, string message)
            : base(message)
        {
            Contract.Requires(dataProcessor is DataProcessorBase);
            DataProcessor = dataProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessorException"/> class.
        /// </summary>
        /// <param name="dataProcessor">The data processor.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        /// <remarks></remarks>
        internal DataProcessorException(object dataProcessor, string message, Exception inner)
            : base(message, inner)
        {
            Contract.Requires(dataProcessor is DataProcessorBase);
            DataProcessor = dataProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        /// <remarks></remarks>
        private DataProcessorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
