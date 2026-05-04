using System.IO;

namespace CADShark.Common.DBManager;

public static class BlobReader
{
    public static byte[] ReadAllBytes(string filePath)
    {
        byte[] fileBytes = null;

        if (File.Exists(filePath))
            fileBytes = File.ReadAllBytes(filePath);
        return fileBytes;
    }
}