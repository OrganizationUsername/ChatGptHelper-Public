using System.Collections.Generic;
using System.IO;

namespace Helper.Wpf.General;

public class FileIo : IFileIo
{
    public byte[] SerializeVector(List<float> vector)
    {
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            foreach (var value in vector) { binaryWriter.Write(value); }
        }

        var serializedVector = memoryStream.ToArray();
        return serializedVector;
    }
}