//References
//https://code-maze.com/csharp-compare-two-files/#:~:text=If%20the%20file%20sizes%20are%20the%20same%2C%20we%20create%20a,the%20same%20name%20and%20size.
using System.Net;

var wcdFileId = "00006722_1697493954000";
var srcDir = @"C:\Users\spottumuttu\Downloads";

Console.WriteLine($"Start time: {DateTime.Now}");
IWCDFolderValidator validator = new WCDFolderValidator();
Console.WriteLine(await validator.ValidateAsync(wcdFileId, srcDir));
Console.WriteLine($"End time: {DateTime.Now}");
