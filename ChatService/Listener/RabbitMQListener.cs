// src/ChatService/RabbitMQListener.cs
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class RabbitMQListener : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<RabbitMQListener> _logger;

    public RabbitMQListener(IHubContext<ChatHub> hubContext, ILogger<RabbitMQListener> logger)
    {
        _queueName = "chatQueue";
        _hubContext = hubContext;
        _logger = logger;

        var factory = new ConnectionFactory() { HostName = "taxirabbitmq" };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to RabbitMQ.");
            throw;
        }
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(message);
            Console.WriteLine($"Received chat request: {chatRequest.UserId1}, {chatRequest.UserId2}");

            await InitiateChat(chatRequest.UserId1, chatRequest.UserId2);
        };
        _channel.BasicConsume(queue: _queueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public void StartListeningWithRetry()
    {
        const int maxRetryCount = 5;
        const int initialDelayMs = 1000; // 1 second
        const int maxDelayMs = 60000;    // 60 seconds

        int retryCount = 0;
        int delayMs = initialDelayMs;

        while (retryCount < maxRetryCount)
        {
            try
            {
                StartListening(); // Attempt to start listening to RabbitMQ
                return; // Connection successful, exit retry loop
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, $"Failed to connect to RabbitMQ. Retry attempt {retryCount + 1}");

                // Exponential backoff: increase delay for next retry
                delayMs = Math.Min(delayMs * 2, maxDelayMs);
                retryCount++;

                // Wait before next retry attempt
                Task.Delay(delayMs).Wait();
            }
        }

        // If max retry count reached, log error and exit
        _logger.LogError($"Failed to connect to RabbitMQ after {maxRetryCount} attempts. Exiting.");
    }


    private async Task InitiateChat(string userId1, string userId2)
    {
        await _hubContext.Clients.User(userId1).SendAsync("InitiateChat", userId2);
        await _hubContext.Clients.User(userId2).SendAsync("InitiateChat", userId1);
        Console.WriteLine($"Chat initiated between {userId1} and {userId2}");
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}