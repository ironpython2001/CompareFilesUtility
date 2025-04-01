//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.Security.Cryptography;



var file1 = "C:\\Users\\spottumuttu\\Downloads\\EDW-209 Screen Recording (2-20-2025 12-17-16 PM).wmv";
var file2 = "C:\\\\Users\\\\spottumuttu\\\\Downloads\\Building-AI-Applications-with-Microsoft-Semantic-Kernel-main.zip";
Console.WriteLine(DateTime.Now.ToString());    
var result = CompareByChecksum(file1, file2);
Console.WriteLine(DateTime.Now.ToString());
Console.WriteLine(result);  

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