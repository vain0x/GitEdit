using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GitEdit.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
        : Window
        , View.Model.IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            RegisterSyntaxHighlightings();

            DataContext = _viewModel = new View.Model.MainWindowViewModel(this);
        }

        private View.Model.MainWindowViewModel _viewModel;
        
        public MyTextEditor Editor
        {
            get { return _editor; }
        }

        private void RegisterSyntaxHighlightings()
        {
            foreach (var name in Constant.SyntaxNames)
            {
                AvalonEditUtility.RegisterSyntaxHighlightDefinition(name, new string[] {});
            }
        }

        public void Quit()
        {
            Close();
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(Editor.Document.FileName))
            {
                var sfd = new SaveFileDialog();
                var result = sfd.ShowDialog(this);
                if (result.HasValue && result.Value)
                {
                    var file = new FileInfo(sfd.FileName);
                    _viewModel.SaveFile(file);
                }
            }
            else
            {
                _viewModel.SaveFile(new FileInfo(Editor.Document.FileName));
            }
        }
        
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }
    }
}
