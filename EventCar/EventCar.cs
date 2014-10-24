using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
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
                var handlers = types.Where(HasEventHandlerInterface).ToList();
                foreach (var ev in eventTypes)
                {
                    var myHandlers = handlers.Where(r => GetEventHandlerInterface(r).GetGenericArguments().Any(x => x == ev)).ToList();
                    myHandlers.ForEach(
                        r =>
                            {
                                Dispatcher.Register(ev, r);
                            });
                }
            }
        }

        private static Type GetEventHandlerInterface(Type evHandlerType)
        {
            return evHandlerType.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .FirstOrDefault();
        }

        private static bool HasEventHandlerInterface(Type eventHandler)
        {
            return GetEventHandlerInterface(eventHandler) != null;
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
