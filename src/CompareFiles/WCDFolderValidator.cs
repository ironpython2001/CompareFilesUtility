//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

public class WCDFolderValidator : IWCDFolderValidator
{
    public async Task<string> ValidateAsync(string wcdFileId, string srcDir)
    {
        string wcdDirPath = Path.Combine(srcDir, wcdFileId);
        string wcdZipFile = Path.Combine(wcdDirPath, $"{wcdFileId}.zip");
        string wcdMD5File = Path.Combine(wcdDirPath, $"{wcdFileId}.md5");
        
        if (!File.Exists(wcdZipFile))
        {
            return $"{wcdFileId}.zip file does not exist.";
        }
        if (!File.Exists(wcdMD5File))
        {
            return $"{wcdFileId}.md5 file does not exist.";
        }

        Dictionary<string, string> filesToCheck = new()
        {
            ["media/kestra-pfs/data/database/primary.db"] = string.Empty,
            ["media/kestra-pfs/data/database/primary.db-shm"] = string.Empty,
            ["media/kestra-pfs/data/database/primary.db-wal"] = string.Empty
        };

        await foreach (string line in File.ReadLinesAsync(wcdMD5File))
        {
            string[] parts = line.Split("  ", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && filesToCheck.ContainsKey(parts[1]))
            {
                filesToCheck[parts[1]] = parts[0];
                if (!filesToCheck.ContainsValue(string.Empty)) break;
            }
        }

        StringBuilder result = new();
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
                        result.AppendLine($"{entry.FullName} - MATCH");
                        matchCount++;
                    }
                    else
                    {
                        result.AppendLine($"{entry.FullName} - MISMATCH");
                    }

                    if (matchCount == filesToCheck.Count) break;
                }
            }
        }
        return result.ToString();
        
    }
}


