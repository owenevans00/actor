using System;
using System.Collections.Generic;

namespace Actor
{
    /// <summary>
    /// The basic Actor interface. Clients pass in a message, which encapsulates some state and 
    /// optionally a reference to another actor. As with actor implementations in more functional
    /// languages the implementation needs to recognize the type of the message, and will then 
    /// act appropriately on that state. Internally the implementation uses a hashtable keyed on 
    /// type rather than language native pattern matching but the concept is similar.
    /// 
    /// The Actor pattern relies on Actor implementations being "containers for logic", and
    /// messages being "containers for data".
    /// Each message should be independent of any other messages in the system and
    /// so implementations of IActor that are not explicitly state machines should be careful not 
    /// to keep any state that is not in the message.
    /// 
    /// An actor can be either a work queue or a message-based state machine. To get data out pass 
    /// an appropriate resultHandler (either Action or Action&lt;TResult&gt;) containing a 
    /// closure that does the next step at that layer of the app. For actors called from the 
    /// UI layer the closure should probably do Dispatcher.Invoke() to marshal the returned 
    /// data back to the UI thread.
    /// </summary>
    public interface IActor
    {
        void Post<TState>(Message<TState> message);

        void Post<TState>(Message<TState> message, 
                          Action resultHandler);

        void Post<TState>(Message<TState> message, 
                          Action<Exception> exceptionHandler);
        
        void Post<TState>(Message<TState> message, 
                          Action resultHandler, 
                          Action<Exception> exceptionHandler);

        void Post<TState, TResult>(Message<TState> message);

        void Post<TState, TResult>(Message<TState> message, 
                                   Action<TResult> resultHandler);

        void Post<TState, TResult>(Message<TState> message, 
                                   Action<Exception> exceptionHandler);

        void Post<TState, TResult>(Message<TState> message, 
                                   Action<TResult> resultHandler, 
                                   Action<Exception> exceptionHandler);

        void Start();

        /// <summary>
        /// Return any pending work items for reassignment
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        //IEnumerable<Message<TState>> Stop<TState>();
    }
}