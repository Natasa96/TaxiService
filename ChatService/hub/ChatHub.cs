using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class ChatHub : Hub
{

    private readonly ILogger<ChatHub> _logger;
    private readonly SharedDb _sharedDb;

    public ChatHub(ILogger<ChatHub> logger, SharedDb sharedDb)
    {
        _sharedDb = sharedDb;
        _logger = logger;
    }

    public async Task SendMessage(string message)
    {
        try
        {
            if (_sharedDb.Connections.TryGetValue(Context.ConnectionId, out var connection))
            {
                await Clients.Group(connection.ChatRoom).SendAsync("ReceiveMessage", connection.Username, message);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in SendMessage");
        }
    }

    public async Task JoinChat(string username, string chatroom)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatroom);
            _sharedDb.Connections[Context.ConnectionId] = new UserConnection() { ChatRoom = chatroom, Username = username };
            await Clients.Group(chatroom).SendAsync("JoinChat", "admin", $"{username} has joined the chat room");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in SendMessage");
        }
    }

    // public async Task SendMessage(string user, string message)
    // {
    //     var userId = Context.GetHttpContext().Request.Query["access_token"];
    //     Console.WriteLine("USERID", userId);
    //     System.Diagnostics.Debug.WriteLine("UserID: " + userId);
    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
    //     }

    //     await Clients.User(user).SendAsync("ReceiveMessage", userId, message);
    // }

    // public override Task OnConnectedAsync()
    // {
    //     var userId = Context.GetHttpContext().Request.Query["access_token"][0];
    //     Console.WriteLine("USERID", userId);
    //     System.Diagnostics.Debug.WriteLine("UserID: " + userId);

    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
    //     }

    //     Groups.AddToGroupAsync(Context.ConnectionId, "TestConnection");
    //     return base.OnConnectedAsync();
    // }

    // public override Task OnDisconnectedAsync(Exception exception)
    // {
    //     var userId = Context.GetHttpContext().Request.Query["access_token"][0];
    //     System.Diagnostics.Debug.WriteLine("UserID: " + userId);
    //     Console.WriteLine("USERID", userId);
    //     if (!string.IsNullOrEmpty(userId))
    //     {
    //         Groups.RemoveFromGroupAsync(Context.ConnectionId, "TestConnection");
    //     }

    //     return base.OnDisconnectedAsync(exception);
    // }
}
