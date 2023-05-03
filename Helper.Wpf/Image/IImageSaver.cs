using System.Threading.Tasks;

namespace Helper.Wpf.Image;

public interface IImageSaver
{
    Task SaveImagesAsync(ImageSearchThing imageSearchThing, string outputFolder);
}