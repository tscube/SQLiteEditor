using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sample1
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // JIT Profile Optimization を有効にするためのコード
        // JIT Profile を保存するディレクトリをユーザーのドキュメントフォルダに設定
        protected override void OnStartup(StartupEventArgs e)
        {
            string appRoot = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Sample1");
            if (!System.IO.Directory.Exists(appRoot))
            {
                System.IO.Directory.CreateDirectory(appRoot);
            }
            System.Runtime.ProfileOptimization.SetProfileRoot(appRoot);
            System.Runtime.ProfileOptimization.StartProfile("Sample1.jitprofile");
            base.OnStartup(e);
        }
    }
}
