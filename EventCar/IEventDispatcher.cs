using System.Threading.Tasks;

namespace EventCar
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent ev, bool required) where TEvent : IEvent;

        void Register<TEvent, THandler>() where TEvent : IEvent;
    }
}
