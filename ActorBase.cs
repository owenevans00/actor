using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actor
{
    /// <summary>
    /// Implements the actual queueing and execution logic around running an actor. The canonical
    /// actor pattern uses a queue of work items that are processed asynchronously. In this 
    /// implementation, we use Tasks to manage asynchronous processing and sequencing - each
    /// item posted to the queue simply becomes another task which is attached to the sequence.
    /// If something needs to be done with the output of a work item, a closure is passed in along
    /// with the message, and this is also attached to the task generated from the message.
    /// Delayed execution of the queue is the default - call Start() to trigger the queue. For 
    /// immediate execution, call the constructor with startImmediately set to true, and the 
    /// first item posted will be triggered as soon as it's set.
    /// </summary>
    public abstract class ActorBase : IActor
    {
        /// <summary>
        /// This is where the actions the Actor can take are defined - the actor only 
        /// responds to messages that are of types listed in this hashtable. If the type 
        /// cannot be found at runtime an exception will be thrown.
        ///
        /// State logic items are stored in the dictionary as Delegates - this allows the 
        /// same container to keep any variety of Action or Func.
        /// </summary>
        protected Dictionary<Type, Delegate> _states = new Dictionary<Type, Delegate>();

        private bool _startImmediately;
        private Task _headtask; // the task we start first
        private Task _tailTask; // the task we append new work items onto

        protected ActorBase(bool startImmediately = true)
        {
            _startImmediately = startImmediately;
        }

        #region Post() methods
        public void Post<TState>(TState state)
        {
            Post(new Message<TState>(state, null));
        }

        public void Post<TState>(Message<TState> message)
        {
            if (_headtask == null)
            {
                _headtask = _tailTask = new Task(() => Exec(message));
                if (_startImmediately) Start();
            }
            else
            {
                _tailTask = _tailTask.ContinueWith(_ => Exec(message));
            }
        }

        public void Post<TState>(Message<TState> message, Action resultHandler)
        {
            Post(message);
            _tailTask.ContinueWith(_ => resultHandler(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Post<TState>(Message<TState> message, Action<Exception> exceptionHandler)
        {
            Post(message);
            _tailTask.ContinueWith(t => exceptionHandler(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Post<TState>(Message<TState> message, Action resultHandler, Action<Exception> exceptionHandler)
        {
            Post(message, exceptionHandler);
            if(resultHandler != null)
                _tailTask.ContinueWith((Action<Task>)(_ => resultHandler()), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Post<TState, TResult>(Message<TState> message)
        {
            Post<TState, TResult>(message, null, null);
        }

        public void Post<TState, TResult>(Message<TState> message, Action<TResult> resultHandler)
        {
            Post(message, resultHandler, null);
        }
        
        public void Post<TState, TResult>(Message<TState> message, Action<Exception> exceptionHandler)
        {
            Post(message, null, exceptionHandler);
        }

        public void Post<TState, TResult>(Message<TState> message, Action<TResult> resultHandler, Action<Exception> exceptionHandler)
        {
            // because _tailTask is just Task and not Task<T> we need to wrap Task<T> in another
            // Task to match the object type. Hence the implementation of the overloads of Post<T,R> 
            // are backwards compared to Post<T> (really I should probably redo Post<T> to work this way too)
            var newTask = new Task<TResult>(() => ExecFunc<TState, TResult>(message));
            if (_headtask == null)
            {
                _headtask = _tailTask = new Task(newTask.Start);
            }
            else
            {
                _tailTask = _tailTask.ContinueWith(_ => newTask.Start());
            }
            // Someone might not care about the return value of the function and be relying on some other side effect
            if (resultHandler != null)
                newTask.ContinueWith(t => resultHandler(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

            // Capture exception if wanted
            if (exceptionHandler != null)
                newTask.ContinueWith(t => exceptionHandler(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

            // And get going
            if (_startImmediately) Start();
        }
        #endregion

        public void Start()
        {
            if (_headtask != null && _headtask.Status == TaskStatus.Created)
            {
                // all tasks attached to this will start in sequence (although ordering's not 
                // guaranteed between branches)
                try
                {
                    _headtask.Start();
                }
                catch (InvalidOperationException) // in rare cases there is a race condition where
                { }                               // the task is already running
            }
            else
            {
                _startImmediately = true; // set flag so we do start as soon as a message arrives
            }
        }

        // ActorBase used to have an abstract InitStates() where derived 
        // classes had to manipulate _states directly but having methods on 
        // the base class seemed to be a better way to do it. Also, you can't
        // force it to be called, so there's no point trying to pretend
        // that there is an actual contract.
        protected void AddState<T>(Action<Message<T>> action)
        {
            var t = typeof(Message<T>);
            if (_states.ContainsKey(t))
                _states[t] = action;
            else
                _states.Add(t, action);
        }

        protected void AddState<T, R>(Func<Message<T>, R> func)
        {
            var t = typeof(Message<T>);
            if (_states.ContainsKey(t))
                _states[t] = func;
            else
                _states.Add(t, func);
        }

        protected void RemoveState(Type t)
        {
            if (_states.ContainsKey(t))
                _states.Remove(t);
        }

        // There are two flavors of Exec - one for Action<foo> and one for Func<foo,bar>
        // This allows use to do sensible casting to the actual delegate type instead of
        // resorting to delegate.dynamicinvoke(message), which also removes a layer of 
        // TaskInvocationException in case of exceptions.
        // A "pure" Actor implementation seems to just use Action, and it's a bit
        // easier to program in this model if we use Actions and Messages in preference
        // to Funcs and callbacks
        
        protected R GetDelegate<T, R>() where R : class
        {
            Delegate action;
            if (!_states.TryGetValue(typeof(T), out action))
                throw new InvalidOperationException("Unrecognized message type - " + typeof(T).ToString());

            var rval = action as R;
            if (rval != null) return rval;

            throw new InvalidOperationException("State function does not match expected signature");
        }
     
        protected virtual void Exec<TState>(Message<TState> message)
        {
            // Indirection here means we could pass message to a delegate running
            // somewhere else with a proxy to message.ReplyTo, thus enabling
            // distributed processing at little additional cost to users of this class :-)
            var action = GetDelegate<Message<TState>, Action<Message<TState>>>();
            action(message);
        }

        protected virtual TResult ExecFunc<TState, TResult>(Message<TState> message)
        {
            var func = GetDelegate<Message<TState>, Func<Message<TState>, TResult>>();
            return func(message);
        }
    }
}
