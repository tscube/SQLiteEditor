using System;
using System.Collections.Generic;
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

namespace Sample1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindow クラスの新しいインスタンスを初期化し、アプリケーションウィンドウの主要なデータコンテキストを設定します。
        /// </summary>
        /// <remarks>このコンストラクタは `DataContext` を `MainVM` の新しいインスタンスに設定し、ウィンドウのコントロールでデータバインディングを有効にします。WPF アプリケーションでメインウィンドウを作成する際にこのコンストラクタを使用します。</remarks>
        public MainWindow()
        {
            // デザイナー生成コードを初期化
            InitializeComponent();

            // DataContext を MainVM の新しいインスタンスに設定
            this.DataContext = new MainVM();
        }

    }
}
