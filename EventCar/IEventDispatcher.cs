using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public interface IEventDispatcher
    {
        void Dispatch<TEvent>(TEvent ev) where TEvent : IEvent;

        void Register<TEvent, THandler>() where TEvent : IEvent;
    }
}
