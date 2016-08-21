using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GitEdit.ViewModel;
using GitEdit.View;

namespace GitEdit
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        #region Syntax highlighting definitions
        void RegisterSyntaxHighlightings()
        {
            foreach (var def in Constant.SyntaxDefinitions)
            {
                var name = def.Item1;
                var path = string.Format("GitEdit.Resource.SyntaxHighlighting.{0}.xshd", name);

                using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
                {
                    MyTextEditor.RegisterSyntaxHighlightDefinition(name, stream, def.Item2);
                }
            }
        }
        #endregion

        public App()
        {
            RegisterSyntaxHighlightings();
        }
    }
}
