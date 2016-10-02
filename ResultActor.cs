using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    /// <summary>
    /// ResultActor takes a message and simply writes the payload to the console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultActor<T> : ActorBase
    {
        public ResultActor(bool startImmediately = true)
            : base(startImmediately)
        {
            Init();
        }

        private void Init()
        {
            AddState(
                new Action<Message<T>>(t =>
                    {
                        if (t.State != null)
                            Console.WriteLine(t.State);
                    }
                ));
        }
    }

    // Use a static wrapper around ResultActor to make sure that all the calls to
    // it are ordered instead of creating a new one every time - if that happens
    // all the tasks aren't guaranteed to be sequenced (and proabably won't be)
    // Note that for each T a separate instance of the class will be created, (see
    // http://stackoverflow.com/questions/2936580/c-sharp-generic-static-constructor and 
    // the section on static constructors in the C# spec) which obviously means that you
    // *still* can't really guarantee result synchronization if you use another actor
    // to handle results from state functions.
    // Also, this message processing chain is independent from whatever message processing
    // chain(s) sent the message, so although they do proceed in order they do not proceed
    // in lockstep
    // "Yo, I heard you like actors, so I put an actor in your actor so you can act while you're acting"
    public static class ResultWrapper<T>
    {
        private static ResultActor<T> actor = new ResultActor<T>();

        public static void ResultHandler<TResult>(TResult result) where TResult : T
        {
            actor.Post(new Message<TResult>(result, null));
        }
    }
}