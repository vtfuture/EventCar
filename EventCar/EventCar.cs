using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public static class EventCar
    {
        public static IEventDispatcher Dispatcher { get; set; }

        public static void Fire<TEvent>(TEvent ev) where TEvent : IEvent
        {
            Dispatcher.Dispatch(ev);
        }

        public static void Register<TEventHandler, TEvent>() where TEventHandler : IEventHandler<TEvent> 
                                                            where TEvent : IEvent
        {
            Dispatcher.Register<TEvent, TEventHandler>();
        }
    }
}
