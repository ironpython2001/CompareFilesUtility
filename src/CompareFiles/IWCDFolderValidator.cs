//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.

public interface IWCDFolderValidator
{
    Task<string> ValidateAsync(string wcdFileId, string srcDir);
}