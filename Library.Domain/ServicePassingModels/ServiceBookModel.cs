using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.ServicePassingModels
{
    public class ServiceBookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PicDate { get; set; }
    }
}
