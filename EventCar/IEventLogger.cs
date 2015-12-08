using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCar
{
    public interface IEventLogger
    {
        void LogExecutionTime(long totalExecutionTime, Dictionary<string, long> handlersExecutionTime);
    }
}
