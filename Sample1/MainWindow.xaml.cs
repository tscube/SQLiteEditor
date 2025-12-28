using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public int TabSize { get; set; } = 4;

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var textBox = (TextBox)sender;

                int caret = textBox.CaretIndex;

                // 現在の行頭からの位置を計算
                int lineIndex = textBox.GetLineIndexFromCharacterIndex(caret);
                int lineStart = textBox.GetCharacterIndexFromLineIndex(lineIndex);
                int column = caret - lineStart;

                // 次のタブ位置までのスペース数
                int spaces = TabSize - (column % TabSize);
                if (spaces == 0) spaces = TabSize;

                textBox.Text = textBox.Text.Insert(caret, new string(' ', spaces));
                textBox.CaretIndex = caret + spaces;

                e.Handled = true; // 既定の Tab 動作を止める
            }
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            if ( this.DataContext is MainVM vm)
            {
                vm.Load();
            }
        }

    }
}
