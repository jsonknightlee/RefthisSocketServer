using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBNParser.Classes
{
    public class SharedDatabaseAccess //: ObservableObject
    {
        public T getDBValue<T>(SqlDataReader row, string name, T def = default(T))
        {
            T value;

            try
            {
                object op = row[name];

                if (op == DBNull.Value)
                {
                    if (def != null)
                    {
                        return def;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    value = (T)op;
                }
            }
            catch
            {
                if (def != null)
                {
                    return def;
                }
                else
                {
                    return default(T);
                }
            }
            return value;

        }

        public T getDBValue<T>(DataRow row, string name, T def = default(T))
        {
            T value;

            try
            {
                object op = row[name];

                if (op == DBNull.Value)
                {
                    if (def != null)
                    {
                        return def;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    value = (T)op;
                }
            }
            catch
            {
                if (def != null)
                {
                    return def;
                }
                else
                {
                    return default(T);
                }
            }
            return value;

        }

        public SqlParameter CreateParameter<T>(string parameterName, SqlDbType dbType, T value)
        {
            SqlParameter temp = new SqlParameter(parameterName, dbType);
            if (value != null)
            {
                temp.Value = value;
            }
            else
            {
                temp.Value = DBNull.Value;
            }
            return temp;
        }

        public void ExecuteNonQuery(string connString, string query, bool isStoredProc = true, List<SqlParameter> Parameters = null)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(connString))
                {
                    Connection.Open();

                    using (SqlCommand Command = new SqlCommand(query, Connection))
                    {
                        Command.CommandTimeout = 1500;

                        Command.CommandType = isStoredProc == true ? CommandType.StoredProcedure : CommandType.Text;
                        if (Parameters != null)
                        {
                            foreach (SqlParameter parm in Parameters)
                            {
                                Command.Parameters.Add(parm);
                            }
                        }

                        Command.Connection = Connection;
                        Command.ExecuteNonQuery();
                    }
                    Connection.Close();
                }
            }
            catch (Exception err)
            {
                Console.Write(err.Message);
                throw;

            }
        }

        public object ExecuteScalar(string connString, string query, bool isStoredProc = true, List<SqlParameter> Parameters = null)
        {
            try
            {
                using (SqlConnection Connection = new SqlConnection(connString))
                {
                    Connection.Open();

                    using (SqlCommand Command = new SqlCommand(query, Connection))
                    {
                        Command.CommandTimeout = 1500;
                        Command.CommandType = isStoredProc == true ? CommandType.StoredProcedure : CommandType.Text;

                        if (Parameters != null)
                        {
                            foreach (SqlParameter parm in Parameters)
                            {
                                Command.Parameters.Add(parm);
                            }
                        }

                        Command.Connection = Connection;
                        object o = Command.ExecuteScalar();
                        Connection.Close();

                        return o;
                    }
                }
            }
            catch (Exception err)
            {

                Console.Write(err.Message);
                throw;
            }


        }

        public DataSet ExecuteQuery(string connString, string query, bool isStoredProc = true, List<SqlParameter> Parameters = null)
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection Connection = new SqlConnection(connString))
                {
                    Connection.Open();

                    using (SqlCommand Command = new SqlCommand(query, Connection))
                    {
                        Command.CommandTimeout = 1500;
                        Command.CommandType = isStoredProc == true ? CommandType.StoredProcedure : CommandType.Text;

                        if (Parameters != null)
                        {
                            foreach (SqlParameter parm in Parameters)
                            {
                                Command.Parameters.Add(parm);
                            }
                        }

                        Command.Connection = Connection;

                        using (SqlDataAdapter adp = new SqlDataAdapter())
                        {
                            adp.SelectCommand = Command;
                            adp.Fill(ds);
                        }
                    }
                    Connection.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);

                throw;
            }

            return ds;
        }

        public bool DatabaseExists(string connect)
        {
            bool exists = false;

            try
            {
                using (SqlConnection con = new SqlConnection(connect))
                {
                    con.Open();

                    con.Close();
                    exists = true;
                }
            }
            catch
            {
                return false;
            }

            return exists;
        }

        private string GetAllExceptionMsgsFromException(Exception ex)
        {
            string msg = "Errors:" + Environment.NewLine;
            msg += ex.Message;

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += ex.Message + Environment.NewLine;
            }

            return msg;
        }

    //    public void LogEvent(Int32? appId, Int32? dataId, Int32? customerId, Object sender, int level, String name, Boolean notifyGaffey = false, Boolean notifyCustomer = false, string connString = "")
    //    {
    //        Event e = new Event();
    //        e.ApplicationID = appId;
    //        e.CreatedDate = DateTime.Now;
    //        e.DatabaseDetailID = dataId;
    //        e.CustomerID = customerId;
    //        e.EventName = name;
    //        e.Sender = sender;
    //        e.LevelValue = level.ToString();
    //        e.isReviewed = false;
    //        e.GaffeyNotification = notifyGaffey;
    //        e.CustomerNotification = notifyCustomer;
    //
    //        InsertEvent_ASManager(e, connString);
    //    }

    //    public Event InsertEvent_ASManager(Event newEvent, string connString)
    //    {
    //        try
    //        {
    //            const string query = "INSERT INTO [Process].[Event](ApplicationID, DatabaseDetailID, EventName, LevelValue, CreatedDate, isReviewed, ReviewedDate, CustomerID, GaffeyNotification, CustomerNotification, AdditionalInformation)" +
    //                    "output INSERTED.EventID VALUES(@ApplicationID, @DatabaseDetailID, @EventName, @LevelValue, @CreatedDate, @isReviewed, @ReviewedDate, @CustomerID, @GaffeyNotification, @CustomerNotification, @AdditionalInformation)";
    //
    //            List<SqlParameter> parms = new List<SqlParameter>();
    //
    //            parms.Add(CreateParameter("@ApplicationID", System.Data.SqlDbType.Int, newEvent.ApplicationID));
    //            parms.Add(CreateParameter("@DatabaseDetailID", System.Data.SqlDbType.Int, newEvent.DatabaseDetailID));
    //            parms.Add(CreateParameter("@EventName", System.Data.SqlDbType.VarChar, newEvent.EventName));
    //            parms.Add(CreateParameter("@LevelValue", System.Data.SqlDbType.VarChar, newEvent.LevelValue));
    //            parms.Add(CreateParameter("@CreatedDate", System.Data.SqlDbType.DateTime, newEvent.CreatedDate));
    //            parms.Add(CreateParameter("@isReviewed", System.Data.SqlDbType.Bit, newEvent.isReviewed));
    //            parms.Add(CreateParameter("@ReviewedDate", System.Data.SqlDbType.DateTime, newEvent.ReviewedDate));
    //            parms.Add(CreateParameter("@CustomerID", System.Data.SqlDbType.Int, newEvent.CustomerID));
    //            parms.Add(CreateParameter("@GaffeyNotification", System.Data.SqlDbType.VarChar, newEvent.GaffeyNotification));
    //            parms.Add(CreateParameter("@CustomerNotification", System.Data.SqlDbType.Bit, newEvent.CustomerNotification));
    //            parms.Add(CreateParameter("@AdditionalInformation", System.Data.SqlDbType.VarChar, newEvent.AdditionalInformation));
    //
    //            object result = ExecuteScalar(connString, query, false, parms);
    //
    //            if (result != null) newEvent.EventID = Int32.Parse(result.ToString());
    //        }
    //        catch (Exception ex)
    //        {
    //            // TODO: LOG ERROR
    //            Console.WriteLine(ex.Message);
    //        }
    //
    //        return newEvent;
    //    }
    }

}


