using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.ServiceLocation;

namespace EventCar
{
    public class EventDispatcher : BaseEventDispatcher
    {
        private Dictionary<Type, List<Type>> mappings = new Dictionary<Type, List<Type>>();

        public override void Dispatch<TEvent>(TEvent ev)
        {
            var eventType = ev.GetType();

            var handlerTypes = GetMappings(eventType);

            var instanceList = handlerTypes.Select(
                x =>
                    {
                        var instance = ServiceLocator.Current.GetInstance(x);

                        var handler = instance as IEventHandler<TEvent>;

                        return handler;
                    });

            foreach (var handler in instanceList.OrderBy(r => r.Order))
            {
                handler.Handle(ev);    
            }
        }

        public override void Register<TEvent, THandler>()
        {
            var eventType = typeof(TEvent);
            var handlerType = typeof(THandler);

            Register(eventType, handlerType);
        }

        internal override void Register(Type eventType, Type handlerType)
        {
            AddMapping(eventType, handlerType);
        }

        private List<Type> GetMappings(Type eventType)
        {
            if (!mappings.ContainsKey(eventType))
            {
                throw new NotSupportedException("No handler implemented for type " + eventType.Name);
            }

            return mappings[eventType];
        }

        private void AddMapping(Type eventType, Type handlerType)
        {
            List<Type> typeList = null;
            if (!mappings.ContainsKey(eventType))
            {
                typeList = new List<Type>();
                mappings.Add(eventType, typeList);
            }
            else
            {
                typeList = mappings[eventType];
            }

            typeList.Add(handlerType);
        }
    }
}
