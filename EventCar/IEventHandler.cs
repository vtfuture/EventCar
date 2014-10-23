using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(TEvent eventMessage);
    }
}
