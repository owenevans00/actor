using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    /// <summary>
    /// Demonstration class to show how to trap exceptions in the Actor pattern - 
    /// it's basically the same as any other actor-based operation
    /// </summary>
    public class ExceptionActor : ActorBase
    {
        public ExceptionActor(bool startImmediately = true)
            : base(startImmediately)
        {
            Init();
        }

        private void Init()
        {
            AddState(new Action<Message<Exception>>(ShowException));
        }

        private void ShowException(Message<Exception> ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (ex.State is AggregateException)
            {
                var newEx = (ex.State as AggregateException).Flatten();
                foreach (var exc in newEx.InnerExceptions)
                {
                    Console.WriteLine(exc.AllMessages());
                }
            }
            else
            {
                Console.WriteLine(ex.State.AllMessages());
            }
            Console.ForegroundColor = color;
        }

        public static void ExceptionHandler(Exception exception)
        {
            var exc = new ExceptionActor();
            exc.Post(new Message<Exception>(exception, null));
        }
    }
}
