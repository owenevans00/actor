using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    // TODO: is it better to say actor.Post(message) or message.PostTo(actor)?
    // Should choose one option for publically accessible semantics unless absolutely
    // necessary...
    public interface IActorMessage
    {
        void PostTo(IActor actor);

        void PostTo(IActor actor, Action<Exception> exceptionHandler);

        void PostTo(IEnumerable<IActor> actors);

        void PostTo(IEnumerable<IActor> actors, Action<Exception> exceptionHandler);
    }
}
