using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph
{
    /// <summary>
    /// Sink that invokes an action
    /// </summary>
    public class ActionInvoker<TData> : DataSink<TData>
    {
        /// <summary>
        /// The action to invoke
        /// </summary>
        private readonly Action<TData> _action;

        /// <summary>
        /// The task scheduler to be used
        /// </summary>
        private readonly TaskScheduler _scheduler;

        /// <summary>
        /// The task creation options for the scheduler
        /// </summary>
        /// <seealso cref="_scheduler"/>
        private readonly TaskCreationOptions _creationOptions = TaskCreationOptions.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public ActionInvoker(Action<TData> action)
        {
            Contract.Requires(action != null);
            _action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="scheduler">The scheduler to use</param>
        public ActionInvoker(Action<TData> action, TaskScheduler scheduler)
            : this(action)
        {
            Contract.Requires(action != null);
            Contract.Requires(scheduler != null);
            _scheduler = scheduler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="scheduler">The scheduler to use</param>
        /// <param name="taskCreationOptions">The task creation options to be used</param>
        public ActionInvoker(Action<TData> action, TaskScheduler scheduler, TaskCreationOptions taskCreationOptions)
            : this(action, scheduler)
        {
            Contract.Requires(action != null);
            Contract.Requires(scheduler != null);
            _scheduler = scheduler;
            _creationOptions = taskCreationOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="taskCreationOptions">The task creation options to be used</param>
        public ActionInvoker(Action<TData> action, TaskCreationOptions taskCreationOptions)
            : this(action)
        {
            Contract.Requires(action != null);
            _creationOptions = taskCreationOptions;
        }

        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="payload">The data to process</param>
        protected override void ProcessData(TData payload)
        {
            Task task = new Task(() => _action(payload), _creationOptions);
            if (_scheduler == null) task.Start(); else task.Start(_scheduler);
        }
    }
}
