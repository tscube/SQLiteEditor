using System;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Windows;

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
                this.AppTitle = Assembly.GetExecutingAssembly().GetName().Name + " [" + ( string.IsNullOrEmpty( this.m_DbFilePath ) ? "DBファイル未指定" : this.m_DbFilePath ) + "]";
                this.RaisePropertyChanged( nameof( this.AppTitle ) ); 
            } 
        }

        public string Password { get; set; } = string.Empty;

        public string SqlStmt { get; set; } = string.Empty;

        public DataView DataList { get; private set; } = new DataView();

        public void Execute()
        {
            if( !string.IsNullOrEmpty( this.DbFilePath ) && !string.IsNullOrEmpty( this.SqlStmt ) )
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
                        using( var cmd = new SQLiteCommand( this.SqlStmt, conn ) )
                        {
                            using( var adapter = new SQLiteDataAdapter( cmd ) )
                            {
                                DataTable table = new DataTable();
                                adapter.Fill( table );
                                this.DataList = table.DefaultView;
                                this.RaisePropertyChanged( nameof( this.DataList ) );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        MessageBox.Show( ex.Message, Assembly.GetExecutingAssembly().GetName().Name );
                    }
                }
            }
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
