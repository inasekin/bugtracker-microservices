using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus
{
    public class RabbitMqEventBus : IEventBus
    {
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "microservices_exchange", type: "direct");
        }

        public void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;  // e.g. "UserCreatedIntegrationEvent"
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "microservices_exchange",
                routingKey: eventName,
                basicProperties: null,
                body: body
            );
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;

            // Создаём очередь с тем же именем, что eventName
            _channel.QueueDeclare(
                queue: eventName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Биндим к exchange
            _channel.QueueBind(
                queue: eventName,
                exchange: "microservices_exchange",
                routingKey: eventName);

            // Подключаем consumer
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                if (ea.RoutingKey == eventName)
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var eventObj = JsonSerializer.Deserialize<T>(message);

                    // Создаем scope на базе нашего IServiceProvider
                    using var scope = _serviceProvider.CreateScope();

                    // Получаем зарегистрированный хендлер
                    var handler = scope.ServiceProvider.GetService(typeof(IIntegrationEventHandler<T>))
                                   as IIntegrationEventHandler<T>;

                    if (handler != null && eventObj != null)
                    {
                        await handler.Handle(eventObj);
                    }
                }
            };

            _channel.BasicConsume(
                queue: eventName,
                autoAck: true,
                consumer: consumer);
        }
    }
}
