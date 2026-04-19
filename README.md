# 网易MC存档管理器 (WPF版)

基于 .NET 8 + WPF 构建的网易我的世界电脑版存档管理工具。

## 技术栈

| 技术 | 作用 |
|------|------|
| .NET 8.0 | 运行时 |
| WPF | UI框架 |
| C# | 后端语言 |
| XAML | 界面定义 |
| CommunityToolkit.Mvvm | MVVM框架 |

## 功能

- 列出存档
- 搜索过滤
- 备份ZIP
- 重命名/删除存档

## 构建

```powershell
dotnet build -c Release
```

## 运行

```powershell
dotnet run
```