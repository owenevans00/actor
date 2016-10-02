using Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapReduce
{
    /*
    * Takes messages containing word frequency dictionaries and merges
    * them into one. This implementation expects to have all the data
    * queued up so it can tell if it's done; finding more sophisticated 
    * ways to manage that is left as an exercise for the user
    */
    public class ReduceActor : ActorBase
    {
        int count = 0;
        Dictionary<string, int> wordCount;

        public int ExpectedMessages { get; internal set; }

        public ReduceActor() : base(false)
        {
            wordCount = new Dictionary<string, int>();
            AddState<Dictionary<string, int>>(Merge);
        }

        private void Merge(Message<Dictionary<string, int>> msg)
        {
            var dict = msg.State;
            foreach (var k in dict.Keys)
            {
                int val;
                if (wordCount.TryGetValue(k, out val))
                    wordCount[k] = val + dict[k];
                else
                    wordCount.Add(k, dict[k]);
            }
            count++;

            // if done, output the 20 most common words
            if (count == ExpectedMessages)
                new OutputActor(20).Post(wordCount);
        }
    }

    internal class OutputActor : ActorBase
    {
        int _topN;

        protected internal OutputActor(int topN) : base(true)
        {
            _topN = topN;
            AddState<Dictionary<string, int>>(Print);
        }

        private void Print(Message<Dictionary<string, int>> msg)
        {
            Console.WriteLine($"{_topN} most common words in corpus");
            foreach (var kv in msg.State.Select(kv => kv).OrderByDescending(kv => kv.Value).Take(_topN))
            {
                Console.WriteLine("{0}\t{1}", kv.Key, kv.Value);
            }
        }
    }
}