using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WangyiMCHelper;

namespace WangyiMCHelper.ViewModels;

/// <summary>
/// 世界存档项目（用于DataGrid绑定）
/// </summary>
public partial class WorldItem : ObservableObject
{
    [ObservableProperty]
    private string _folder = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private DateTime? _lastSaved;

    [ObservableProperty]
    private long _size;

    public string SizeFormatted => FormatSize(Size);
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
/// 排序方式
/// </summary>
public enum SortMode
{
    TimeDesc,
    TimeAsc,
    NameAsc,
    NameDesc,
    SizeDesc,
    SizeAsc
}

/// <summary>
/// 主窗口ViewModel
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly WorldService _worldService = new();

    [ObservableProperty]
    private ObservableCollection<WorldItem> _worlds = new();

    [ObservableProperty]
    private ObservableCollection<WorldItem> _filteredWorlds = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private WorldItem? _selectedWorld;

    [ObservableProperty]
    private string _statusText = "正在加载...";

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private bool _isDirectoryExists;

    [ObservableProperty]
    private SortMode _currentSort = SortMode.TimeDesc;

    public MainViewModel()
    {
        IsDirectoryExists = _worldService.DirectoryExists();
        if (IsDirectoryExists)
        {
            LoadWorlds();
        }
        else
        {
            StatusText = "存档目录不存在";
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSelectedWorldChanged(WorldItem? value)
    {
        // Can add selection changed logic here if needed
    }

    public void LoadWorlds()
    {
        try
        {
            IsLoading = true;
            StatusText = "正在加载存档...";

            var worlds = _worldService.GetAllWorlds();
            Worlds.Clear();
            foreach (var w in worlds)
            {
                Worlds.Add(new WorldItem
                {
                    Folder = w.Folder,
                    Name = w.Name,
                    LastSaved = w.LastSaved,
                    Size = w.Size
                });
            }

            ApplyFilter();
            StatusText = $"共 {FilteredWorlds.Count} 个存档";
        }
        catch (Exception ex)
        {
            StatusText = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? Worlds.AsEnumerable()
            : Worlds.Where(w =>
                w.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                w.Folder.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        // 应用排序
        filtered = CurrentSort switch
        {
            SortMode.TimeDesc => filtered.OrderByDescending(w => w.LastSaved),
            SortMode.TimeAsc => filtered.OrderBy(w => w.LastSaved),
            SortMode.NameAsc => filtered.OrderBy(w => w.Name),
            SortMode.NameDesc => filtered.OrderByDescending(w => w.Name),
            SortMode.SizeDesc => filtered.OrderByDescending(w => w.Size),
            SortMode.SizeAsc => filtered.OrderBy(w => w.Size),
            _ => filtered.OrderByDescending(w => w.LastSaved)
        };

        FilteredWorlds = new ObservableCollection<WorldItem>(filtered);
        StatusText = $"共 {FilteredWorlds.Count} 个存档";
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadWorlds();
    }

    [RelayCommand]
    private void Sort(string mode)
    {
        CurrentSort = mode switch
        {
            "TimeDesc" => SortMode.TimeDesc,
            "TimeAsc" => SortMode.TimeAsc,
            "NameAsc" => SortMode.NameAsc,
            "NameDesc" => SortMode.NameDesc,
            "SizeDesc" => SortMode.SizeDesc,
            "SizeAsc" => SortMode.SizeAsc,
            _ => SortMode.TimeDesc
        };
        ApplyFilter();
    }

    [RelayCommand]
    private void Backup(WorldItem? world)
    {
        if (world == null) return;

        var path = Path.Combine(_worldService.GetWorldsDirectory(), world.Folder);
        var backupPath = _worldService.BackupWorld(path);

        StatusText = backupPath != null ? "备份成功" : "备份失败";
    }

    [RelayCommand]
    private void Rename(WorldItem? world)
    {
        if (world == null) return;
        // 这里会触发View中的对话框
    }

    [RelayCommand]
    private void Delete(WorldItem? world)
    {
        if (world == null) return;

        var path = Path.Combine(_worldService.GetWorldsDirectory(), world.Folder);
        var result = _worldService.DeleteWorld(path, backupFirst: true);

        if (result)
        {
            StatusText = "删除成功";
            LoadWorlds();
        }
        else
        {
            StatusText = "删除失败";
        }
    }

    [RelayCommand]
    private void OpenFolder(WorldItem? world)
    {
        if (world == null) return;

        var path = Path.Combine(_worldService.GetWorldsDirectory(), world.Folder);
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
    }

    public bool RenameWorld(string folder, string newName)
    {
        var path = Path.Combine(_worldService.GetWorldsDirectory(), folder);
        var result = _worldService.RenameWorld(path, newName);

        if (result)
        {
            LoadWorlds();
        }
        return result;
    }
}