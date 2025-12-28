using System.ComponentModel;
using System.Data;
using System.Data.SQLite;

namespace Sample1
{
    internal class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private const string Password = "pass";
        private const string DataSource = "Sample1.db";

        public string SqlStmt { get; set; } = "select * from user where id = 1";

        public DataView DataList { get; private set; } = new DataView();

        public void Load()
        {
            string connectionString = new SQLiteConnectionStringBuilder()
            {
                DataSource = MainVM.DataSource,
                Password = MainVM.Password,
                SyncMode = SynchronizationModes.Off,
                JournalMode = SQLiteJournalModeEnum.Wal,
                BusyTimeout = 3000
            }
            .ToString();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(this.SqlStmt, conn))
                {
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        this.DataList = table.DefaultView;
                        this.RaisePropertyChanged(nameof(this.DataList));
                    }
                }
            }
        }

    }
}
