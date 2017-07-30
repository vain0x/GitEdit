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
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;

namespace GitEdit.UI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
        : Window
    {
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

        void LoadInitialFile()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var file = new FileInfo(args[1]);

                try
                {
                    editor.LoadFile(file);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Couldn't load file: '{file.FullName}'.", App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void OnSaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DataContext.TrySave();
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

        public MainWindow()
        {
            InitializeComponent();

            var document = new TextDocument();
            editor.Document = document;

            var saveFileChooser = new GuiSaveFileChooser(this);
            var dataContext = new MainWindowViewModel(document, saveFileChooser);
            base.DataContext = dataContext;

            dataContext.QuitRequested += (sender, e) => Close();

            dataContext.Editor.FileLoadRequested += (sender, e) => editor.LoadFile(e);
            dataContext.Editor.FileSaveRequested += (sender, e) => editor.SaveFile(e);

            Rect = Settings.Default.MainWindowRect;

            editor.Focus();

            LoadInitialFile();
        }
    }
}
