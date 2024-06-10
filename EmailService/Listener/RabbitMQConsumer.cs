// RabbitMQ Consumer Example
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using DataLib;
using System.Text;

public class RabbitMQConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;

    public RabbitMQConsumer(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = "taxirabbitmq",
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "emailEventQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
        _configuration = configuration;
    }

    public async Task StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received event: {message}");
            var chatRequest = JsonSerializer.Deserialize<RabbitEmailRequest>(message);
            Console.WriteLine($"Parsed object: {chatRequest.Email}");

            // Process the event and send email using Mailtrap
            await SendEmail(chatRequest.Email, chatRequest.DriverName);

            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queue: "emailEventQueue",
                              autoAck: false,
                              consumer: consumer);
    }

    private async Task SendEmail(string recipientEmail, string driverName)
    {
        // Extract Mailtrap SMTP settings from configuration
        var smtpHost = _configuration["Mailtrap:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Mailtrap:SmtpPort"]);
        var smtpUsername = _configuration["Mailtrap:SmtpUsername"];
        var smtpPassword = _configuration["Mailtrap:SmtpPassword"];

        // Construct the email message
        var fromAddress = new MailAddress("taxiServiceFTN@gmail.com", "TaxiService");
        var toAddress = new MailAddress(recipientEmail, driverName);
        const string subject = "Status Update";
        string body = $"Hello {driverName},\n\nYour status has been changed to Approved. You can now login.\n\nSincerely,\nAdmin";

        // Configure the SMTP client
        var smtpClient = new SmtpClient
        {
            Host = smtpHost,
            Port = smtpPort,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true // Set to true if Mailtrap requires SSL
        };

        // Send the email
        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            await smtpClient.SendMailAsync(message);
        }
    }
}
