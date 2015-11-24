﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventCar
{
    public static class EventCar
    {
        public static BaseEventDispatcher Dispatcher { get; set; }

        public static Task FireAsync<TEvent>(TEvent ev, bool required = false) where TEvent : IEvent
        {
            CheckForDispatcher();
            return Dispatcher.DispatchAsync(ev, required);
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

            var eventTypes = new List<Type>();
            var handlers = new List<Type>();


            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                eventTypes.AddRange(types.Where(r => typeof(IEvent).IsAssignableFrom(r)).ToList());
                handlers.AddRange(types.Where(HasEventHandlerInterface).ToList());
            }

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
