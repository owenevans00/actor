using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfActor.Model
{
    /// <summary>
    /// Simple class to demonstrate that an Actor can pass some 
    /// meaningful data around
    /// </summary>
    public class UrlFetchResult
    {
        public string Url { get; set; }
        public TimeSpan Time { get; set; }
        public Exception Error { get; set; }
    }
}
