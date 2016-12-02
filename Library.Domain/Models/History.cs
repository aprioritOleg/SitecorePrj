using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Models
{
    public class History
    {
        public int Id { get; set; }
       public string BookTitle { get; set; }
        public string UserEmail { get; set; }
        public DateTime PicDate { get; set; }

    }
}
