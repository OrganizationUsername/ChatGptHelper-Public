namespace Helper.ServiceGateways.Models;

public class ImageQuery
{
    public int ImageQueryId { get; set; }
    public string ImageQueryText { get; set; } = null!;
    public ICollection<ImageResult> ImageResults { get; } = new List<ImageResult>();
}