using Microsoft.Isam.Esent.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsentSample.Crud
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseRepository repo = new DatabaseRepository();
            repo.CreateInstance();
            repo.CreateDatabase();

            repo.AddToDo(new ToDo() { Id = Guid.NewGuid(), Note = "Something... Something..." });
            repo.AddToDo(new ToDo() { Id = Guid.NewGuid(), Note = "Something... Something..." });

            foreach (var toDo in repo.GetAllToDos())
            {
                Console.WriteLine($"ToDo Found: {toDo.Id}: {toDo.Note}");
            }
        }
    }

    public class ToDo
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
    }

    public class DatabaseRepository
    {
        private Instance _instance;
        private string _instancePath;
        private string _databasePath;
        private const string DatabaseName = "Database";

        public void CreateInstance()
        {
            // DatabaseName is "Database"
            _instancePath = Path.Combine(Directory.GetCurrentDirectory(), DatabaseName);
            _databasePath = Path.Combine(_instancePath, "database.edb");
            _instance = new Instance(_databasePath);

            // configure instance
            _instance.Parameters.CreatePathIfNotExist = true;
            _instance.Parameters.TempDirectory = Path.Combine(_instancePath, "temp");
            _instance.Parameters.SystemDirectory = Path.Combine(_instancePath, "system");
            _instance.Parameters.LogFileDirectory = Path.Combine(_instancePath, "logs");
            _instance.Parameters.Recovery = true;
            _instance.Parameters.CircularLog = true;

            _instance.Init();
        }

        public void CreateDatabase()
        {
            using (var session = new Session(_instance))
            {
                // create database file
                JET_DBID database;
                Api.JetCreateDatabase(session, _databasePath, null, out database, CreateDatabaseGrbit.OverwriteExisting);

                // create database schema
                using (var transaction = new Transaction(session))
                {
                    JET_TABLEID tableid;
                    Api.JetCreateTable(session, database, "ToDos", 1, 100, out tableid);

                    // ID
                    JET_COLUMNID columnid;
                    Api.JetAddColumn(session, tableid, "Id",
                                            new JET_COLUMNDEF
                                            {
                                                cbMax = 16,
                                                coltyp = JET_coltyp.Binary,
                                                grbit = ColumndefGrbit.ColumnFixed | ColumndefGrbit.ColumnNotNULL
                                            }, null, 0, out columnid);
                    // Note
                    Api.JetAddColumn(session, tableid, "Note",
                                     new JET_COLUMNDEF
                                     {
                                         coltyp = JET_coltyp.LongText,
                                         cp = JET_CP.Unicode,
                                         grbit = ColumndefGrbit.None
                                     }, null, 0, out columnid);

                    // Define table indices
                    var indexDef = "+Id\0\0";
                    Api.JetCreateIndex(session, tableid, "id_index", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                    transaction.Commit(CommitTransactionGrbit.None);
                }

                Api.JetCloseDatabase(session, database, CloseDatabaseGrbit.None);
                Api.JetDetachDatabase(session, _databasePath);
            }
        }

        public void AddToDo(ToDo toDo)
        {
            ExecuteInTransaction((session, table) =>
            {
                using (var updater = new Update(session, table, JET_prep.Insert))
                {
                    var columnId = Api.GetTableColumnid(session, table, "Id");
                    Api.SetColumn(session, table, columnId, toDo.Id);

                    var columnDesc = Api.GetTableColumnid(session, table, "Note");
                    Api.SetColumn(session, table, columnDesc, toDo.Note, Encoding.Unicode);

                    updater.Save();
                }
                return null;
            });
        }

        private IList<ToDo> ExecuteInTransaction(Func<Session, Table, IList<ToDo>> dataFunc)
        {
            IList<ToDo> results;
            using (var session = new Session(_instance))
            {
                JET_DBID dbid;
                Api.JetAttachDatabase(session, _databasePath, AttachDatabaseGrbit.None);
                Api.JetOpenDatabase(session, _databasePath, String.Empty, out dbid, OpenDatabaseGrbit.None);
                using (var transaction = new Transaction(session))
                {
                    using (var table = new Table(session, dbid, "ToDos", OpenTableGrbit.None))
                    {
                        results = dataFunc(session, table);
                    }

                    transaction.Commit(CommitTransactionGrbit.None);
                }
            }

            return results;
        }


        private ToDo GetToDo(Session session, Table table)
        {
            var toDo = new ToDo();

            var columnId = Api.GetTableColumnid(session, table, "Id");
            toDo.Id = Api.RetrieveColumnAsGuid(session, table, columnId) ?? Guid.Empty;

            var columnDesc = Api.GetTableColumnid(session, table, "Note");
            toDo.Note = Api.RetrieveColumnAsString(session, table, columnDesc, Encoding.Unicode);

            return toDo;
        }

        public IList<ToDo> GetAllToDos()
        {
            return ExecuteInTransaction((session, table) =>
            {
                IList<ToDo> results = new List<ToDo>();
                if (Api.TryMoveFirst(session, table))
                {
                    do
                    {
                        results.Add(GetToDo(session, table));
                    }
                    while (Api.TryMoveNext(session, table));
                }
                return results;
            });
        }

        public IList<ToDo> GetToDosById(Guid id)
        {
            return ExecuteInTransaction((session, table) =>
            {
                IList<ToDo> results = new List<ToDo>();
                Api.JetSetCurrentIndex(session, table, null);
                Api.MakeKey(session, table, id, MakeKeyGrbit.NewKey);
                if (Api.TrySeek(session, table, SeekGrbit.SeekEQ))
                {
                    results.Add(GetToDo(session, table));
                }
                return results;
            });
        }

        public void Delete(Guid id)
        {
            ExecuteInTransaction((session, table) =>
            {
                Api.JetSetCurrentIndex(session, table, null);
                Api.MakeKey(session, table, id, MakeKeyGrbit.NewKey);
                if (Api.TrySeek(session, table, SeekGrbit.SeekEQ))
                {
                    Api.JetDelete(session, table);
                }
                return null;
            });
        }
    }
}
