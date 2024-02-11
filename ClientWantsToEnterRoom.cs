using System.Text.Json;
using Fleck;
using lib;

namespace ws;

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