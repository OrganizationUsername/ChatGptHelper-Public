using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Helper.Wpf.Image;

public class ImageSaver : IImageSaver
{
    public async Task SaveImagesAsync(ImageSearchThing imageSearchThing, string outputFolder)
    {
        if (!Directory.Exists(outputFolder)) { Directory.CreateDirectory(outputFolder); }

        var imageIndex = 1;
        foreach (var image in imageSearchThing.Images)
        {
            string fullPath;
            do
            {
                var fileName = $"{imageIndex}.png";
                fullPath = Path.Combine(outputFolder, fileName);
                imageIndex++;
            }
            while (File.Exists(fullPath));

            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(fileStream);
        }
    }
}