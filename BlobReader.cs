using System.IO;

namespace CADShark.Common.DBManager;

public static class BlobReader
{
    public static byte[] ReadAllBytes(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new IOException("File path is empty.");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File was not found.", filePath);

        return File.ReadAllBytes(filePath);
    }
}
