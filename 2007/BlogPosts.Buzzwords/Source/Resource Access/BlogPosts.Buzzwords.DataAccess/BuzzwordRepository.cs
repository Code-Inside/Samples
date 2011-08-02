using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using BlogPosts.Buzzwords.BusinessEntities;
using System.Data.SqlClient;
using System.Diagnostics;
using BlogPosts.Buzzwords.DataAccess.SQLServer;

namespace BlogPosts.Buzzwords.DataAccess
{
    /// <summary>
    /// Respository that lets you find Buzzword in the
    /// CRM database.
    /// </summary>
    public class BuzzwordRepository : Repository<Buzzword>
    {
        private string databaseName;

        public BuzzwordRepository(string databaseName)
            : base(databaseName)
        {
            this.databaseName = databaseName;
        }


        public Buzzword GetbuzzwordsByid(System.Int32 id)
        {
            ISelectionFactory<System.Int32> selectionFactory = new GetbuzzwordsByidSelectionFactory();

            try
            {
                return base.FindOne(selectionFactory, new GetbuzzwordsByidFactory(), id);
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, selectionFactory);
            }

            return new Buzzword();
        }

        public List<Buzzword> GetAllFrombuzzwords()
        {
            ISelectionFactory<NullableIdentity> selectionFactory = new GetAllFrombuzzwordsSelectionFactory();

            try
            {
                NullableIdentity nullableIdentity = new NullableIdentity();
                return base.Find(selectionFactory, new GetAllFrombuzzwordsFactory(), nullableIdentity);
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, selectionFactory);
            }

            return new List<Buzzword>();
        }

        public void Add(Buzzword buzzword)
        {
            BuzzwordInsertFactory insertFactory = new BuzzwordInsertFactory();
            try
            {
                base.Add(insertFactory, buzzword);
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, insertFactory);
            }
        }

        private void HandleSqlException(SqlException ex, IDbToBusinessEntityNameMapper mapper)
        {
            if (ex.Number == ErrorCodes.SqlUserRaisedError)
            {
                switch (ex.State)
                {
                    case ErrorCodes.ValidationError:
                        string[] messageParts = ex.Errors[0].Message.Split(':');
                        throw new RepositoryValidationException(
                            mapper.MapDbParameterToBusinessEntityProperty(messageParts[0]),
                            messageParts[1], ex);

                    case ErrorCodes.ConcurrencyViolationError:
                        throw new ConcurrencyViolationException(ex.Message, ex);

                }
            }

            throw new RepositoryFailureException(ex);
        }
    }
}

