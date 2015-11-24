using System.Threading.Tasks;

namespace EventCar
{
    public interface IEventHandler<in TEvent>
    {
        int Order { get; }

        Task HandleAsync(TEvent eventMessage);
    }
}
