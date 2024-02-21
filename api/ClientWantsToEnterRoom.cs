using System.Text.Json;
using System.Threading.Tasks;
using Fleck;
using lib;

namespace api;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int roomid { get; set; }
}

public class ClientWantsToEnterRoom : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        var isSuccess = State.AddToRoom(socket, dto.roomid);
        socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            message = "You were successfully added to room with id" + dto.roomid
        }));
        return Task.CompletedTask;
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public string message { get; set; }
}