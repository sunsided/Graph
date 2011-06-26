using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Graph
{
	/// <summary>
	/// Exception, die innerhalb eines <see cref="DataProcessor{TData}"/> auftrat.
	/// </summary>
	[Serializable]
	public sealed class DataProcessorException : Exception
	{
		/// <summary>
		/// Der verursachende <see cref="DataProcessor{TData}"/>
		/// </summary>
		public object DataProcessor { get; private set; }

		internal DataProcessorException(object dataProcessor)
		{
			Contract.Requires(dataProcessor is DataProcessor<>)
			DataProcessor = dataProcessor;
		}

		internal DataProcessorException(object dataProcessor, string message)
			: base(message)
		{
			DataProcessor = dataProcessor;
		}

		internal DataProcessorException(object dataProcessor, string message, Exception inner)
			: base(message, inner)
		{
			DataProcessor = dataProcessor;
		}

		protected DataProcessorException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}
