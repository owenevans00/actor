using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor;
using System.Windows.Threading;
using WpfActor.Model;

namespace WpfActor.ViewModel
{
    /// <summary>
    /// Receives a message posted back from some other actor and marshals it back via a nice
    /// asynchronous Dispatcher.BeginInvoke() call
    /// </summary>
    class ViewModelUpdateActor : ActorBase
    {
        // this actor has a close relationship with ViewModel, so we keep a reference to it
        private ViewModel _viewModel;

        public ViewModelUpdateActor(ViewModel viewModel)
        {
            _viewModel = viewModel;
            Init();
        }

        // This actor only does on thing because there is only one thing in the viewmodel
        // that can have something done to it. Depending on preference, one could have this
        // actor play many roles or keep many actors each of which does one or a few things.
        private void Init()
        {
            // Call Update(message). Using a member function for clarity but it could be 
            // an anonymous closure
            _states.Add(typeof(Message<UrlFetchResult>), (Action<Message<UrlFetchResult>>)Update);

            // code above makes more sense, I think, and the compiler will inline it anyway
            // _states.Add(typeof(Message<ResponseTime>),
            //            new Action<Message<ResponseTime>>(
            //                m => _viewModel.Dispatcher.BeginInvoke(
            //                    new Action(() => _viewModel.ResponseTimes.Add(m.State))
            //                    )));
        }

        /// <summary>
        /// Call the Dispatcher to update the ViewModel. Otherwise it will all fall down
        /// because there's no knowing what thread we'll be on when this gets called
        /// </summary>
        /// <param name="message"></param>
        private void Update(Message<UrlFetchResult> message)
        {
            // Avoid NotSupprtedException with message 
            // "This type of CollectionView does not support changes to its SourceCollection 
            // from a thread different from the Dispatcher thread."
            _viewModel.Dispatcher.BeginInvoke(
                new Action(() => _viewModel.ResponseTimes.Add(message.State))
                );
            
        }
    }
}
