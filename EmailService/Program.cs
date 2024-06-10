var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new RabbitMQConsumer(configuration);
});

var app = builder.Build();


app.UseHttpsRedirection();

//Start RabbitMQ Listener
var rabbitMQListener = app.Services.GetRequiredService<RabbitMQConsumer>();
await rabbitMQListener.StartConsuming();


app.Run();
