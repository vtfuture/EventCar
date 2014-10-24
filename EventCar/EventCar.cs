using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public static class EventCar
    {
        public static BaseEventDispatcher Dispatcher { get; set; }

        public static void Fire<TEvent>(TEvent ev) where TEvent : IEvent
        {
            CheckForDispatcher();
            Dispatcher.Dispatch(ev);
        }

        public static void Register<TEventHandler, TEvent>() where TEventHandler : IEventHandler<TEvent> 
                                                            where TEvent : IEvent
        {
            CheckForDispatcher();
            Dispatcher.Register<TEvent, TEventHandler>();
        }

        public static void RegisterAll(params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                var eventTypes = types.Where(r => typeof(IEvent).IsAssignableFrom(r)).ToList();
                var handlers = types.Where(r => r.GetInterfaces()
                                                    .Where(x => x.IsGenericType)
                                                    .Where(x => x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                                    .Any()).ToList();
                foreach (var ev in eventTypes)
                {
                    var myHandlers = handlers.Where(r => r.GetGenericArguments().Contains(ev)).ToList();
                    myHandlers.ForEach(
                        r =>
                            {
                                Dispatcher.Register(ev, r);
                            });
                }
            }
        }

        private static void CheckForDispatcher()
        {
            if (Dispatcher == null)
            {
                throw new NullReferenceException("No EventDispatcher has been set.");
            }
        }
    }
}
