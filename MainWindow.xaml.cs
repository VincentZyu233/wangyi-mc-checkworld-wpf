using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WangyiMCHelper;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        var path = WorldService.GetWorldsDirectory();
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
    }

    private void SortTimeDesc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.SortBy = "Time";
            vm.SortDescending = true;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void SortNameAsc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.SortBy = "Name";
            vm.SortDescending = false;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void SortSizeDesc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.SortBy = "Size";
            vm.SortDescending = true;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void OpenWorldFolder_Click(object sender, RoutedEventArgs e)
    {
        // 实现打开单个存档文件夹
    }

    private void BackupWorld_Click(object sender, RoutedEventArgs e)
    {
        // 实现备份存档
    }

    private void RenameWorld_Click(object sender, RoutedEventArgs e)
    {
        // 实现重命名存档
    }

    private void DeleteWorld_Click(object sender, RoutedEventArgs e)
    {
        // 实现删除存档
    }
}