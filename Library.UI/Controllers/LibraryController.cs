using Library.Domain.Models;
using Library.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Library.UI.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        readonly private Library.Service.ILibraryService service;
        // GET: Library

        public LibraryController(Library.Service.ILibraryService service)
        {
            this.service = service;
        }

        public ActionResult Index()
        {
            try
            {
                List<Book> all = service.GetAllBooks().ToList();
                ModelState.Clear();
                List<Author> authorsList = service.GetAllAuthros().ToList();
                TempData["authros"] = authorsList;
                TempData["histories"] = service.GetAllHistories().ToArray();
                return View(all);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", ex.Message);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(int? id)
        {
            if (id == null)
            {
                return PartialView("_LibraryBooks", service.GetAllBooks().ToList());
            }
            else
            {
                service.RemoveBook((int)id);
                ModelState.Clear();
                return PartialView("_LibraryBooks", service.GetAllBooks().ToList());
            }
        }
        [HttpGet]
        public PartialViewResult SetQuantity(int? id, int quantity)
        {
            if (id == null || quantity < 0)
            {
                return PartialView("_LibraryBooks", service.GetAllBooks().ToList());
            }
            else
            {

                service.ChangeBookQuantity((int)id, (int)quantity);
                return PartialView("_LibraryBooks", service.GetAllBooks().ToList());
            }
        }
        [HttpPost]
        public JsonResult AddNewBook(Book book, string[] arr)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<Author> authorArray = new List<Author>();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        authorArray.Add(new Author() { Name = arr[i] });
                    }
                    service.AddBook(book, authorArray.ToArray());
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Library");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    throw new Exception("Model isn't valid");
                }

            }
            catch (Exception ex)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Error", "Library", ex.Message);
                return Json(new { Url = redirectUrl });
            }

        }
        [HttpPost]
        public async Task<ActionResult> TakeBook(Book book)
        {
            try
            {
                service.TakeBook(User.Identity.Name, book.Id);
                var result = await Contact(book);
                if (!result)
                {
                    throw new Exception("Can't send mail");
                }
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Library");
                return Json(new { Url = redirectUrl });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", ex.Message);
            }

        }

        private async Task<bool> Contact(Book book)
        {
            var fromMail = ConfigurationManager.AppSettings["ownerMail"];
            var fromPassword = ConfigurationManager.AppSettings["ownerPassword"];
            var model = new EmailModel()
            {
                Body = $"You took the {book.Title} books in our library",
                From = fromMail,
                FromPassword = fromPassword,
                To = fromMail,//User.Identity.Name,
                Subject = "About books"
            };
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(model.From);
                mail.To.Add(new MailAddress(model.From));
                mail.Subject = model.Subject;
                mail.Body = model.Body;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(model.From.Split('@')[0], model.FromPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(mail);
                return true;
            }
            return false;
        }

        [HttpGet]
        public ActionResult Error(string message)
        {
            TempData["Error"] = message;
            return View();
        }
    }
}