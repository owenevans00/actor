using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfActor.ViewModel
{
    class ViewModel
    {
        // Dispatcher object for posting updates to the view thread
        public Dispatcher Dispatcher { get; private set; }

        // list of times it took to get back a given Url
        public ResponseTimes ResponseTimes { get; private set; }

        public ViewModel(Dispatcher dispatcher)
        {
            ResponseTimes = new ResponseTimes();
            Dispatcher = dispatcher;
        }

        public void GetUrlResponseTime(string Url)
        {
            // Create instance of actor to do work - this actor is part of the 
            // "business logic" (such as it is) so it lives in the Model
            var updateActor = new Model.UrlFetchActor();

            // Create instance of actor to handle the response
            var responseActor = new ViewModelUpdateActor(this);

            // queue up a work item on the actor - if we wanted to get back
            // results for a list of urls, we'd just call this in a loop and 
            // it would automatically happen in parallel
            // Considered doing this as a whois app but the responses
            // from each different registry's whois servers vary wildly
            // so it would be a bigger demo than I wanted
            updateActor.Post(new Actor.Message<Uri>(new Uri(Url), responseActor));
        }
    }
}
