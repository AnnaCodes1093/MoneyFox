﻿using System;
using System.IO;
using Android.App;
using Android.Runtime;
using MoneyFox.DataLayer;
using MoneyFox.Foundation.Constants;
using NLog;
using NLog.Targets;
using Application = Android.App.Application;

namespace MoneyFox.Droid
{
    [Application]
    public class MainApplication : Application
    {
        private Logger logManager;

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

            InitLogger();

            logManager.Info("Application Started.");
            logManager.Info("App Version: {Version}", new DroidAppInformation().GetVersion());
            
            EfCoreContext.DbPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                             DatabaseConstants.DB_NAME);

            logManager.Debug("Database Path: {dbPath}", EfCoreContext.DbPath);

            base.OnCreate();
        }

        public override void OnTerminate()
        {
            LogManager.Shutdown();
            base.OnTerminate();
        }

        private void InitLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(Path.Combine(GetExternalFilesDir(null).AbsolutePath, "moneyfox.log")),
                AutoFlush = true,
                ArchiveEvery = FileArchivePeriod.Month
            };
            var debugTarget = new DebugTarget("console");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, debugTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
            logManager = LogManager.GetCurrentClassLogger();
        }

        void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            logManager.Fatal(e.Exception);
        }
    }
}