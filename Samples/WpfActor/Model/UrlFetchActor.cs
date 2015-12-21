using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Actor;
using System.Diagnostics;

namespace WpfActor.Model
{
    /// <summary>
    /// Receives a message with a url in it and attempts to download it. If successful
    /// it posts back the time it took to download it; if not, it posts the error message
    /// </summary>
    public class UrlFetchActor : ActorBase
    {
        public UrlFetchActor() { Init(); }

        private void Init()
        {
            // This actor only does one thing, so there's just one state
            _states.Add(typeof(Message<Uri>), (Action<Message<Uri>>)FetchUrl);
        }

        private void FetchUrl(Message<Uri> message)
        {
            // Build the response message - begin with the actual url we're going after
            var responseTime = new UrlFetchResult { Url = message.State.AbsoluteUri };

            // Next, see how long it takes to grab the requested resource
            // put the stopwatch outside so if the request fails we can still
            // track how much time we spent processing exceptions (assuming the 
            // downstream actor supports that, which of course wo don't actually know
            // (for the record for an invalid FQDN it's about 300msec on this dev box,
            // which is not coincidentally just about the time taken to make a DNS request)
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                using (var client = new WebClient())
                {

                    client.DownloadString(message.State);
                }
            }
            catch (Exception ex)
            {
                // And if that blows up, log the error. Since it's not the job of
                // the Model to format the data, the View ought to do something with 
                // it so it doesnt just display the entire thing from exception.ToString()...
                responseTime.Error = ex;
            }
            finally
            {
                sw.Stop();
                responseTime.Time = sw.Elapsed;
                // Now we send the message to the request's reply-to destination
                // the message we send has null as the reply-to, indicating that 
                // there's no further prcessing to be done after the message has been
                // processed

                // be defensive... Or attach an exception handler to the Post() operation. 
                // after all, this could be being used to make a heartbeat call to a web
                // service or some other off-label shenanigans where the caller doesn't
                // care about the result
                if (message.ReplyTo != null)
                    message.ReplyTo.Post(new Message<UrlFetchResult>(responseTime, null));
            }
        }
    }
}
