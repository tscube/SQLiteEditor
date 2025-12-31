using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SQLiteEditor
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // JIT Profile Optimization を有効にするためのコード
        // JIT Profile を保存するディレクトリをユーザーのドキュメントフォルダに設定
        protected override void OnStartup( StartupEventArgs e )
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            string appRoot = System.IO.Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName);
            if( !System.IO.Directory.Exists( appRoot ) )
            {
                System.IO.Directory.CreateDirectory( appRoot );
            }
            System.Runtime.ProfileOptimization.SetProfileRoot( appRoot );
            System.Runtime.ProfileOptimization.StartProfile( appName + ".jitprofile" );
            base.OnStartup( e );
        }
    }
}
