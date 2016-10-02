using System;
using System.Collections.Generic;

namespace Actor
{
    public class Message<TState>:IActorMessage
    {
        public TState State { get; private set; }

        public IActor ReplyTo { get; private set; }

        public Message(TState state, IActor replyTo)
        {
            State = state;
            ReplyTo = replyTo;
        }

        public void PostTo(IActor actor)
        {
            actor.Post(this);
        }
        
        public void PostTo(IActor actor, Action<Exception> exceptionHandler)
        {
            actor.Post(this, exceptionHandler);
        }

        public void PostTo(IEnumerable<IActor> actors)
        {
            foreach (var actor in actors) actor.Post(this);
        }

        public void PostTo(IEnumerable<IActor> actors, Action<Exception> exceptionHandler)
        {
            foreach (var actor in actors) actor.Post(this, exceptionHandler);
        }
    }
}
