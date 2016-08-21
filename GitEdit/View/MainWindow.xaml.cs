using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit;
using GitEdit.Properties;
using GitEdit.ViewModel;

namespace GitEdit.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
        : Window
        , IMainWindow
    {
        public new MainWindowViewModel DataContext =>
            (MainWindowViewModel)base.DataContext;

        public MainWindow()
        {
            InitializeComponent();

            base.DataContext = new MainWindowViewModel(this);

            _editor.ShowLineNumbers = true;
            _editor.WordWrap = true;
            _editor.Focus();
        }

        public ITextEditor Editor =>
            _editor;

        public void Quit()
        {
            Close();
        }

        #region Save
        FileInfo IMainWindow.GetSaveFileOrNull()
        {
            var sfd = new SaveFileDialog();
            var result = sfd.ShowDialog(this);
            return
                result.HasValue && result.Value
                ? new FileInfo(sfd.FileName)
                : null;
        }
        
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DataContext.Save();
        }
        #endregion

        private void _mainWindow_Drop(object sender, DragEventArgs e)
        {
            var files =
                ((string[])e.Data.GetData(DataFormats.FileDrop))
                .Select(path => new FileInfo(path))
                .ToArray();

            if (files.Length == 1 && Editor.Document.UndoStack.IsOriginalFile)
            {
                DataContext.OpenFile(files[0]);
            }
            else
            {
                // Start new instances of this program
                foreach (var file in files)
                {
                    var exePath = Environment.GetCommandLineArgs()[0];
                    var commandLine = string.Format("{0}", file.FullName);
                    Process.Start(exePath, commandLine);
                }
            }
        }

        private void _mainWindow_Closed(object sender, EventArgs e)
        {
            var settings = Settings.Default;
            settings.MainWindowRect = new Rect(Left, Top, Width, Height);
            settings.Save();
        }
    }
}
