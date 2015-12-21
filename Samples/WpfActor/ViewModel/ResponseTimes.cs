using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfActor.ViewModel
{
    // Really simple subclass of obbservablecollection to be our 
    // viewmodel's data container
    class ResponseTimes : ObservableCollection<Model.UrlFetchResult> { }
}
