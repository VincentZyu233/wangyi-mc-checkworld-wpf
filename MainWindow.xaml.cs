using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WangyiMCHelper.ViewModels;

namespace WangyiMCHelper;

public partial class MainWindow : Window
{
    private readonly WorldService _worldService = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        var path = _worldService.GetWorldsDirectory();
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
    }

    private void SortTimeDesc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CurrentSort = SortMode.TimeDesc;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void SortNameAsc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CurrentSort = SortMode.NameAsc;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void SortSizeDesc_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CurrentSort = SortMode.SizeDesc;
            vm.RefreshCommand.Execute(null);
        }
    }

    private void OpenWorldFolder_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedWorld != null)
        {
            var path = vm.SelectedWorld.Folder;
            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
            }
        }
    }

    private void BackupWorld_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedWorld != null)
        {
            _worldService.BackupWorld(vm.SelectedWorld.Folder);
            vm.RefreshCommand.Execute(null);
        }
    }

    private void RenameWorld_Click(object sender, RoutedEventArgs e)
    {
        // TODO: 实现重命名
    }

    private void DeleteWorld_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.SelectedWorld != null)
        {
            _worldService.DeleteWorld(vm.SelectedWorld.Folder);
            vm.RefreshCommand.Execute(null);
        }
    }
}