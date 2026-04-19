using System;
using System.IO;
using System.Windows;
using Serilog;

namespace WangyiMCHelper;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // 配置日志
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WangyiMCHelper",
            "logs",
            "app_.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        Log.Information("应用启动");

        // 全局异常处理
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            Log.Fatal(args.ExceptionObject as Exception, "未处理的异常");
            Log.CloseAndFlush();
        };

        DispatcherUnhandledException += (s, args) =>
        {
            Log.Error(args.Exception, "UI线程异常");
            args.Handled = true;
        };

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("应用退出");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}