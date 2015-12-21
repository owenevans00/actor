using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor;

namespace ActorDemo
{
    class PingActor : Actor.ActorBase
    {
        public bool Quiet { get; set; }
        public PingActor(bool startImmediately = false)
            : base(startImmediately)
        {
            Quiet = true;
            Init();
        }

        private void Init()
        {
            AddState(new Func<Message<long>, string>(PingCountdown));
            AddState(new Action<Message<string>>(_ => Console.WriteLine("PING stop")));
        }

        private string PingCountdown(Message<long> message)
        {
            Console.WriteLine("-> PING {0}", message.State);

            if (message.State > 0)
            {
                // reply to PongActor
                // State management here - request the recipient of the message to post back on this actor
                message.ReplyTo.Post(
                    new Message<long>(message.State - 1, this),
                    () => { Ponged(message.State - 1); },
                    ExceptionActor.ExceptionHandler
                    );
            }
            else
            {
                message.ReplyTo.Post(new Message<string>("stop", this));
            }


            return Quiet ? null : string.Format("PING RESULT {0}", message.State);
        }

        private void Ponged(long pong)
        {
            if (Quiet) return;
            Console.WriteLine("Call to PONG {0} finished", pong);
        }
    }
}
