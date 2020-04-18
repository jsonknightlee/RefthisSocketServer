using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ISBNParser.Classes
{
    public class DataAccess : SharedDatabaseAccess
    {
        //const string connString = "Server=(LocalDB)\\MSSQLLocalDB; Initial Catalog=JobSite; Integrated Security=SSPI"; //Live site
        const string connString = "Server=(LocalDB)\\MSSQLLocalDB; Initial Catalog=Refthis; Integrated Security=SSPI"; //Live site
        //const string connString = "Server=tcp:quvada.database.windows.net,1433;Initial Catalog=refthis_db;Persist Security Info=False;User ID=adminlog;Password=Huntnhustle7;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<ReferenceItem> SaveSingleISBN(IsbnItem item, string isbn, List<ReferenceItem> reference)
        {
            try
            {



                //Check for duplicates, exit early
                string query = "SELECT COUNT(*) FROM [dbo].[Reference] " +
                            " WHERE";

                 query += " ISBN = @ISBN";

                List<SqlParameter> parms = new List<SqlParameter>();
                 parms.Add(CreateParameter("@ISBN", SqlDbType.NVarChar, item.ISBN));
                // parms.Add(CreateParameter("@StoreName", SqlDbType.VarChar, product.StoreName));
                // parms.Add(CreateParameter("@Price", SqlDbType.VarChar, product.Price));
                // if (checkPostDate) parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, product.PostDate));
                object result = ExecuteScalar(connString, query, false, parms);

                int count = 1;
                if (result != null) int.TryParse(result.ToString(), out count);
                if (count > 0)
                {
                    /* Disabled 
                    //Check for duplicates, exit early
                    query = "SELECT * FROM [dbo].[Reference] " +
                                " WHERE";

                    query += " ISBN = @ISBN";

                    List<SqlParameter> parms3 = new List<SqlParameter>();
                    parms3.Add(CreateParameter("@ISBN", SqlDbType.NVarChar, item.ISBN));
                    // parms.Add(CreateParameter("@StoreName", SqlDbType.VarChar, product.StoreName));
                    // parms.Add(CreateParameter("@Price", SqlDbType.VarChar, product.Price));
                    // if (checkPostDate) parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, product.PostDate));
                    result = ExecuteScalar(connString, query, false, parms3);
                    */
                    reference = GetReference(isbn);

                    return reference;
                }


                

                List<SqlParameter> parms1 = new List<SqlParameter>();
                parms1 = new List<SqlParameter>();
                parms1.Add(CreateParameter("@ISBN", SqlDbType.NVarChar, item.ISBN));
                parms1.Add(CreateParameter("@UserID", SqlDbType.NVarChar, item.UserID));

                string mainQuery = "INSERT INTO [dbo].[Reference] " + Environment.NewLine +
                  //"  ([ID] " + Environment.NewLine +
                  "  ([UserID] " + Environment.NewLine +
                  "   ,[ISBN]) " + Environment.NewLine +
                  "VALUES " +
                  "  (@UserID " + Environment.NewLine +             
                  "  ,@ISBN)";



                //            //If not in DB, add.
                //            string mainQuery = "INSERT INTO [dbo].[ISBN] " + Environment.NewLine +
                //                //"  ([ID] " + Environment.NewLine +
                //                "  ([ISBN] " + Environment.NewLine +
                //                "  ,[titleID] " + Environment.NewLine +
                //                "  ,[coverID] " + Environment.NewLine +
                //                "  ,[bookID] " + Environment.NewLine +
                //                "  ,[authorID] " + Environment.NewLine +
                //                "  ,[refDate] " + Environment.NewLine +
                //                "  ,[BIBtexID] " + Environment.NewLine +
                //                "  ,[ReManID] " + Environment.NewLine +
                //                "  ,[EndNote] " + Environment.NewLine +
                //                "  ,[ReferenceID]) " + Environment.NewLine +
                //                "VALUES " + Environment.NewLine +
                //                // "  (@ID] " + Environment.NewLine +
                //                "  (@number] " + Environment.NewLine +
                //                "  ,[@titleID] " + Environment.NewLine +
                //                "  ,[@coverID] " + Environment.NewLine +
                //                "  ,[@bookID] " + Environment.NewLine +
                //                "  ,[@authorID] " + Environment.NewLine +
                //                "  ,[@refDate] " + Environment.NewLine +
                //                "  ,[@BIBtexID] " + Environment.NewLine +
                //                "  ,[@RefManID] " + Environment.NewLine +
                //                "  ,[@EndNote] " + Environment.NewLine +
                //                "  ,[@ReferenceID])";
                //




                // parms = new List<SqlParameter>();
                // parms.Add(CreateParameter("@number", SqlDbType.Int, item.number));
                // parms.Add(CreateParameter("@titleID", SqlDbType.Int, item.titleID));
                // parms.Add(CreateParameter("@coverID", SqlDbType.VarChar, item.coverID));
                //
                // parms.Add(CreateParameter("@bookID", SqlDbType.VarChar, item.bookID));
                // parms.Add(CreateParameter("@authorID", SqlDbType.Bit, item.authorID));
                // parms.Add(CreateParameter("@refDate", SqlDbType.Int, item.refDate));
                // parms.Add(CreateParameter("@BIBtexID", SqlDbType.Int, item.BIBTexID));
                // parms.Add(CreateParameter("@EndNote", SqlDbType.Int, item.EndNote));
                // parms.Add(CreateParameter("@ReferenceID", SqlDbType.Int, item.ReferenceID));



                ExecuteNonQuery(connString, mainQuery, false, parms1);

                reference = GetReference(isbn);
                return reference;
            }
            catch (Exception e)
            {
                //  RecordDetailedErrorData(e, 0, 1);
                Console.WriteLine("SaveSingleISBN Error" + e.Message);
                return null;
            }
        }

        public List<UserItem> RegisterUser(UserItem item1,string deviceID, List<UserItem> user)
        {

            try
            {



                //Check for duplicates, exit early
                string query = "SELECT COUNT(*) FROM [dbo].[RefUser] " +
                            " WHERE";

                query += " DeviceID = @DeviceID";

                List<SqlParameter> parms = new List<SqlParameter>();
                parms.Add(CreateParameter("@DeviceID", SqlDbType.NVarChar, item1.DeviceID));
                // parms.Add(CreateParameter("@StoreName", SqlDbType.VarChar, product.StoreName));
                // parms.Add(CreateParameter("@Price", SqlDbType.VarChar, product.Price));
                // if (checkPostDate) parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, product.PostDate));
                object result = ExecuteScalar(connString, query, false, parms);
             

                int count = 1;
                if (result != null)
                {
                    int.TryParse(result.ToString(), out count);
                }
                else
                {
                 


                }
                



                   
                if (count > 0)
                {
                    user = GetSavedUser(deviceID);
                    return user;
                }
                else
                {



                    List<SqlParameter> parms1 = new List<SqlParameter>();
                   
                    parms1.Add(CreateParameter("@DeviceID", SqlDbType.NVarChar, item1.DeviceID));
                    parms1.Add(CreateParameter("@UserName", SqlDbType.NVarChar, item1.UserName));
               //     parms1.Add(CreateParameter("@PasswordHash", SqlDbType.VarChar, item1.PasswordHash));
                    parms1.Add(CreateParameter("@Email", SqlDbType.NVarChar, item1.Email));
                    parms1.Add(CreateParameter("@FirstName", SqlDbType.NVarChar, item1.FirstName));
                    parms1.Add(CreateParameter("@LastName", SqlDbType.NVarChar, item1.LastName));

                    //            //If not in DB, add.
                    string mainQuery = "INSERT INTO [dbo].[RefUser] " +
                        //"  ([ID] " + Environment.NewLine +
                        "  ([DeviceID] " +
                        "  ,[UserName] " +
                        "  ,[Email] " + 
               //         "  ,[PasswordHash] " + Environment.NewLine +
                        "  ,[FirstName] " + 
                        "  ,[LastName]) " + 
                        "VALUES " + Environment.NewLine +
                        // "  (@ID] " + Environment.NewLine +
                        "  (@DeviceID " + Environment.NewLine +
                        "  ,@UserName " + Environment.NewLine +
                        "  ,@Email " + Environment.NewLine +
            //            "  ,[@PasswordHash] " + Environment.NewLine +
                        "  ,@FirstName " + Environment.NewLine +
                        "  ,@LastName)";


                    ExecuteNonQuery(connString, mainQuery, false, parms1);



                    user = GetSavedUser(deviceID);
                    return user;

                    /* Disabled

                    // get the newly created user ID
                    //Check for duplicates, exit early
                    string query2 = "SELECT * FROM [dbo].[RefUser] " +
                                " WHERE";

                    query += " DeviceID = @DeviceID";

                    List<SqlParameter> parms2 = new List<SqlParameter>();
                    parms.Add(CreateParameter("@DeviceID", SqlDbType.NVarChar, item1.DeviceID));

                    object result2 = ExecuteScalar(connString, query2, false, parms);

                    if (result2 != null)
                    {
                        return result2.ToString();
                    }

                    else
                    {
                        Console.WriteLine("User not found");
                        return "Fokkol";
                    }

                    */



                }             


            }
            catch (Exception e)
            {
                //  RecordDetailedErrorData(e, 0, 1);
                Console.WriteLine("SaveSingleISBN Error" + e.Message);
                return null;
            }
            
        }

        public List<UserItem> GetSavedUser(string deviceID = "")
        {

            List<UserItem> results = new List<UserItem>();

           string query = "SELECT * FROM [dbo].[RefUser] WHERE DeviceID = @DeviceID";

            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@DeviceID", SqlDbType.NVarChar, deviceID));

            DataSet ds = ExecuteQuery(connString, query, false, parms);

            if (ds.Tables.Count > 0)
            {
                DataRow[] rows = ds.Tables[0].Select();

                foreach (DataRow row in rows)
                {
                    try
                    {
                        UserItem jl = new UserItem();
                        jl.UserID = getDBValue<int>(row, "UserID");
                        jl.DeviceID = getDBValue<string>(row, "DeviceID");
                        jl.UserName = getDBValue<string>(row, "UserName");
                        jl.PasswordHash = getDBValue<string>(row, "PW");
                        jl.FirstName = getDBValue<string>(row, "FirstName");
                        jl.LastName = getDBValue<string>(row, "LastName");
                        jl.Email = getDBValue<string>(row, "Email");
                        jl.InsertDate = getDBValue<DateTime>(row, "InsertDate");
                        results.Add(jl);
                    }
                    catch (Exception ex)
                    {
                        RecordDetailedErrorData(ex, 0, 1);
                    }
                }
                return results;
            }
            return null;
        }



        public List<ReferenceItem> GetReference(string isbn = "")
        {

            List<ReferenceItem> results = new List<ReferenceItem>();

            string query = "SELECT * FROM [dbo].[Reference] WHERE ISBN = @ISBN";

            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@ISBN", SqlDbType.NVarChar, isbn));

            DataSet ds = ExecuteQuery(connString, query, false, parms);

            if (ds.Tables.Count > 0)
            {
                DataRow[] rows = ds.Tables[0].Select();

                foreach (DataRow row in rows)
                {
                    try
                    {


                        ReferenceItem jl = new ReferenceItem();
                        jl.RefID = getDBValue<int>(row, "RefID");
                        jl.Title = getDBValue<string>(row, "Title");
                        jl.Author = getDBValue<string>(row, "Author");
                        jl.ISBN = getDBValue<string>(row, "ISBN");
                        jl.Url = getDBValue<string>(row, "Url");
                        jl.YearPublished = getDBValue<string>(row, "YearPublished");
                        jl.Publisher = getDBValue<string>(row, "Publisher");
                        jl.StudentID = getDBValue<string>(row, "StudentID");
                        jl.InsertDate = getDBValue<DateTime>(row, "InsertDate");
                        results.Add(jl);
                    }
                    catch (Exception ex)
                    {
                        RecordDetailedErrorData(ex, 0, 1);
                    }
                }
                return results;
            }
            return null;
        }



        /*
        public void SaveRaceItem(IsbnItem item, bool checkPostDate = true)
        {

            try
            {
                // Regex test = new Regex("^Email.*");
                // if (j.Contact != null && test.IsMatch(j.Contact))
                // {
                //     j.Contact = j.Contact.Substring(5).Trim();
                // }
                //
                // if (j.Contact != null && j.Contact.Length > 400) j.Contact = j.Contact.Substring(0, 399);
                // if (j.ContactName != null && j.ContactName.Length > 400) j.ContactName = j.ContactName.Substring(0, 399);
                // if (j.Company != null && j.Company.Length > 200) j.Company = j.Company.Substring(0, 199);
                // if (j.Location != null && j.Location.Length > 100) j.Location = j.Location.Substring(0, 99);
                // //   if (j.JobTitle != null && j.JobTitle.Length > 200) j.JobTitle = j.JobTitle.Substring(0, 199);
                // SaveContact(j);


                //Check for duplicates, exit early
                string query = "SELECT COUNT(*) FROM [dbo].[Race] " +
                            " WHERE";

                if (checkPostDate) query += " RaceDetailName = @RaceDetailName";

                List<SqlParameter> parms = new List<SqlParameter>();
                // parms.Add(CreateParameter("@ProductName", SqlDbType.VarChar, product.ProductName));
                // parms.Add(CreateParameter("@StoreName", SqlDbType.VarChar, product.StoreName));
                // parms.Add(CreateParameter("@Price", SqlDbType.VarChar, product.Price));
                // if (checkPostDate) parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, product.PostDate));
                object result = ExecuteScalar(connString, query, false, parms);

                int count = 1;
                if (result != null) int.TryParse(result.ToString(), out count);
                if (count > 0) return;



                //If not in DB, add.
                string mainQuery = "INSERT INTO [dbo].[Product.ProductItem] " + Environment.NewLine +
                    "  ([WebsiteID] " + Environment.NewLine +
                    "  ,[InShopId] " + Environment.NewLine +
                    "  ,[Price] " + Environment.NewLine +
                    "  ,[StoreName] " + Environment.NewLine +
                    "  ,[ProductName] " + Environment.NewLine +
                    "  ,[TagLine] " + Environment.NewLine +
                    "  ,[Brand] " + Environment.NewLine +
                    "  ,[Description] " + Environment.NewLine +
                    "  ,[UnitOfMeasure] " + Environment.NewLine +
                    "  ,[ListingAge] " + Environment.NewLine +
                    "  ,[MainBarCode] " + Environment.NewLine +
                    "  ,[URL] " + Environment.NewLine +
                    "  ,[InsertDate] " + Environment.NewLine +
                    "  ,[PostDate] " + Environment.NewLine +
                    "  ,[isPosted] " + Environment.NewLine +
                    "  ,[SavedAmount] " + Environment.NewLine +
                    "  ,[isSaving] " + Environment.NewLine +
                    "  ,[ProductImage] " + Environment.NewLine +
                    "  ,[ProductImageLg] " + Environment.NewLine +
                    "  ,[PriceNonSpecial] " + Environment.NewLine +
                    "  ,[ProductInstructions] " + Environment.NewLine +
                    "  ,[Comments] " + Environment.NewLine +
                    "  ,[ProductCount] " + Environment.NewLine +
                    "  ,[JobID]) " + Environment.NewLine +
                    "VALUES " + Environment.NewLine +
                    "  (@WebsiteID " + Environment.NewLine +
                    "  ,@InShopID " + Environment.NewLine +
                    "  ,@Price " + Environment.NewLine +
                    "  ,@StoreName " + Environment.NewLine +
                    "  ,@ProductName " + Environment.NewLine +
                    "  ,@TagLine " + Environment.NewLine +
                    "  ,@Brand " + Environment.NewLine +
                    "  ,@Description " + Environment.NewLine +
                    "  ,@UnitOfMeasure " + Environment.NewLine +
                    "  ,@ListingAge " + Environment.NewLine +
                    "  ,@MainBarCode " + Environment.NewLine +
                    "  ,@URL " + Environment.NewLine +
                    "  ,@InsertDate " + Environment.NewLine +
                    "  ,@PostDate " + Environment.NewLine +
                    "  ,@isPosted " + Environment.NewLine +
                    "  ,@SavedAmount " + Environment.NewLine +
                    "  ,@isSaving " + Environment.NewLine +
                    "  ,@ProductImage " + Environment.NewLine +
                    "  ,@ProductImageLg " + Environment.NewLine +
                    "  ,@PriceNonSpecial " + Environment.NewLine +
                    "  ,@ProductInstructions " + Environment.NewLine +
                    "  ,@Comments " + Environment.NewLine +
                    "  ,@ProductCount " + Environment.NewLine +
                    "  ,@JobID)";

                parms = new List<SqlParameter>();
                // parms.Add(CreateParameter("@WebsiteID", SqlDbType.Int, product.WebsiteID));
                // parms.Add(CreateParameter("@InShopID", SqlDbType.Int, product.InShopID));
                // parms.Add(CreateParameter("@Price", SqlDbType.VarChar, product.Price));
                // parms.Add(CreateParameter("@StoreName", SqlDbType.VarChar, product.StoreName));
                // parms.Add(CreateParameter("@ProductName", SqlDbType.VarChar, product.ProductName));
                // parms.Add(CreateParameter("@TagLine", SqlDbType.VarChar, product.TagLine));
                // parms.Add(CreateParameter("@Brand", SqlDbType.VarChar, product.Brand));
                // parms.Add(CreateParameter("@Description", SqlDbType.VarChar, product.Description));
                // parms.Add(CreateParameter("@UnitOfMeasure", SqlDbType.VarChar, product.UnitOfMeasure));
                // parms.Add(CreateParameter("@ListingAge", SqlDbType.VarChar, product.ListingAge));
                // parms.Add(CreateParameter("@MainBarCode", SqlDbType.VarChar, product.MainBarcode));
                // parms.Add(CreateParameter("@URL", SqlDbType.VarChar, product.URL));
                // parms.Add(CreateParameter("@InsertDate", SqlDbType.DateTime, product.InsertDate));
                // parms.Add(CreateParameter("@PostDate", SqlDbType.DateTime, product.PostDate));
                // parms.Add(CreateParameter("@isPosted", SqlDbType.Bit, product.isPosted));
                // parms.Add(CreateParameter("@SavedAmount", SqlDbType.VarChar, product.SavedAmount));
                // parms.Add(CreateParameter("@isSaving", SqlDbType.Bit, product.isPosted));
                // parms.Add(CreateParameter("@ProductImage", SqlDbType.VarChar, product.ProductImage));
                // parms.Add(CreateParameter("@ProductImageLg", SqlDbType.VarChar, product.ProductImageLg));
                // parms.Add(CreateParameter("@PriceNonSpecial", SqlDbType.VarChar, product.PriceNonSpecial));
                // parms.Add(CreateParameter("@ProductInstructions", SqlDbType.VarChar, product.ProductInstructions));
                // parms.Add(CreateParameter("@Comments", SqlDbType.Bit, product.isPosted));
                // parms.Add(CreateParameter("@ProductCount", SqlDbType.Int, product.ProductCount));
                // parms.Add(CreateParameter("@JobID", SqlDbType.Int, product.JobID));



                ExecuteNonQuery(connString, mainQuery, false, parms);
            }
            catch (Exception e)
            {
                RecordDetailedErrorData(e, 0, 1);
            }
        }

        */



        /*    public void SaveJobListing(JobListing j, bool checkPostDate = true)
            {
                try
                {
                    Regex test = new Regex("^Email.*");
                    if (j.Contact != null && test.IsMatch(j.Contact))
                    {
                        j.Contact = j.Contact.Substring(5).Trim();
                    }

                    if (j.Contact != null && j.Contact.Length > 400) j.Contact = j.Contact.Substring(0, 399);
                    if (j.ContactName != null && j.ContactName.Length > 400) j.ContactName = j.ContactName.Substring(0, 399);
                    if (j.Company != null && j.Company.Length > 200) j.Company = j.Company.Substring(0, 199);
                    if (j.Location != null && j.Location.Length > 100) j.Location = j.Location.Substring(0, 99);
                    //   if (j.JobTitle != null && j.JobTitle.Length > 200) j.JobTitle = j.JobTitle.Substring(0, 199);
                    SaveContact(j);

                    //Check for duplicates, exit early
                    string query = "SELECT COUNT(*) FROM [dbo].[Job.JobList] " +
                                " WHERE";

                    if (checkPostDate) query += " abs(DateDiff(dd, PostDate ,@PostDate)) < 2 AND";
                    query += " Company = @Company AND JobTitle = @JobTitle AND Contact = @Contact";

                    List<SqlParameter> parms = new List<SqlParameter>();
                    //     parms.Add(CreateParameter("@JobTitle", SqlDbType.VarChar, j.JobTitle));
                    parms.Add(CreateParameter("@Company", SqlDbType.VarChar, j.Company));
                    parms.Add(CreateParameter("@Contact", SqlDbType.VarChar, j.Contact));
                    if (checkPostDate) parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, j.PostDate));
                    object result = ExecuteScalar(connString, query, false, parms);

                    int count = 1;
                    if (result != null) int.TryParse(result.ToString(), out count);
                    if (count > 0) return;

                    //If not in DB, add.
                    query = "INSERT INTO [dbo].[Job.JobList] " + Environment.NewLine +
                        "  ([JobTitle] " + Environment.NewLine +
                        "  ,[Company] " + Environment.NewLine +
                        "  ,[JobListDesc] " + Environment.NewLine +
                        "  ,[Location] " + Environment.NewLine +
                        "  ,[ListingAge] " + Environment.NewLine +
                        "  ,[Contact] " + Environment.NewLine +
                        "  ,[ContactName] " + Environment.NewLine +
                        "  ,[PostDate] " + Environment.NewLine +
                        "  ,[WebsiteID] " + Environment.NewLine +
                        "  ,[URL]) " + Environment.NewLine +
                        "VALUES " + Environment.NewLine +
                        "  (@JobTitle " + Environment.NewLine +
                        "  ,@Company " + Environment.NewLine +
                        "  ,@JobListDesc " + Environment.NewLine +
                        "  ,@Location " + Environment.NewLine +
                        "  ,@ListingAge " + Environment.NewLine +
                        "  ,@Contact " + Environment.NewLine +
                        "  ,@ContactName " + Environment.NewLine +
                        "  ,@PostDate " + Environment.NewLine +
                        "  ,@WebsiteID " + Environment.NewLine +
                        "  ,@URL)";

                    parms = new List<SqlParameter>();
                    //  parms.Add(CreateParameter("@JobTitle", SqlDbType.VarChar, j.JobTitle));
                    parms.Add(CreateParameter("@Company", SqlDbType.VarChar, j.Company));
                    parms.Add(CreateParameter("@JobListDesc", SqlDbType.VarChar, j.Description));
                    parms.Add(CreateParameter("@Location", SqlDbType.VarChar, j.Location));
                    parms.Add(CreateParameter("@ListingAge", SqlDbType.VarChar, j.ListingAge));
                    parms.Add(CreateParameter("@Contact", SqlDbType.VarChar, j.Contact));
                    parms.Add(CreateParameter("@ContactName", SqlDbType.VarChar, j.ContactName));
                    parms.Add(CreateParameter("@PostDate", SqlDbType.VarChar, j.PostDate));
                    parms.Add(CreateParameter("@WebsiteID", SqlDbType.Int, j.WebsiteID));
                    parms.Add(CreateParameter("@URL", SqlDbType.VarChar, j.URL));

                    ExecuteNonQuery(connString, query, false, parms);
                }
                catch (Exception e)
                {
                    RecordDetailedErrorData(e, 0, 1);
                }
            }

            public void SaveContact(JobListing j)
            {
                //return;

                try
                {
                    if (j.Contact.Length > 100) j.Contact = j.Contact.Substring(0, 99);

                    //Check for duplicates, exit early
                    string query = "SELECT COUNT(*) FROM [dbo].[Job.JobList]" +
                                " WHERE ContactName = @ContactName AND Company = @Company AND ContactEmail = @ContactEmail";
                    List<SqlParameter> parms = new List<SqlParameter>();
                    parms.Add(CreateParameter("@Company", SqlDbType.VarChar, j.Company));
                    parms.Add(CreateParameter("@ContactName", SqlDbType.VarChar, j.ContactName));
                    parms.Add(CreateParameter("@ContactEmail", SqlDbType.VarChar, j.Contact));
                    object result = ExecuteScalar(connString, query, false, parms);

                    int count = 1;
                    if (result != null) int.TryParse(result.ToString(), out count);
                    if (count > 0) return;

                    //If not in DB, add.
                    query = "INSERT INTO [dbo].[Job.JobList] " + Environment.NewLine +
                        "  ([ContactName] " + Environment.NewLine +
                        "  ,[ContactEmail] " + Environment.NewLine +
                        "  ,[Company] " + Environment.NewLine +
                        "  ,[isContacted]) " + Environment.NewLine +
                        "VALUES " + Environment.NewLine +
                        "  (@ContactName " + Environment.NewLine +
                        "  ,@ContactEmail " + Environment.NewLine +
                        "  ,@Company " + Environment.NewLine +
                        "  , 0)";

                    parms = new List<SqlParameter>();
                    parms.Add(CreateParameter("@ContactEmail", SqlDbType.VarChar, j.Contact));
                    parms.Add(CreateParameter("@ContactName", SqlDbType.VarChar, j.ContactName));
                    parms.Add(CreateParameter("@Company", SqlDbType.VarChar, j.Company));

                    ExecuteNonQuery(connString, query, false, parms);
                }
                catch (Exception e)
                {
                    RecordDetailedErrorData(e, 0, 1);
                }
            }

            public List<JobListing> GetSavedJobs(int AgeCutoff = 10)
            {
                try
                {
                    List<JobListing> results = new List<JobListing>();

                    const string query = "SELECT * FROM [dob].[Job.JobList] WHERE DateDiff(dd, PostDate, GetDate()) <= @AgeCutoff ";

                    List<SqlParameter> parms = new List<SqlParameter>();
                    parms.Add(CreateParameter("@AgeCutoff", SqlDbType.Int, AgeCutoff));

                    DataSet ds = ExecuteQuery(connString, query, false, parms);

                    if (ds.Tables.Count > 0)
                    {
                        DataRow[] rows = ds.Tables[0].Select();

                        foreach (DataRow row in rows)
                        {
                            try
                            {
                                JobListing jl = new JobListing();
                                jl.JobID = getDBValue<int>(row, "JobListID");
                                //  jl.JobTitle = getDBValue<string>(row, "JobTitle");
                                jl.Company = getDBValue<string>(row, "Company");
                                jl.Description = getDBValue<string>(row, "JobListDesc");
                                jl.Location = getDBValue<string>(row, "Location");
                                jl.ListingAge = getDBValue<string>(row, "ListingAge");
                                jl.Contact = getDBValue<string>(row, "Contact");
                                jl.ContactName = getDBValue<string>(row, "ContactName");
                                jl.URL = getDBValue<string>(row, "URL");
                                jl.InsertDate = getDBValue<DateTime>(row, "CreatedDate");
                                jl.PostDate = getDBValue<DateTime>(row, "PostDate");
                                jl.isReviewed = getDBValue<bool>(row, "isReviewed");
                                jl.isMessaged = getDBValue<bool>(row, "isMessaged");
                                jl.isNoContact = getDBValue<bool>(row, "isNotContact");
                                results.Add(jl);
                            }
                            catch (Exception ex)
                            {
                                RecordDetailedErrorData(ex, 0, 1);
                            }
                        }
                        return results;
                    }
                }
                catch (Exception e)
                {
                    RecordDetailedErrorData(e, 0, 1);
                }
                return null;
            }
    */

        /*
    public int GetMostRecentPostDate(int WebsiteID)
    {
        try
        {
            //filter out negative Aged entries
            const string query = "SELECT TOP 1 DateDiff(dd,PostDate, GetDate()) FROM [dbo].[Job.JobList] j WHERE WebsiteID = @WebsiteID AND ListingAge NOT LIKE '-%' ORDER BY PostDate Desc";

            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@WebsiteID", SqlDbType.Int, WebsiteID));
            object result = ExecuteScalar(connString, query, false, parms);

            int days = 999;
            if (result != null) int.TryParse(result.ToString(), out days);

            return days;
        }
        catch (Exception e)
        {
            RecordDetailedErrorData(e, 0, 1);
        }
        return 999;
    }
    */

        /*
    public void ClearSavedJobs()
    {
        try
        {
            const string query = "TRUNCATE TABLE [dbo].[Job.JobList]";
            object result = ExecuteScalar(connString, query, false, null);
        }
        catch (Exception e)
        {
            RecordDetailedErrorData(e, 0, 1);
        }
    }
    */

        /*
    public void MarkJobAsReviewed(int JobID)
    {
        try
        {
            const string query = "UPDATE j SET isReviewed = 1 FROM [dbo].[Job.JobList] j WHERE JobID = @JobID";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@JobID", SqlDbType.VarChar, JobID));
            object result = ExecuteScalar(connString, query, false, parms);
        }
        catch (Exception e)
        {
            RecordDetailedErrorData(e, 0, 1);
        }
    }

    */

        /*
    public void MarkJobAsContacted(int JobID)
    {
        try
        {
            const string query = "UPDATE j SET [isMessaged] = 1 FROM [dbo].[Job.JobList] j WHERE JobID = @JobID";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@JobID", SqlDbType.VarChar, JobID));
            object result = ExecuteScalar(connString, query, false, parms);
        }
        catch (Exception e)
        {
            RecordDetailedErrorData(e, 0, 1);
        }
    }
    */

        /*

    public void MarkJobAsBlocked(int JobID)
    {
        try
        {
            const string query = "UPDATE j SET [isNoContact] = 1 FROM [dbo].[Job.JobList] j WHERE JobID = @JobID";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@JobID", SqlDbType.VarChar, JobID));
            object result = ExecuteScalar(connString, query, false, parms);
        }
        catch (Exception e)
        {
            RecordDetailedErrorData(e, 0, 1);
        }
    }

    */

        /*
        public void SendJobEmail(string message)
        {
            try
            {
                const string query = "sp_JobEmail";
                List<SqlParameter> parms = new List<SqlParameter>();
                parms.Add(CreateParameter("@strBody", SqlDbType.VarChar, message));
                parms.Add(CreateParameter("@Subject", SqlDbType.VarChar, "Test- Job Listing"));
                parms.Add(CreateParameter("@NotifyBosses", SqlDbType.Bit, false));
                parms.Add(CreateParameter("@isHtml", SqlDbType.Bit, false));
                object result = ExecuteScalar(connString, query, true, parms);
            }
            catch (Exception e)
            {
                RecordDetailedErrorData(e, 0, 1);
            }
        }

        public void SendJobSummary(string message)
        {
            try
            {
                const string query = "sp_JobEmail";
                List<SqlParameter> parms = new List<SqlParameter>();
                parms.Add(CreateParameter("@strBody", SqlDbType.VarChar, message));
                parms.Add(CreateParameter("@Subject", SqlDbType.VarChar, "Daily Summary of Job Listings"));
                parms.Add(CreateParameter("@TestOnly", SqlDbType.Bit, false));
                parms.Add(CreateParameter("@isHtml", SqlDbType.Bit, true));
                object result = ExecuteScalar(connString, query, true, parms);
            }
            catch (Exception e)
            {
                RecordDetailedErrorData(e, 0, 1);
            }
        }
        */



        public void RecordDetailedErrorData(Exception e, int WebsiteID, int priority, string message ="")
    {
        StackTrace st = null;

        ErrorReporting err = new ErrorReporting();

        if (e != null)
        {
            st = new StackTrace(e, true);
            // Get the top stack frame

            StackFrame frame = null;
            for (int frameCount = 0; frameCount < st.FrameCount; frameCount++)
            {
                // Get the line number from the stack frame
                frame = st.GetFrame(frameCount);

                //Want to skip past WatiN embedded functions.
                if (!String.IsNullOrEmpty(frame.GetFileName())) break;
            }

            err.TargetLine = frame.GetFileLineNumber();
            err.TargetMethod = frame.GetMethod().Name;
            err.TargetFile = frame.GetFileName();
        }
        err.WebsiteID = WebsiteID;
        err.Priority = priority;

        if (e == null)
        {
            err.Message = message;
        }
        else
        {
            err.Message = e.Message;
            err.ExceptionType = e.GetType().Name;


            if (e.InnerException != null)
            {
                err.InnerMessage = e.InnerException.Message;
                err.InnerExceptionType = e.InnerException.GetType().Name;
            }
            else if (message != "")
            {
                err.InnerMessage = message;
            }
        }

        this.ReportError(err);
    }

    

    public bool ReportError(ErrorReporting err)
    {
        if (err == null || err.ExceptionType == "COMException") return true;

        try
        {
            string query = "INSERT INTO [dbo].[Process.ErrorReporting] "
               + " ([WebsiteID] ,[ExceptionType] ,[Message] ,[TargetFile] ,[TargetMethod] ,[TargetLine] ,[InnerExceptionType] ,[InnerMessage],[Priority]) "
               + " VALUES (@WebsiteID ,@ExceptionType ,@Message ,@TargetFile ,@TargetMethod ,@TargetLine ,@InnerExceptionType ,@InnerMessage ,@Priority) ";

            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@WebsiteID", SqlDbType.Int, err.WebsiteID));

            parms.Add(CreateParameter("@ExceptionType", SqlDbType.VarChar, err.ExceptionType));
            parms.Add(CreateParameter("@Message", SqlDbType.VarChar, err.Message));
            parms.Add(CreateParameter("@TargetFile", SqlDbType.VarChar, err.TargetFile));
            parms.Add(CreateParameter("@TargetMethod", SqlDbType.VarChar, err.TargetMethod));
            parms.Add(CreateParameter("@TargetLine", SqlDbType.VarChar, err.TargetLine));
            parms.Add(CreateParameter("@InnerExceptionType", SqlDbType.VarChar, err.InnerExceptionType));
            parms.Add(CreateParameter("@InnerMessage", SqlDbType.VarChar, err.InnerMessage));
            parms.Add(CreateParameter("@Priority", SqlDbType.Int, err.Priority));

            ExecuteNonQuery(connString, query, false, parms);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            //MessageBox.Show("Error reporting error");
            //LogEvent(appId, connections.CurrentDB.ID, null, null, Gaffey.Core.EventType.EventLevel.Error, "SQL-Error Reporting AutoStatus Error " + ex.Message, true, false);
            return false;
        }
    }
    

    public bool ProcessLog(string message, int count = 0, int websiteID = 0, DateTime? startdate = null)
    {

        try
        {
            if (startdate == null) startdate = DateTime.Now;

            string query = "INSERT INTO [dbo].[Process.ProcessLog] "
               + " ([ProcessTypeID] ,[WebsiteID] ,[Comments] ,[StartTime] ,[FinishTime] ,[TotalCount]) "
               + " VALUES (@ProcessTypeID ,@WebsiteID ,@Comments ,@StartTime ,@FinishTime ,@TotalCount) ";

            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(CreateParameter("@WebsiteID", SqlDbType.Int, websiteID));
            parms.Add(CreateParameter("@ProcessTypeID", SqlDbType.Int, 1));

            parms.Add(CreateParameter("@Comments", SqlDbType.VarChar, message));
            parms.Add(CreateParameter("@StartTime", SqlDbType.VarChar, startdate));
            parms.Add(CreateParameter("@FinishTime", SqlDbType.VarChar, DateTime.Now));
            parms.Add(CreateParameter("@TotalCount", SqlDbType.Int, count));

            ExecuteNonQuery(connString, query, false, parms);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            //LogEvent(appId, connections.CurrentDB.ID, null, null, Gaffey.Core.EventType.EventLevel.Error, "SQL-Error Reporting AutoStatus Error " + ex.Message, true, false);
            return false;
        }
        
    }
    

}


}