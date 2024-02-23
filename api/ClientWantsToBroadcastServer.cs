using System.Text.Json;
using System.Threading.Tasks;
using Fleck;
using lib;

namespace api;

public class ClientWantsToBroadcastServerDto: BaseDto
{
    public string messageContent { get; set; }
}

public class ClientWantsToBroadcastToServer : BaseEventHandler<ClientWantsToBroadcastServerDto>
{
    public override Task Handle(ClientWantsToBroadcastServerDto dto, IWebSocketConnection socket)
    {
        var broadcast = new ServerBroadcastClient()
        {
            broadcastValue = "broadcast: " + dto.messageContent
        };
        var messageToClient = JsonSerializer.Serialize(broadcast);
        foreach (var client in State.Connections)
        {
            if (client.Key == socket.ConnectionInfo.Id)
            {
                continue;
            }
            client.Value.Connection.Send(messageToClient);
        }
        return Task.CompletedTask;
    }
}

public class ServerBroadcastClient : BaseDto
{
    public string broadcastValue { get; set; }
}