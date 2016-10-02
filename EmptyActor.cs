using System;

namespace Actor
{
    /// <summary>
    /// An actor variant designed to be a stateless container with no embedded logic.
    /// Instances of this class should have logic injected at runtime.
    /// </summary>
    public sealed class EmptyActor:ActorBase
    {
        public EmptyActor() : base(true) { }

        public new void AddState<TState>(Action<Message<TState>> action)
        {
            base.AddState(action);
        }

        public new void AddState<TState, RState>(Func<Message<TState>, RState> func)
        {
            base.AddState(func);
        }

        public new void RemoveState(Type t)
        {
            base.RemoveState(t);
        }
    }
}
