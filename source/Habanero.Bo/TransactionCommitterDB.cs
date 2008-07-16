//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.Data;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using log4net;

namespace Habanero.BO
{
    /// <summary>
    /// This class manages and commits a collection of ITransactions to a database using SQL.
    /// </summary>
    public class TransactionCommitterDB : TransactionCommitter
    {
        private IDbTransaction _dbTransaction;
        private IDbConnection _dbConnection;
        private static readonly ILog log = LogManager.GetLogger("Habanero.BO.TransactionCommitterDB");

        /// <summary>
        /// Begins the transaction on the appropriate databasource.
        /// </summary>
        protected override void BeginDataSource()
        {
            _dbConnection = DatabaseConnection.CurrentConnection.GetConnection();
            _dbConnection.Open();
            _dbTransaction = _dbConnection.BeginTransaction(IsolationLevel.RepeatableRead);
        }

        /// <summary>
        /// Tries to execute an individual transaction against the datasource.
        /// 1'st phase of a 2 phase database commit.
        /// </summary>
        protected override void ExecuteTransactionToDataSource(ITransactional transaction)
        {
            ITransactionalDB transactionDB = (ITransactionalDB)transaction;

            if (transaction is TransactionalBusinessObjectDB)
            {
                TransactionalBusinessObjectDB transactionBusObjDB = (TransactionalBusinessObjectDB) transaction;
                if (transactionBusObjDB.IsDeleted)
                {
                    DeleteRelatedChildren(transactionBusObjDB);
                    DereferenceRelatedChildren(transactionBusObjDB);
                }
            }

            ISqlStatementCollection sql = transactionDB.GetPersistSql();
            if (sql == null) return;
            DatabaseConnection.CurrentConnection.ExecuteSql(sql, _dbTransaction);
            base.ExecuteTransactionToDataSource(transaction);
        }

        private void DereferenceRelatedChildren(TransactionalBusinessObject transaction)
        {
            foreach (Relationship relationship in transaction.BusinessObject.Relationships)
            {
                if (MustDereferenceRelatedObjects(relationship))
                {
                    IBusinessObjectCollection col = relationship.GetRelatedBusinessObjectCol();
                    for (int i = col.Count - 1; i >= 0; i--)
                    {
                        BusinessObject bo = (BusinessObject) col[i];
                        foreach (RelPropDef relPropDef in relationship.RelationshipDef.RelKeyDef)
                        {
                            bo.SetPropertyValue(relPropDef.RelatedClassPropName, null);
                        }
                        ExecuteTransactionToDataSource(new TransactionalBusinessObjectDB(bo));
                    }
                }
            }
        }

        private static bool MustDereferenceRelatedObjects(Relationship relationship)
        {
            return relationship.DeleteParentAction == DeleteParentAction.DereferenceRelated;
        }

        private void DeleteRelatedChildren(TransactionalBusinessObject transaction)
        {
            foreach (Relationship relationship in transaction.BusinessObject.Relationships)
            {
                if (MustDeleteRelatedObjects(relationship))
                {
                    IBusinessObjectCollection col = relationship.GetRelatedBusinessObjectCol();
                    for (int i = col.Count - 1; i >= 0; i--)
                    {
                        BusinessObject bo = (BusinessObject) col[i];
                        bo.Delete();
                        ExecuteTransactionToDataSource(new TransactionalBusinessObjectDB(bo));
                    }
                }
            }
        }

        private static bool MustDeleteRelatedObjects(Relationship relationship)
        {
            return relationship.DeleteParentAction == DeleteParentAction.DeleteRelated;
        }


        /// <summary>
        /// Commits all the successfully executed statements to the datasource.
        /// 2'nd phase of a 2 phase database commit.
        /// </summary>
        protected override void CommitToDatasource()
        {
            try
            {
                _dbTransaction.Commit();
                _CommittSuccess = true;
            }
            catch (Exception ex)
            {
                log.Error("Error commiting transaction: " + Environment.NewLine +
                    ExceptionUtilities.GetExceptionString(ex, 4, true));
                TryRollback();
                throw;
            }
            finally
            {
                if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// In the event of any errors occuring during executing statements to the datasource 
        /// <see cref="TransactionCommitter.ExecuteTransactionToDataSource"/> or during committing to the datasource
        /// <see cref="TransactionCommitter.CommitToDatasource"/>
        /// </summary>
        protected override void TryRollback()
        {
            try
            {
                if (_dbTransaction != null) _dbTransaction.Rollback();
            }
            catch (Exception ex)
            {
                log.Error("Error rolling back transaction: " + Environment.NewLine +
                    ExceptionUtilities.GetExceptionString(ex, 4, true));
                throw;
            }
            finally
            {
                if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Decorates the business object with a TransactionalBusinessObjectDB.
        /// See <see cref="TransactionCommitter.CreateTransactionalBusinessObject" />
        /// </summary>
        /// <param name="businessObject">The business object to decorate in a TransactionalBusinessObjectDB</param>
        /// <returns>The decorated TransactionalBusinessObjectDB</returns>
        protected override TransactionalBusinessObject CreateTransactionalBusinessObject(
            IBusinessObject businessObject)
        {
            return new TransactionalBusinessObjectDB(businessObject);
        }
    }
}