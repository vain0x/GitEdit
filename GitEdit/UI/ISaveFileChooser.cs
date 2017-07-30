using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitEdit.UI
{
    public interface ISaveFileChooser
    {
        FileInfo GetSaveFileOrNull();
    }
}
