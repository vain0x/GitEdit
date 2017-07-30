using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace GitEdit.UI
{
    public interface ISaveFileChooser
    {
        FileInfo GetSaveFileOrNull();
    }

    public sealed class GuiSaveFileChooser
        : ISaveFileChooser
    {
        Window Window { get; }

        public FileInfo GetSaveFileOrNull()
        {
            var sfd = new SaveFileDialog();
            var result = sfd.ShowDialog(Window);
            return
                result.HasValue && result.Value
                ? new FileInfo(sfd.FileName)
                : null;
        }

        public GuiSaveFileChooser(Window window)
        {
            Window = window;
        }
    }
}
