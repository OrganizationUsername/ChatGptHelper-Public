namespace Helper.ServiceGateways.Models;

public class Response
{
    public int ResponseId { get; set; }
    public int QueryId { get; set; }
    public Query Query { get; set; }
    public string ModelUsed { get; set; }
    public int TokenCount { get; set; }
    public ICollection<Answer> Answers { get; } = new List<Answer>();
}