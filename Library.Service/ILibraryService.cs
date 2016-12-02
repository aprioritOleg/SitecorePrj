using Library.Domain.Models;
using Library.Domain.ServicePassingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
   public interface ILibraryService
    {
        List<Author> GetAllAuthros();

        List<Book> GetAllBooks();

        List<Book> GetAllAvailableBooks();

        List<ServiceBookModel> GetBooksTakenByUser(string userEmail);
        List<History> GetAllHistories();
        bool TakeBook(string userEmail, int bookId);
        void AddBook(Book book, params Author[] authors);
        void RemoveBook(int bookId);
        void ChangeBookQuantity(int bookId, int newQuantity);
        bool RegistrationNewUser(User user);
        bool Login(User user);
    }
}
