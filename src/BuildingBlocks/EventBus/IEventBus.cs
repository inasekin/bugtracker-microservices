namespace EventBus
{
    public abstract class IntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreationDate { get; } = DateTime.UtcNow;
    }

    public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}