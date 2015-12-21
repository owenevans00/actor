using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor
{
    // TODO: Make this internal
    public static class MessageSerializer
    {
        public  static SerializedMessageWrapper Serialize<T>(Message<T> message)
        {
            var type = message.State.GetType();
            if(!type.IsSerializable)
                throw new InvalidOperationException(
                    string.Format( "Cannot serialize message with state payload of type {0}",type)
                    );

            return new SerializedMessageWrapper { 
                State=message.State,
                MessageType = message.GetType().AssemblyQualifiedName
            };
        }
    }

    [Serializable]
    public class SerializedMessageWrapper
    {
        public object State { get; set; }

        public string MessageType { get; set; }

        public Guid ReplyToInstance { get; set; }

        public Uri ReplyTo { get; set; }
    }
}