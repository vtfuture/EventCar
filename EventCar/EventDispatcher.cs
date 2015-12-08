using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Practices.ServiceLocation;

namespace EventCar
{
    public class EventDispatcher : BaseEventDispatcher
    {
        private Dictionary<Type, List<Type>> mappings = new Dictionary<Type, List<Type>>();

        public override async Task DispatchAsync<TEvent>(TEvent ev, bool required)
        {
            var instanceList = GetInstances(ev, required);

            if (instanceList != null)
            {
                var sw = Stopwatch.StartNew();
                Dictionary<string, long> handlerTimings = new Dictionary<string, long>();

                foreach (var handler in instanceList.OrderBy(r => r.Order).ToList())
                {
                    var hsw = Stopwatch.StartNew();

                    await handler.HandleAsync(ev);

                    hsw.Stop();

                    var handlerMs = hsw.ElapsedMilliseconds;

                    handlerTimings.Add(handler.GetType().Name, hsw.ElapsedMilliseconds);
                }

                sw.Stop();

                var totalMs = sw.ElapsedMilliseconds;

                if (Logger != null)
                {
                    Logger.LogExecutionTime(totalMs, handlerTimings);
                }
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

        public override void SetLogger(IEventLogger logger)
        {
            Logger = logger;
        }
    }
}
