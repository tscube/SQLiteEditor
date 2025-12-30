using Microsoft.Win32;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SQLiteEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainVM();
            vm.Load();

            this.DataContext = vm;
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            this.SqlStmt.Focus();
            Keyboard.Focus( this.SqlStmt );
        }

        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Save();
            }

            Application.Current.Shutdown();
        }

        private void Shutdown_Click( object sender, RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }

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

        private void ClearFilePath_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.DbFilePath = string.Empty;
            }
        }

        private void Password_Click( object sender, RoutedEventArgs e )
        {
            var inputWindow = new TextInputWindow();
            inputWindow.Owner = this;
            inputWindow.Input.Text = Properties.Settings.Default.Password;
            if( inputWindow.ShowDialog().Value )
            {
                Properties.Settings.Default.Password = inputWindow.Input.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void Execute_Click( object sender, RoutedEventArgs e )
        {
            if( this.DataContext is MainVM vm )
            {
                vm.Execute();
            }
        }

        public int TabSize { get; set; } = 4;

        private void SqlStmt_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if( Key.Enter == e.Key && Keyboard.Modifiers.HasFlag( ModifierKeys.Control ) )
            {
                if( this.DataContext is MainVM vm )
                {
                    vm.Execute();
                }
                e.Handled = true;
            }
            else if( Key.Tab == e.Key )
            {
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
