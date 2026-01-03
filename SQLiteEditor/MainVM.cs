using System;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace SQLiteEditor
{
    internal class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged( string propertyName )
        {
            this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        public string AppTitle { get; set; } = string.Empty;

        public string StatusMessage { get; set; } = string.Empty;

        public string SqlExecuted { get; set; } = string.Empty;

        private string m_DbFilePath = string.Empty;    
        public string DbFilePath
        { 
            get
            {
                return this.m_DbFilePath;
            }
            set
            {
                this.m_DbFilePath = value;
                this.AppTitle = Assembly.GetExecutingAssembly().GetName().Name + " v" + Assembly.GetExecutingAssembly().GetName().Version + " [" + ( string.IsNullOrEmpty( this.m_DbFilePath ) ? "DBファイル未指定" : this.m_DbFilePath ) + "]";
                this.RaisePropertyChanged( nameof( this.AppTitle ) ); 
            } 
        }

        public string SqlStmt { get; set; } = string.Empty;

        public DataView DataList { get; private set; } = new DataView();

        public void Execute( string sqlStmt = "" )
        {
            this.StatusMessage = string.Empty;

            sqlStmt = string.IsNullOrEmpty( sqlStmt ) ? this.SqlStmt : sqlStmt;
            this.SqlExecuted = " \"" + Regex.Replace( sqlStmt, @"\r\n|\r|\n", " " ).Trim() + "\"";

            if( !string.IsNullOrEmpty( this.DbFilePath ) && !string.IsNullOrEmpty( sqlStmt ) )
            {
                string connectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = this.DbFilePath,
                    Password = Properties.Settings.Default.Password,
                    SyncMode = SynchronizationModes.Off,
                    JournalMode = SQLiteJournalModeEnum.Wal,
                    BusyTimeout = 3000
                }
                .ToString();

                using( var conn = new SQLiteConnection( connectionString ) )
                {
                    try
                    {
                        conn.Open();
                        using( var cmd = new SQLiteCommand( sqlStmt, conn ) )
                        {
                            bool isQuery = false;
                            using( var reader = cmd.ExecuteReader() )
                            {
                                isQuery = reader.HasRows;
                            }

                            if( isQuery )
                            {
                                /* SQLがクエリーステートメントの場合 */
                                using( var adapter = new SQLiteDataAdapter( cmd ) )
                                {
                                    DataTable table = new DataTable();
                                    int result = adapter.Fill( table );
                                    this.StatusMessage = $"取得件数: {result} 件";
 
                                    this.DataList = table.DefaultView;
                                    this.RaisePropertyChanged( nameof( this.DataList ) );
                                }
                            }
                            else
                            {
                                /* SQLが非クエリーステートメントの場合 */
                                int result = cmd.ExecuteNonQuery();
                                this.StatusMessage = $"影響件数: {result} 件";
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        MessageBox.Show( ex.Message, Assembly.GetExecutingAssembly().GetName().Name );
                    }
                }
            }

            this.RaisePropertyChanged( nameof( this.StatusMessage ) );
            this.RaisePropertyChanged( nameof( this.SqlExecuted ) );
        }

        public void Load()
        {
            this.DbFilePath = Properties.Settings.Default.DbFilePath;

            this.SqlStmt = Properties.Settings.Default.SqlStmt;
            this.RaisePropertyChanged( nameof( this.SqlStmt ) );
        }

        public void Save()
        {
            Properties.Settings.Default.DbFilePath = this.DbFilePath;
            Properties.Settings.Default.SqlStmt = this.SqlStmt;
            Properties.Settings.Default.Save();
        }

    }
}
