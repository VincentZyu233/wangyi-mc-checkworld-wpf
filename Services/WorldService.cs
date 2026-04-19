using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace WangyiMCHelper;

/// <summary>
/// 世界存档模型
/// </summary>
public class WorldInfo
{
    [JsonProperty("folder")]
    public string Folder { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("lastSaved")]
    public DateTime? LastSaved { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonIgnore]
    public string SizeFormatted => FormatSize(Size);

    [JsonIgnore]
    public string LastSavedText => LastSaved?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知";

    private static string FormatSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int unitIndex = 0;
        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }
        return $"{size:F2} {units[unitIndex]}";
    }
}

/// <summary>
/// 存档服务 - 核心业务逻辑
/// </summary>
public class WorldService
{
    private static readonly string WorldsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MinecraftPC_Netease_PB",
        "minecraftWorlds");

    public string GetWorldsDirectory() => WorldsDir;

    public bool DirectoryExists() => Directory.Exists(WorldsDir);

    public WorldInfo[] GetAllWorlds()
    {
        if (!Directory.Exists(WorldsDir))
            return Array.Empty<WorldInfo>();

        var worlds = new List<WorldInfo>();
        foreach (var dir in Directory.GetDirectories(WorldsDir))
        {
            var info = GetWorldInfo(dir);
            if (info != null)
                worlds.Add(info);
        }

        return worlds.OrderByDescending(w => w.LastSaved).ToArray();
    }

    public WorldInfo? GetWorldInfo(string folderPath)
    {
        var folderName = Path.GetFileName(folderPath);
        var levelNamePath = Path.Combine(folderPath, "levelname.txt");
        var levelDatPath = Path.Combine(folderPath, "level.dat");

        if (!File.Exists(levelNamePath))
            return null;

        // 读取世界名称
        var name = File.ReadAllText(levelNamePath, System.Text.Encoding.UTF8).Trim();

        // 计算文件夹大小
        var size = GetDirectorySize(folderPath);

        // 获取最后保存时间
        DateTime? lastSaved = null;
        if (File.Exists(levelDatPath))
        {
            lastSaved = File.GetLastWriteTime(levelDatPath);
        }

        return new WorldInfo
        {
            Folder = folderName,
            Name = name,
            LastSaved = lastSaved,
            Size = size
        };
    }

    public bool RenameWorld(string folderPath, string newName)
    {
        var levelNamePath = Path.Combine(folderPath, "levelname.txt");
        if (!File.Exists(levelNamePath))
            return false;

        File.WriteAllText(levelNamePath, newName, System.Text.Encoding.UTF8);
        return true;
    }

    public string? BackupWorld(string folderPath, string? destination = null)
    {
        var folderName = Path.GetFileName(folderPath);
        var backupDir = destination ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MCWorldsBackups");

        if (!Directory.Exists(backupDir))
            Directory.CreateDirectory(backupDir);

        var zipPath = Path.Combine(backupDir, $"{folderName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

        try
        {
            ZipFile.CreateFromDirectory(folderPath, zipPath);
            return zipPath;
        }
        catch
        {
            return null;
        }
    }

    public bool DeleteWorld(string folderPath, bool backupFirst = true)
    {
        if (backupFirst)
        {
            var backup = BackupWorld(folderPath);
            if (backup == null)
                return false;
        }

        try
        {
            Directory.Delete(folderPath, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static long GetDirectorySize(string path)
    {
        long size = 0;
        foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            try
            {
                size += new FileInfo(file).Length;
            }
            catch { }
        }
        return size;
    }
}