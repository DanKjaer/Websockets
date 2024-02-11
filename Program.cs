using System.Reflection;
using Fleck;
using lib;
using Npgsql;
using ws;
using ws.infrastructure;
using ws.infrastructure.repositories;

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
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException);
            Console.WriteLine(e.StackTrace);
        }
    };
});

Console.ReadLine();
