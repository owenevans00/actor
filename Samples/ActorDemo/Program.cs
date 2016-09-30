using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor;

namespace ActorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var ping = new PingActor(true);
            // If you don't handle the return value of the state function
            // that's fine, but you still need to specify the type. Otherwise 
            // ActorBase won't find it and there'll be a runtime error.
            // Note the implication that having states be Actions not Funcs is a
            // nicer implementation/less leaky abstraction
            ping.Post<long, string>(
                new Message<long>(10, new PongActor(true))
                );
            
            ping.Start();
            
            Console.ReadKey();
            
            
            Console.WriteLine();

            // do it twice to demo that a started actor remains active
            // even if all the Tasks it contains are finished
            ping.Quiet = false; // enable verbose output
            ping.Post(  // this time we do respond to the return value of the
                // func so type args don't have to be specifed 
               new Message<long>(10, new PongActor(true)),
                // But explicit casts of method groups to the expected
                // action are still needed - another reason to implment 
                // Actors with Actions not Funcs
               (Action<string>)ResultWrapper<string>.ResultHandler);

            Console.ReadKey();
            
        }
    }
}
