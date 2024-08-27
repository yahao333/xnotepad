﻿using NotesApp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NotesApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.Write("Log opened.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Write("Log closed.");
        }
    }
}
