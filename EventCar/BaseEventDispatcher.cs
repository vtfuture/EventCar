using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public abstract class BaseEventDispatcher : IEventDispatcher
    {
        public abstract void Dispatch<TEvent>(TEvent ev, bool required) where TEvent : IEvent;

        public abstract void Register<TEvent, THandler>() where TEvent : IEvent;

        internal abstract void Register(Type eventType, Type handlerType);
    }
}
