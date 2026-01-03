using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SQLiteEditor
{
    public partial class MainWindow : Window
    {
        public static int WindowCount = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            /* ウインドウ数カウントアップ */
            MainWindow.WindowCount++;

            InitializeComponent();

            /* 設定読み込み */
            var vm = new MainVM();
            vm.Load();
            this.DataContext = vm;
        }

        /// <summary>
        /// ウインドウ起動時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            this.PasswordMenu.IsChecked = !string.IsNullOrEmpty( Properties.Settings.Default.Password );

            this.SqlStmt.Focus();
            Keyboard.Focus( this.SqlStmt );
        }

        /// <summary>
        /// ウインドウ終了時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {

            if( 0 >= --MainWindow.WindowCount )
            {
                /* 最後のウインドウが閉じられた時 */

                /* 設定保存 */
                if( this.DataContext is MainVM vm )
                {
                    vm.Save();
                }

                /* アプリケーション終了 */
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 終了メニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shutdown_Click( object sender, RoutedEventArgs e )
        {
            this.Close();
        }

        /// <summary>
        /// DBファイル選択メニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFilePath_Click( object sender, RoutedEventArgs e )
        {
            // ファイル選択ダイアログを開く
            var openFileDialog = new OpenFileDialog();
            if( openFileDialog.ShowDialog().Value )
            {
                if( this.DataContext is MainVM vm )
                {
                    vm.DbFilePath = openFileDialog.FileName;
                }
            }

        }

        /// <summary>
        /// DBファイルパスクリアメニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFilePath_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.DbFilePath = string.Empty;
            }
        }

        /// <summary>
        /// DBパスワード設定メニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_Click( object sender, RoutedEventArgs e )
        {
            var inputWindow = new TextInputWindow();
            inputWindow.Owner = this;
            inputWindow.Input.Password = Properties.Settings.Default.Password;
            if( inputWindow.ShowDialog().Value )
            {
                Properties.Settings.Default.Password = inputWindow.Input.Password;
                Properties.Settings.Default.Save();

                this.PasswordMenu.IsChecked = !string.IsNullOrEmpty( Properties.Settings.Default.Password );
            }
        }

        /// <summary>
        /// SQL実行ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execute_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute();
            }
        }

        /// <summary>
        /// テーブル一覧表示メニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTableList_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute( "select tbl_name from sqlite_master where type = 'table' and tbl_name not like 'sqlite_%' order by tbl_name;" );
            }
        }
        private void ShowTableDefinitions_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute( "select tbl_name, sql from sqlite_master where type in ( 'table', 'index' ) and tbl_name not like 'sqlite_%' order by tbl_name;" );
            }
        }

        private void ShowTriggerList_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute( "select tbl_name, name from sqlite_master where type = 'trigger' and tbl_name not like 'sqlite_%' order by tbl_name, name;" );
            }
        }

        private void ShowTriggerDefinitions_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute( "select tbl_name, name, sql from sqlite_master where type = 'trigger' and tbl_name not like 'sqlite_%' order by tbl_name, name;" );
            }
        }

        /// <summary>
        /// 新規ウィンドウメニュークリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewWindow_Click( object sender, RoutedEventArgs e )
        {
            // 新しいウィンドウを開く
            var newWindow = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = this.Left + 30,
                Top  = this.Top  + 30
            };

            newWindow.Show();
        }

        /// <summary>
        /// SQLエディタ入力時のタブサイズ指定
        /// </summary>
        public int TabSize { get; set; } = 4;

        /// <summary>
        /// SQLエディタの特定キー押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SqlStmt_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if( Key.Enter == e.Key && Keyboard.Modifiers.HasFlag( ModifierKeys.Control ) )
            {
                /* Ctrl + Enter */
                if( this.DataContext is MainVM vm )
                {
                    vm.Execute();
                }
                e.Handled = true;
            }
            else if( Key.Tab == e.Key )
            {
                /* Tab */
                var textBox = (TextBox)sender;
                int caret = textBox.CaretIndex;

                int lineIndex = textBox.GetLineIndexFromCharacterIndex(caret);
                int lineStart = textBox.GetCharacterIndexFromLineIndex(lineIndex);
                int column = caret - lineStart;
                int spaces = TabSize - (column % TabSize);
                if( spaces == 0 ) { spaces = TabSize; }

                textBox.Text = textBox.Text.Insert( caret, new string( ' ', spaces ) );
                textBox.CaretIndex = caret + spaces;

                e.Handled = true;
            }
            else
            {
                // Do nothing
            }
        }

    }
}
