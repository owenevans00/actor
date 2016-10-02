using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    /// <summary>
    /// A class to make it easier to spin up the simplest and probably
    /// most common case - an Actor that only handles one message type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SimpleActor<T>:ActorBase
    {
        public SimpleActor(Action<Message<T>> action):base(true)
        {
            AddState(action);
        }
    }
}