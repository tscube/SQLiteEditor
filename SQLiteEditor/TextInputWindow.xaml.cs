using System.Windows;
using System.Windows.Input;

namespace SQLiteEditor
{
    /// <summary>
    /// TextInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TextInputWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextInputWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ウインドウ起動時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            this.Input.Focus();
            Keyboard.Focus( this.Input );
        }

        /// <summary>
        /// Inputコントロールのキーアップイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_PreviewKeyUp( object sender, KeyEventArgs e )
        {
            if( Key.Enter == e.Key )
            {
                /*  OKボタンと同じ処理 */
                this.DialogResult = true;
                this.Close();
            }
            else if( Key.Escape == e.Key )
            {
                /*  キャンセルボタンと同じ処理 */
                this.DialogResult = false;
                this.Close();
            }
            else
            {
                // Do nothing
            }
        }

        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
