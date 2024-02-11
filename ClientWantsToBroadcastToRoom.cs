using System.Text.Json;
using Fleck;
using lib;
using ws.infrastructure.datamodels;
using ws.infrastructure.repositories;

namespace ws;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string message { get; set; }
    public int roomId { get; set; }
}

public class ClientWantsToBroadcastToRoom : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    private MessageRepository _messageRepository;

    public ClientWantsToBroadcastToRoom(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    public override Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        var message = new ServerBroadcastsMessageWithUsername()
        {
            message = dto.message,
            username = State.Connections[socket.ConnectionInfo.Id].Username
        };
        State.BroadcastToRoom(dto.roomId, JsonSerializer.Serialize(message));
        _messageRepository.CreateMessage(message.username, message.message, dto.roomId);
        return Task.CompletedTask;
    }
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}