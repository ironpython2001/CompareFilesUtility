//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.IO.Compression;
using System.Security.Cryptography;


var wcdFileId = "00007794_1711405171000";
var srcDir = $"C:\\Users\\spottumuttu\\Downloads";

Console.WriteLine($"start time {DateTime.Now}");

var wcdDirPath = Path.Combine(srcDir, wcdFileId);
var wcdZipFile = Path.Combine($"{wcdDirPath}", $"{wcdFileId}.zip");
var wcdMD5File = Path.Combine($"{wcdDirPath}", $"{wcdFileId}.md5");

//folder should contain .zip file
if (File.Exists(wcdZipFile) == false)
    Console.WriteLine($"{wcdFileId}.zip file not exists ");
//folder should contain .md5 file
if (File.Exists(wcdMD5File) == false)
    Console.WriteLine($"{wcdFileId}.md5 file not exists ");

string primarydbFileCheckSum = string.Empty;
string primarydbshmFileCheckSum = string.Empty;
string primarydbwalFileCheckSum = string.Empty;
string primarydbFile = $"media/kestra-pfs/data/database/primary.db";
string primarydbshmFile = $"media/kestra-pfs/data/database/primary.db-shm";
string primarydbwalFile = $"media/kestra-pfs/data/database/primary.db-wal";

await foreach (var line in File.ReadLinesAsync(wcdMD5File))
{
    if ((primarydbFileCheckSum != string.Empty) && (primarydbshmFileCheckSum != string.Empty) && (primarydbwalFileCheckSum != string.Empty))
    {
        break;
    }
    if ((line.Contains(primarydbFile)) && (primarydbFile == line.Split("  ")[1]))
    {
        primarydbFileCheckSum = line.Split("  ")[0];
    }
    else if ((line.Contains(primarydbshmFile)) && (primarydbshmFile == line.Split("  ")[1]))
    {
        primarydbshmFileCheckSum = line.Split("  ")[0];
    }
    else if ((line.Contains(primarydbwalFile)) && (primarydbwalFile == line.Split("  ")[1]))
    {
        primarydbwalFileCheckSum = line.Split("  ")[0];
    }
    else
    {
        continue;
    }
}

//compute the checksum from the zip files
int count = 0;
using FileStream zipToOpen = new FileStream(wcdZipFile, FileMode.Open);
using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
foreach (ZipArchiveEntry entry in archive.Entries)
{
    if (count == 3) break; //just to avoid unnecessary loops
    if (entry.FullName == primarydbFile)
    {
        using Stream zipStream = entry.Open();
        byte[] hashresult = await MD5.HashDataAsync(zipStream);
        string targetFileCheckSumInZipArchive = Convert.ToHexStringLower(hashresult);
        if (primarydbFileCheckSum == targetFileCheckSumInZipArchive)
        {
            count++;
            Console.WriteLine($"{entry.FullName} true");
        }
        else
        {
            Console.WriteLine($"{entry.FullName} false");
        }
    }
    if (entry.FullName == primarydbshmFile)
    {
        using Stream zipStream = entry.Open();
        byte[] hashresult = await MD5.HashDataAsync(zipStream);
        string targetFileCheckSumInZipArchive = Convert.ToHexStringLower(hashresult);
        if (primarydbshmFileCheckSum == targetFileCheckSumInZipArchive)
        {
            count++;
            Console.WriteLine($"{entry.FullName} true");
        }
        else
        {
            Console.WriteLine($"{entry.FullName} false");
        }
    }
    if (entry.FullName == primarydbwalFile)
    {
        using Stream zipStream = entry.Open();
        byte[] hashresult = await MD5.HashDataAsync(zipStream);
        string targetFileCheckSumInZipArchive = Convert.ToHexStringLower(hashresult);
        if (primarydbwalFileCheckSum == targetFileCheckSumInZipArchive)
        {
            count++;
            Console.WriteLine($"{entry.FullName} true");
        }
        else
        {
            Console.WriteLine($"{entry.FullName} false");
        }
    }
}
Console.WriteLine($"end time {DateTime.Now}");