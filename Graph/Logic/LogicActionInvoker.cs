using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph.Logic
{
    /// <summary>
    /// Sink that receives a boolean value and invokes an action.
    /// </summary>
    public sealed class LogicActionInvoker : ActionInvoker<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicActionInvoker"/> class.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        public LogicActionInvoker(Action<bool> action)
            : base(action)
        {
            Contract.Requires(action != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="scheduler">The scheduler to use.</param>
        public LogicActionInvoker(Action<bool> action, TaskScheduler scheduler)
            : base(action, scheduler)
        {
            Contract.Requires(action != null);
            Contract.Requires(scheduler != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="scheduler">The scheduler to use.</param>
        /// <param name="taskCreationOptions">The <see cref="TaskCreationOptions"/> to use.</param>
        public LogicActionInvoker(Action<bool> action, TaskScheduler scheduler, TaskCreationOptions taskCreationOptions)
            : base(action, scheduler, taskCreationOptions)
        {
            Contract.Requires(action != null);
            Contract.Requires(scheduler != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="taskCreationOptions">The <see cref="TaskCreationOptions"/> to use.</param>
        public LogicActionInvoker(Action<bool> action, TaskCreationOptions taskCreationOptions)
            : base(action, taskCreationOptions)
        {
            Contract.Requires(action != null);
        }
    }
}
