using System.Text.Json;
using System.Threading.Tasks;
using Fleck;
using lib;

namespace api;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        State.Connections[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser()));
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto;