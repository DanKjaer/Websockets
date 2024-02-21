using System.Collections.Generic;
using Dapper;
using Npgsql;
using api.infrastructure.datamodels;

namespace api.infrastructure.repositories;

public class MessageRepository
{
    private NpgsqlDataSource _dataSource;

    public MessageRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    
    public IEnumerable<Message> GetMessage(int roomId)
    {
        var sql = "SELECT * FROM main.messages WHERE roomId = @RoomId ORDER BY id DESC";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Message>(sql, new {RoomId = roomId});
        }
    }

    public void CreateMessage(string username, string message, int roomid)
    {
        var sql = "INSERT INTO main.messages (username, message, roomid) VALUES (@username, @message, @roomid)";
        using (var conn = _dataSource.OpenConnection())
        { 
            conn.Execute(sql, new {username, message, roomid});
        }
    }
}