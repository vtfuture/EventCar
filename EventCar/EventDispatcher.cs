using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Practices.ServiceLocation;

namespace EventCar
{
    public class EventDispatcher : BaseEventDispatcher
    {
        private Dictionary<Type, List<Type>> mappings = new Dictionary<Type, List<Type>>();

        public override Task DispatchAsync<TEvent>(TEvent ev, bool required)
        {
            var instanceList = GetInstances(ev, required);

            if (instanceList != null)
            {
                var tasks = new List<Task>();

                foreach (var handler in instanceList.OrderBy(r => r.Order))
                {
                    tasks.Add(handler.HandleAsync(ev));
                }

                return Task.WhenAll(tasks);
            }

            return Task.FromResult(0);
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

        private IEnumerable<IEventHandler<TEvent>> GetInstances<TEvent>(TEvent ev, bool required)
        {
            var eventType = ev.GetType();

            var handlerTypes = GetMappings(eventType);

            if (handlerTypes == null)
            {
                if (required)
                {
                    throw new NotSupportedException("No handler implemented for type " + eventType.Name);
                }

                return null;
            }

            return handlerTypes.Select(
                x =>
                {
                    var instance = ServiceLocator.Current.GetInstance(x);

                    var handler = instance as IEventHandler<TEvent>;

                    return handler;
                });

        }

        private List<Type> GetMappings(Type eventType)
        {
            if (!mappings.ContainsKey(eventType))
            {
                return null;
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
