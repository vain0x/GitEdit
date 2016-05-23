using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GitEdit.View
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static void Start(string commandLine)
        {
            var exePath = Environment.GetCommandLineArgs()[0];
            Process.Start(exePath, commandLine);
        }
    }
}
