using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    public static class Extensions
    {
        public static string AllMessages(this Exception exception)
        {
            return exception.Message + (exception.InnerException != null
                ? " -- > " + exception.InnerException.AllMessages()
                : "");
        }
    }
}
