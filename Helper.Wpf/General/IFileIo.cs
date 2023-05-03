using System.Collections.Generic;

namespace Helper.Wpf.General;

public interface IFileIo
{
    byte[] SerializeVector(List<float> vector);
}