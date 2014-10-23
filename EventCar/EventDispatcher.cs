using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.ServiceLocation;

namespace EventCar
{
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<Type, Type> mappings = new Dictionary<Type, Type>();

        public void Dispatch<TEvent>(TEvent ev) where TEvent : IEvent
        {
            var eventType = ev.GetType();

            if (!mappings.ContainsKey(eventType))
            {
                throw new NotSupportedException("No handler implemented for type " + eventType.Name);
            }

            var handlerType = mappings[eventType];

            var instance = ServiceLocator.Current.GetInstance(handlerType);

            var handler = instance as IEventHandler<TEvent>;

            handler.Handle(ev);
        }

        public void Register<TEvent, THandler>() where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var handlerType = typeof(THandler);

            mappings.Add(eventType, handlerType);
            
        }
    }
}
