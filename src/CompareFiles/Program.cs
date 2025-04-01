//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.IO.Compression;
using System.Security.Cryptography;


var wcdFileId = "00007794_1711405171000";
string[] fileNamesNeedToCheck = { "primary.db", "primary.db-shm", "primary.db-wal" };

var wcdDirPath = $"C:\\Users\\spottumuttu\\Downloads\\{wcdFileId}";
var wcdZipFile = Path.Combine($"{wcdDirPath}", $"{wcdFileId}.zip");
var wcdMD5File = Path.Combine($"{wcdDirPath}", $"{wcdFileId}.md5");

//folder should contain .zip file
if (File.Exists(wcdZipFile) == false)
    Console.WriteLine($"{wcdFileId}.zip file not exists ");
//folder should contain .md5 file
if (File.Exists(wcdMD5File) == false)
    Console.WriteLine($"{wcdFileId}.md5 file not exists ");



for (var i = 0; i < fileNamesNeedToCheck.Length; i++)
{
    var targetFileCheckSum = string.Empty;
    var targetFile = $"media/kestra-pfs/data/database/{fileNamesNeedToCheck[i]}";
    await foreach (var line in File.ReadLinesAsync(wcdMD5File))
    {
        if ((line.Contains(targetFile)) && (targetFile == line.Split("  ")[1]))
        {
            targetFileCheckSum = line.Split("  ")[0];
            break;
        }
    }

    //compute the checksum from the zip files
    using FileStream zipToOpen = new FileStream(wcdZipFile, FileMode.Open);
    using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
        if (entry.FullName == targetFile)
        {
            var zipStream1 = entry.Open();
            //Span<byte> hash = stackalloc byte[16];
            //MD5.HashData(zipStream1, hash);
            //hash.SequenceEqual(hash2);
            byte[] result = await MD5.HashDataAsync(zipStream1);
            string targetFileCheckSumInZipArchive = BitConverter.ToString(result).Replace("-", "").ToLower();
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(targetFileCheckSum);
            //result.SequenceEqual(buffer);
            if (targetFileCheckSum == targetFileCheckSumInZipArchive)
                Console.WriteLine("true");
            else
                Console.WriteLine("false");
        }
    }

}


static bool CompareByChecksum(string firstFilePath, string secondFilePath)
{
    using var stream1 = new FileStream(firstFilePath, FileMode.Open, FileAccess.Read);
    using var stream2 = new FileStream(secondFilePath, FileMode.Open, FileAccess.Read);

    //if (stream1.Length != stream2.Length)
    //{
    //    return false;
    //}

    Span<byte> hash1 = stackalloc byte[16];
    Span<byte> hash2 = stackalloc byte[16];

    MD5.HashData(stream1, hash1);
    MD5.HashData(stream2, hash2);

    return hash1.SequenceEqual(hash2);
}