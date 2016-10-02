using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor;
using System.Threading;
using System.Text.RegularExpressions;

namespace MapReduce
{
    class Program
    {

        /*
         * Map/Reduce with actors: this example calculates the frequency of words in
         * the "Friends, Romans, Countrymen" speech from Julius Caesar.
         * 
         * the corpus is mapped across 4 actors, each of which produce a 
         * dictionary of <word,frequency>, and those results are then reduced down to
         * a single combined dictionary. That dictionary is then formwarded to another
         * for output.
         */
        static void Main(string[] args)
        {
            // this is the actual mapping operation, which is fed into a SimpleActor
            // instead of building an entire subclass. Note that this is only possible
            // to do when the actor is stateless 
            var mapFunc = new Action<Message<string>>(msg =>
            {
                var processed = msg.State
                    .Split(' ')
                    .Select(s => Regex.Replace(s, "\\W", "").ToLower())
                    .GroupBy(s => s)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );
                msg.ReplyTo.Post(processed);
            });

            var mapperList = new IActor[] {
                new SimpleActor<string>(mapFunc),
                new SimpleActor<string>(mapFunc),
                new SimpleActor<string>(mapFunc),
                new SimpleActor<string>(mapFunc)
            };

            var reduce = new ReduceActor();

            using (var corpus = new StreamReader(@".\corpus.txt"))
            {
                int idx = 0;

                while (!corpus.EndOfStream)
                {
                    var mapperId = idx % mapperList.Length;
                    mapperList[mapperId].Post(
                        new Message<string>(corpus.ReadLine(), reduce)
                    );
                    idx++;
                }

                reduce.ExpectedMessages = idx;
                reduce.Start();
            }

            Console.ReadKey();
        }
    }
    

}
