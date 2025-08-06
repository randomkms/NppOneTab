using static System.Environment;

namespace NppOneTab;

internal static class Program
{
    static void Main()
    {
        // e.g. C:\Users\mkyian\AppData\Roaming\Notepad++\backup
        var nppBackupFolder = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "Notepad++", "backup");
        Console.WriteLine($"Processing backup folder {nppBackupFolder}");

        var latestUnsavedFiles = Directory.GetFiles(nppBackupFolder, "new *")
            .GroupBy(static f => f[..f.IndexOf('@')])
            .Select(static gr => gr.Max()!)
            .Where(static f => new FileInfo(f).Length != 0)
            .ToArray();
        if (latestUnsavedFiles.Length == 0)
        {
            Console.WriteLine("No unsaved files found");
            Exit();
            return;
        }

        var saveDestination = Directory.CreateDirectory(Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), "NppOneTab" + DateTime.Now.ToString("yyyy-dd-MM_HHmmss"))).FullName;
        Console.WriteLine($"Copying unsaved files to {saveDestination}");

        foreach (var file in latestUnsavedFiles)
        {
            File.Copy(file, Path.Combine(saveDestination, Path.ChangeExtension(Path.GetFileName(file), ".txt")));
        }

        Exit();
    }

    private static void Exit()
    {
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}
