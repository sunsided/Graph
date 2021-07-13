using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using Graph.Processors;

namespace Graph.Sinks
{
    /// <summary>
    /// Base class for a data sink
    /// </summary>
    public abstract class DataSink<TData> : DataProcessor<TData>, ISink<TData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSink{TData}"/> class.
        /// </summary>
        protected DataSink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSink{Data}"/> class.
        /// </summary>
        /// <param name="registrationTimeout">Timeout in milliseconds to be used during value registration.</param>
        /// <param name="inputQueueLength">Maximum queue length for input values.</param>
        protected DataSink([DefaultValue(RegistrationTimeoutDefault)]
            int registrationTimeout, [DefaultValue(InputQueueLengthDefault)]
            int inputQueueLength)
            : base(registrationTimeout, inputQueueLength)
        {
            Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
            Contract.Requires(inputQueueLength > 0);
        }
    }
}
