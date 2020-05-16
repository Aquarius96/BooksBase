using System;
using System.Collections.Generic;
using System.Text;

namespace BooksBase.Models
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public Guid AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}
