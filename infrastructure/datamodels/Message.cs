namespace ws.infrastructure.datamodels;

public class Message
{
    public int id { get; set; }
    public string username { get; set; }
    public string message { get; set; }
    public int roomId { get; set; }
}