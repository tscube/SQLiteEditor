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
            string profileRoot = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "JITProfiles");
            if (!System.IO.Directory.Exists(profileRoot))
            {
                System.IO.Directory.CreateDirectory(profileRoot);
            }
            System.Runtime.ProfileOptimization.SetProfileRoot(profileRoot);
            System.Runtime.ProfileOptimization.StartProfile("Sample1.jitprofile");
            base.OnStartup(e);
        }
    }
}
