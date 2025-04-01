//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.IO.Compression;
using System.Security.Cryptography;

var wcdFileId = "00007794_1711405171000";
var srcDir = @"C:\Users\spottumuttu\Downloads";

Console.WriteLine($"Start time: {DateTime.Now}");

string wcdDirPath = Path.Combine(srcDir, wcdFileId);
string wcdZipFile = Path.Combine(wcdDirPath, $"{wcdFileId}.zip");
string wcdMD5File = Path.Combine(wcdDirPath, $"{wcdFileId}.md5");

// Validate the required files
if (!File.Exists(wcdZipFile))
{
    Console.WriteLine($"{wcdFileId}.zip file does not exist.");
    return;
}
if (!File.Exists(wcdMD5File))
{
    Console.WriteLine($"{wcdFileId}.md5 file does not exist.");
    return;
}

// Files to check
Dictionary<string, string> filesToCheck = new Dictionary<string, string>
{
    ["media/kestra-pfs/data/database/primary.db"] = string.Empty,
    ["media/kestra-pfs/data/database/primary.db-shm"] = string.Empty,
    ["media/kestra-pfs/data/database/primary.db-wal"] = string.Empty
};

// Read and parse the MD5 checksum file
await foreach (string line in File.ReadLinesAsync(wcdMD5File))
{
    string[] parts = line.Split("  ", StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 2 && filesToCheck.ContainsKey(parts[1]))
    {
        filesToCheck[parts[1]] = parts[0];
        if (!filesToCheck.ContainsValue(string.Empty)) break; // Stop early if all checksums are found
    }
}

// Compute checksums from the zip archive
int matchCount = 0;
using (FileStream zipToOpen = new(wcdZipFile, FileMode.Open, FileAccess.Read, FileShare.Read))
using (ZipArchive archive = new(zipToOpen, ZipArchiveMode.Read))
{
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
        if (filesToCheck.ContainsKey(entry.FullName))
        {
            using Stream zipStream = entry.Open();
            byte[] hashResult = await MD5.HashDataAsync(zipStream);
            string computedChecksum = Convert.ToHexString(hashResult).ToLowerInvariant();

            if (filesToCheck[entry.FullName] == computedChecksum)
            {
                Console.WriteLine($"{entry.FullName} - MATCH");
                matchCount++;
            }
            else
            {
                Console.WriteLine($"{entry.FullName} - MISMATCH");
            }

            if (matchCount == filesToCheck.Count) break; // Stop early if all matches are found
        }
    }
}

Console.WriteLine($"End time: {DateTime.Now}");
