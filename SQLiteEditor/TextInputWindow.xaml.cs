using System.Windows;
using System.Windows.Input;

namespace SQLiteEditor
{
    /// <summary>
    /// TextInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TextInputWindow : Window
    {
        public TextInputWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            this.Input.Focus();
            Keyboard.Focus( this.Input );
        }

        private void Input_PreviewKeyUp( object sender, KeyEventArgs e )
        {
            if( Key.Enter == e.Key )
            {
                this.DialogResult = true;
                this.Close();
            }
            else if( Key.Escape == e.Key )
            {
                this.DialogResult = false;
                this.Close();
            }
            else
            {
                // Do nothing
            }
        }

    }
}
