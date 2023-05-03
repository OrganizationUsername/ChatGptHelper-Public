namespace Helper.ServiceGateways.Models;

public class Query
{
    public int QueryId { get; set; }
    public string Text { get; set; }
    public int TokenCount { get; set; }
    public int? ResponseId { get; set; }
    public Response Response { get; set; }
}