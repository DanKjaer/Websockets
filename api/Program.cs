using System.Reflection;
using System.Text.Json;
using api;
using Fleck;
using lib;
using api.infrastructure;
using api.infrastructure.repositories;



public static class Startup
{
    public static void Main(string[] args)
    {
        startUp(args);
        Console.ReadLine();
    }

    public static void startUp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

        builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
            dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());

        builder.Services.AddSingleton<MessageRepository>();

        var app = builder.Build();



        var server = new WebSocketServer("ws://0.0.0.0:8181");
        var wsConnections = new List<IWebSocketConnection>();
        server.Start(ws =>
        {
            ws.OnOpen = () =>
            {
                State.AddConnection(ws);
            };
            ws.OnMessage = async message =>
            {
                try
                {
                    await app.InvokeClientEventHandler(clientEventHandlers, ws, message);
                }
                catch(Exception e)
                {
                    ws.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient()
                    {
                        errorMessage = e.Message
                    }));
                    
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e.StackTrace);
                }
            };
        });

    }
}

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string errorMessage { get; set; }
}
