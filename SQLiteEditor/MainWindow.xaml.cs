// ※ コメントは必ず日本語で記述すること

using Microsoft.Win32;
using System;
using System.IO;
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
            if( this.DataContext is MainVM vm )
            {
                // ファイル選択ダイアログの初期ディレクトリを設定する
                var openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = this.GetExistPath( vm.DbFilePath )
                };            

                // ファイル選択ダイアログを開く
                if( openFileDialog.ShowDialog().Value )
                {
                    vm.DbFilePath = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// 指定されたファイルパスまたは存在する親ディレクトリのパスを取得する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>指定されたファイルパスまたは存在する親ディレクトリのパス</returns>
        private string GetExistPath( string filePath )
        {
            if( File.Exists( filePath ) )
            {
                // ファイルが存在する場合はそのまま返す
                return filePath;
            }
            else if( string.IsNullOrEmpty( filePath ) )
            {
                // ファイルパスが空の場合はデスクトップを返す
                return Environment.GetFolderPath( Environment.SpecialFolder.Desktop ); ;
            }
            else
            {
                // ファイルが存在しない場合は存在する親ディレクトリを返す
                string parentDir = Path.GetDirectoryName( filePath );
                while( !Directory.Exists( parentDir ) )
                {
                    parentDir = Directory.GetParent( parentDir ).FullName;

                    // ルートディレクトリまで到達した場合はデスクトップを返す
                    if( string.IsNullOrEmpty( parentDir ) )
                    {
                        parentDir = Environment.GetFolderPath( Environment.SpecialFolder.Desktop ); ;
                        break;
                    }
                }
                return parentDir;
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
                if( File.Exists( vm.DbFilePath ) )
                {
                    vm.Execute();
                }
                else
                {
                    if( string.IsNullOrEmpty( vm.DbFilePath ) )
                    {
                        MessageBox.Show( "DBファイルが指定されていません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error );
                        vm.StatusMessage = "DBファイルが指定されていません。";
                    }
                    else
                    {
                        MessageBox.Show( "指定されたDBファイルが存在しません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error );
                        vm.StatusMessage = "指定されたDBファイルが存在しません。";
                    }
                }
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

            // 新しいウィンドウのSQLエディタを空にする
            newWindow.SqlStmt.Clear();
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
