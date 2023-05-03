namespace Helper.ServiceGateways.Models;

public class EmbedResult
{
    public int EmbedThingId { get; set; }
    public string Text { get; set; }
    public byte[] Vector { get; set; }
}