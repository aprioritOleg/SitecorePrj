using Library.Domain.Models;
using Library.Domain.ServicePassingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LibraryService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ILibraryService
    {
        [OperationContract]
        List<Author> GetAllAuthros();

        [OperationContract]
        List<Book> GetAllBooks();

        [OperationContract]
        List<Book> GetAllAvailableBooks();

        [OperationContract]
        List<ServiceBookModel> GetBooksTakenByUser(string userEmail);

        [OperationContract]
        List<History> GetAllHistories();

        [OperationContract]
        bool TakeBook(string userEmail, int bookId);
        
        [OperationContract]
        void AddBook(Book book, params Author[] authors);

        [OperationContract]
        void RemoveBook(int bookId);

        [OperationContract]
        void ChangeBookQuantity(int bookId, int newQuantity);

        [OperationContract]
        bool RegistrationNewUser(User user);

        [OperationContract]
        bool Login(User user);
    }
}
