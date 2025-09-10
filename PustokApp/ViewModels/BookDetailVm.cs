using PustokApp.Models;
using PustokApp.Models.BookSlider;

namespace PustokApp.ViewModels
{
    public class BookDetailVm
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public BookComment BookComments { get; set; }
        public int AvgRate { get; set; }
        public bool HasComment { get; set; }
        }
}
