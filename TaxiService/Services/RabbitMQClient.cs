// src/TaxiService/RabbitMQClient.cs
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQClient : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMQClient(string hostname, string queueName)
    {
        _queueName = queueName;

        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void SendMessage<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "",
                             routingKey: _queueName,
                             basicProperties: null,
                             body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
