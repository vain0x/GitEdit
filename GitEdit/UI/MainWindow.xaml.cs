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
using GitEdit.Properties;
using GitEdit.UI.Editors;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;

namespace GitEdit.UI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
        : Window
    {
        sealed class SaveFileChooser
            : ISaveFileChooser
        {
            MainWindow Window { get; }

            public FileInfo GetSaveFileOrNull()
            {
                var sfd = new SaveFileDialog();
                var result = sfd.ShowDialog(Window);
                return
                    result.HasValue && result.Value
                    ? new FileInfo(sfd.FileName)
                    : null;
            }

            public SaveFileChooser(MainWindow window)
            {
                Window = window;
            }
        }

        public new MainWindowViewModel DataContext =>
            (MainWindowViewModel)base.DataContext;

        Rect Rect
        {
            get
            {
                return new Rect(Left, Top, Width, Height);
            }
            set
            {
                Left = value.Left;
                Top = value.Top;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var saveFileChooser = new SaveFileChooser(this);
            var dataContext = new MainWindowViewModel(saveFileChooser);
            base.DataContext = dataContext;

            dataContext.QuitRequested += (sender, e) => Close();

            dataContext.Editor.FileLoadRequested += (sender, e) => editor.LoadFile(e);
            dataContext.Editor.FileSaveRequested += (sender, e) => editor.SaveFile(e);

            Rect = Settings.Default.MainWindowRect;

            editor.Focus();
        }

        void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DataContext.TrySave();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Open the given file.
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var file = new FileInfo(args[1]);
                if (file.Exists) DataContext.OpenFile(file);
            }
        }

        void OnDrop(object sender, DragEventArgs e)
        {
            var files =
                ((string[])e.Data.GetData(DataFormats.FileDrop))
                .Select(path => new FileInfo(path))
                .ToArray();

            if (files.Length == 1 && !editor.IsModified)
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

        void OnClosed(object sender, EventArgs e)
        {
            var settings = Settings.Default;
            settings.MainWindowRect = Rect;
            settings.Save();
        }

        void OnEditorFileNameChanged(object sender, EventArgs e)
        {
            DataContext.Editor.OnFileNameChanged();
        }
    }
}
