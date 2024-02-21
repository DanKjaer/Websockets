using System;
using System.Collections.Generic;
using Fleck;

namespace ws;

public class WsWithMetaData(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
    
    public string Username { get; set; }
}

public static class State
{
    public static Dictionary<Guid, WsWithMetaData> Connections = new();
    public static Dictionary<int, HashSet<Guid>> Rooms = new();
    public static void AddConnection(IWebSocketConnection ws)
    {
        Connections.TryAdd(ws.ConnectionInfo.Id, 
            new WsWithMetaData(ws));
    }

    public static bool AddToRoom(IWebSocketConnection ws, int room)
    {
        if (!Rooms.ContainsKey(room))
            Rooms.Add(room, new HashSet<Guid>());
        return Rooms[room].Add(ws.ConnectionInfo.Id);
    }

    public static void BroadcastToRoom(int room, string message)
    { 
        if(Rooms.TryGetValue(room, out var guids))
            foreach (var guid in guids)
            {
                if(Connections.TryGetValue(guid, out var ws))
                {
                    ws.Connection.Send(message); 
                } 
            }
    }
}