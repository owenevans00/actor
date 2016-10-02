using Actor;
using System;

namespace HelloWorld
{
    /*
     * The classic "Hello, World" program written using actors. One actor creates
     * a message, and then another echoes it to the console.
     */
    class Program
    {
        static void Main(string[] args)
        {
            // EmptyActor is a minimal implementation with no actual state logic
            var actor = new EmptyActor();
            
            // Inject some instructions. The lambda is of type Action<Message<string>>
            actor.AddState<string>(m => m.PostTo(new ResultActor<string>()));

            // alternatively (this one overwrites the previous!)
            actor.AddState<string>(m => new ResultActor<string>().Post(m));
            
            // send a message - the ResultActor above will write it out
            actor.Post("Hello, World");

            // wait for all the tasks etc to complete
            Console.ReadKey();
        }
    }
}