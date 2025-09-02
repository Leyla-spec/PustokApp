using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PustokApp.Models;
using PustokApp.Models.BookSlider;

namespace PustokApp.Data
{
    public class PustokDbContex(DbContextOptions<PustokDbContex> options): IdentityDbContext<AppUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PustokDbContex).Assembly);
            base.OnModelCreating(modelBuilder); 
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Book> books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genres> Genre { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
