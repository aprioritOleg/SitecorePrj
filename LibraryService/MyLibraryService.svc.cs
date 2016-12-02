using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Library.Domain.Models;
using System.Data.SqlClient;
using System.Data;
using Library.Domain.ServicePassingModels;
using System.Text.RegularExpressions;

namespace LibraryService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class MyLibraryService : ILibraryService
    {
        enum DataBaseCommands
        {
            sp_AddBook,
            sp_ChangeBookQuantity,
            sp_DeleteBook,
            sp_RegisterNewUser,
            fc_ShowAllBooks,
            fc_BookAuthors,
            fc_ShowAvailableBooks,
            fc_ShowHistory,
            fc_UserBooksTaken,
            fc_GetAllAuhtors,
            fc_LoginValidation,
            sp_TakeBook
        };
        private SqlConnection connection;
        private readonly string sqlConnection;
        public MyLibraryService()
        {
            sqlConnection = @"Data Source=T\SQLEXPRESS;Initial Catalog=Libraries;Integrated Security=True";
        }
        public MyLibraryService(string connection)
        {
            sqlConnection = connection;
            this.connection = new SqlConnection(sqlConnection);
        }
        public void AddBook(Book book, params Author[] authors)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                try
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("id");
                    dt.Columns.Add("name");
                    if (authors.Length == 1 && authors[0].Name =="")
                    {
                        authors[0].Name = "Unknown Author";
                    }
                    for (int i = 0; i < authors.Length; i++)
                    {
                        dt.Rows.Add(authors[i].Id, authors[i].Name);
                    }
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_AddBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@title", book.Title));
                    cmd.Parameters.Add(new SqlParameter("@quantity", book.Quantity));
                    SqlParameter tvparam = cmd.Parameters.AddWithValue("@List", dt);
                    tvparam.SqlDbType = SqlDbType.Structured;
                    int count = cmd.ExecuteNonQuery();
                    Console.WriteLine(count);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public void ChangeBookQuantity(int bookId, int newQuantity)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_ChangeBookQuantity.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(new SqlParameter[] { new SqlParameter("@id", bookId), new SqlParameter("@newQuantity", newQuantity) });
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public List<Book> GetAllAvailableBooks()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<Book> books = new List<Book>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowAvailableBooks.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book() { Id = (int)reader[0], Title = (string)reader[1], Quantity = (int)reader[2] });
                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public List<Book> GetAllBooks()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                List<Book> books = new List<Book>();
                try
                {
                    //fc_BookAuthors
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowAllBooks.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new Book() { Id = (int)reader[0], Title = (string)reader[1], Quantity = (int)reader[2] });
                        using (SqlConnection authorsCon = new SqlConnection(sqlConnection))
                        {
                            authorsCon.Open();

                            SqlCommand authorsCmd = new SqlCommand();
                            authorsCmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_BookAuthors.ToString()}(@bookId)";
                            authorsCmd.Parameters.Add(new SqlParameter("@bookId", books[books.Count - 1].Id));
                            authorsCmd.CommandType = CommandType.Text;
                            authorsCmd.Connection = authorsCon;
                            SqlDataReader authorsReader = authorsCmd.ExecuteReader();
                            while (authorsReader.Read())
                            {
                                books[books.Count - 1].Authors.Add(new Author() { Id = (int)authorsReader[0], Name = (string)authorsReader[1] });
                            }
                        }

                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public List<History> GetAllHistories()
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<History> histories = new List<History>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_ShowHistory.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        histories.Add(new History() { Id = (int)reader[0], BookTitle = (string)reader[1], UserEmail = (string)reader[2], PicDate = (DateTime)reader[3] });
                    }
                    return histories;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public List<ServiceBookModel> GetBooksTakenByUser(string userEmail)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<ServiceBookModel> books = new List<ServiceBookModel>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_UserBooksTaken.ToString()}(@userEmail)";
                    //SqlParameter param = new SqlParameter();
                    ////param.ParameterName = "@userEmail";
                    ////param.Value = userEmail;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@userEmail", userEmail));
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        books.Add(new ServiceBookModel() { Id = (int)reader[0], Title = (string)reader[1], PicDate = (DateTime)reader[2] });
                    }
                    return books;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        public bool RegistrationNewUser(User user)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    if (!EmailValidation.IsValidEmail(user.Email))
                    {
                        throw new Exception("Your email isn't valid");
                    }
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_RegisterNewUser.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                    int row = cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public void RemoveBook(int bookId)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_DeleteBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", bookId));
                    int row = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        public bool TakeBook(string userEmail, int bookId)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(DataBaseCommands.sp_TakeBook.ToString(), con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@email", userEmail));
                    cmd.Parameters.Add(new SqlParameter("@bookId", bookId));
                    int row = cmd.ExecuteNonQuery();
                    if (row > 0)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return false;

        }

        public List<Author> GetAllAuthros()
        {

            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
                List<Author> authrorsList = new List<Author>();
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select * from dbo.{DataBaseCommands.fc_GetAllAuhtors.ToString()}()";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        authrorsList.Add(new Author() { Id = (int)reader[0], Name = (string)reader[1] });
                    }
                    return authrorsList;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool Login(User user)
        {
            using (SqlConnection con = new SqlConnection(sqlConnection))
            {
              
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = $"select dbo.{DataBaseCommands.fc_LoginValidation.ToString()}(@email)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@email", user.Email));
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0] == null)
                        {
                            return false;
                        }
                        return true;
                    }
                    
                }
                catch (Exception)
                {
                    throw;
                }
                return false;
            }
        }

        static class EmailValidation
        {
            public static bool IsValidEmail(string email)
            {
                Regex emailValidation = new Regex(@"\w{2,15}@\w{1,5}\.\w{1,4}");
                return emailValidation.IsMatch(email);
            }
        }
    }
}
