using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor;

namespace ActorDemo
{
    class PongActor : ActorBase, IDisposable
    {
        public PongActor(bool startImmediately = false)
            : base(startImmediately)
        {
            Init();
        }

        private void Init()
        {
            AddState(new Action<Message<long>>(PongCountdown));
            AddState(new Action<Message<string>>(_ => Console.WriteLine("PONG stop")));
        }

        private void PongCountdown(Message<long> message)
        {
            //if (message.State < 2)
            //    // THIS WILL ALWAYS THROW IN THE DEBUGGER BECAUSE IT'S
            //    // NOT HANDLED WITHIN THE TASK. 
            //    throw new InvalidOperationException("too small");

            Console.WriteLine("<- PONG {0}", message.State);

            if (message.State > 0)
            {
                // Reply to PingActor - specify return type of func inside PingActor
                message.ReplyTo.Post<long, string>(
                    new Message<long>(message.State - 1, this),
                    ResultWrapper<string>.ResultHandler,
                    ExceptionActor.ExceptionHandler
                    );
            }
            else
            {
                message.ReplyTo.Post(new Message<string>("stop", new PingActor(true)));
            }
        }

        public void Dispose()
        {
            Console.WriteLine("PongActor disposed");
        }
    }
}