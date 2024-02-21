using System.Text.Json;
using System.Threading.Tasks;
using Fleck;
using lib;

namespace ws;

public class ClientWelcomeToServerDto : BaseDto
{
    public string messageContent { get; set; }
}

public class ClientWelcomeToServer : BaseEventHandler<ClientWelcomeToServerDto>
{
    public override Task Handle(ClientWelcomeToServerDto dto, IWebSocketConnection socket)
    {
        var welcome = new ServerWelcomeClient()
        {
            welcomeValue = "welcome: " + socket.ConnectionInfo.Id,
            notificationValue = socket.ConnectionInfo.Id + "has joined the server"
        };
        var welcomeMessage = JsonSerializer.Serialize(welcome);
        foreach (var welcomeClient in State.Connections)
        {
            if (welcomeClient.Key != socket.ConnectionInfo.Id)
            {
                welcomeClient.Value.Connection.Send(welcomeMessage);
            }
        }
        return Task.CompletedTask;
    }
}

public class ServerWelcomeClient : BaseDto
{
    public string welcomeValue { get; set; }
    public string notificationValue { get; set; }
}