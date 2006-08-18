//
// Mono.Data.MySql.MySqlTransaction.cs
//
// Author:
//   Rodrigo Moya (rodrigo@ximian.com)
//   Daniel Morgan (danmorg@sc.rr.com)
//
// (C) Ximian, Inc. 2002
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Data;
using System.Data.Common;

namespace Mono.Data.MySql
{
	/// <summary>
	/// Represents a transaction to be performed on a SQL database.
	/// </summary>
	public sealed class MySqlTransaction : MarshalByRefObject,
		IDbTransaction, IDisposable
	{
		#region Fields

		private bool doingTransaction = false;
		private MySqlConnection conn = null;
		private IsolationLevel isolationLevel =	
			IsolationLevel.ReadCommitted;
		// FIXME: What is the default isolation level for MySQL?
		
		private bool disposed = false;

		#endregion
               
		#region Public Methods

		[MonoTODO]
		public void Commit ()
		{
			if(doingTransaction == false)
				throw new InvalidOperationException(
					"Begin transaction was not " +
					"done earlier " +
					"thus PostgreSQL can not " +
					"Commit transaction.");
			
			MySqlCommand cmd = new MySqlCommand("COMMIT", conn);
			cmd.ExecuteNonQuery();
						
			doingTransaction = false;
		}		

		[MonoTODO]
		public void Rollback()
		{
			if(doingTransaction == false)
				throw new InvalidOperationException(
					"Begin transaction was not " +
					"done earlier " +
					"thus PostgreSQL can not " +
					"Rollback transaction.");
			
			MySqlCommand cmd = new MySqlCommand("ROLLBACK", conn);
			cmd.ExecuteNonQuery();
						
			doingTransaction = false;
		}

		// For MySQL, Rollback(string) will not be implemented
		// because MySQL does not support Savepoints
		[Obsolete]
		public void Rollback(string transactionName) {
			// throw new NotImplementedException ();
			Rollback();
		}

		// For MySQL, Save(string) will not be implemented
		// because MySQL does not support Savepoints
		[Obsolete]
		public void Save (string savePointName) {
			// throw new NotImplementedException ();
		}

		#endregion // Public Methods

		#region Internal Methods to Mono.Data.MySql Assembly

		internal void Begin()
		{
			if(doingTransaction == true)
				throw new InvalidOperationException(
					"Transaction has begun " +
					"and MySQL does not " +
					"support nested transactions.");
			
			MySqlCommand cmd = new MySqlCommand("BEGIN", conn);
			cmd.ExecuteNonQuery();
						
			doingTransaction = true;
		}

		internal void SetIsolationLevel(IsolationLevel isoLevel)
		{
			String sSql = "SET TRANSACTION ISOLATION LEVEL ";
 
			switch (isoLevel) {
			case IsolationLevel.ReadCommitted:
				sSql += "READ COMMITTED";
				break;
			case IsolationLevel.ReadUncommitted:
				sSql += "READ UNCOMMITTED";
				break;
			case IsolationLevel.RepeatableRead:
				sSql += "REPEATABLE READ";
				break;
			case IsolationLevel.Serializable:
				sSql += "SERIALIZABLE";
				break;
			default:
				// generate exception here for anything else
				break;
			}
			MySqlCommand cmd = new MySqlCommand(sSql, conn);
			cmd.ExecuteNonQuery();

			this.isolationLevel = isoLevel;
		}

		internal void SetConnection(MySqlConnection connection)
		{
			this.conn = connection;
		}

		#endregion // Internal Methods to System.Data.dll Assembly

		#region Properties

		IDbConnection IDbTransaction.Connection	{
			get { 
				return Connection; 
			}
		}

		public MySqlConnection Connection	{
			get { 
				return conn; 
			}
		}

		public IsolationLevel IsolationLevel {
			get { 
				return isolationLevel; 
			}
		}

		internal bool DoingTransaction {
			get {
				return doingTransaction;
			}
		}

		#endregion Properties

		#region Destructors

		// Destructors aka Finalize and Dispose

		private void Dispose(bool disposing) {
			if(!this.disposed) {
				if(disposing) {
					// release any managed resources
					conn = null;
				}
				// release any unmanaged resources

				// close any handles									
				this.disposed = true;
			}
		}

		void IDisposable.Dispose() {
			Dispose(true);
		}

		// aka Finalize
		~MySqlTransaction() {
			Dispose (false);
		}
		#endregion // Destructors

	}
}