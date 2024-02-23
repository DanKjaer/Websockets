using System.Text.Json;
using System.Threading.Tasks;
using api.infrastructure.datamodels;
using api.infrastructure.repositories;
using Fleck;
using lib;

namespace api;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int roomid { get; set; }
}

public class ClientWantsToEnterRoom : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    private readonly MessageRepository _chat;
    private ClientWantsToEnterRoom(MessageRepository chat)
    {
        _chat = chat;
    }
    public override Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        var oldChat = _chat.GetMessage(dto.roomid);
        var isSuccess = State.AddToRoom(socket, dto.roomid);
        socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            message = "You were successfully added to room with id" + dto.roomid,
            chatMessage = oldChat.ToList()
        }));
        return Task.CompletedTask;
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public string message { get; set; }
    public List<Message> chatMessage { get; set; }
}