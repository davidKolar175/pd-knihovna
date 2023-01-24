using System.Diagnostics;

namespace BibliothecaApi.Services;

public class AppService
{
    public AppService()
    {
            
    }

    public async Task BackupDatabase() =>
        await RunProcess(@"C:\Program Files\MongoDB\Tools\100\bin\mongodump.exe", "-d BookStore -o ./mongo-backup");

    public async Task RestoreDatabase() =>
        await RunProcess(@"C:\Program Files\MongoDB\Tools\100\bin\mongorestore.exe", "-d BookStore --drop ./mongo-backup/BookStore");

    private static async Task RunProcess(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = false
        };

        var exportProcess = new Process();
        exportProcess.StartInfo = startInfo;

        exportProcess.Start();
        await exportProcess.WaitForExitAsync();
    }
}
