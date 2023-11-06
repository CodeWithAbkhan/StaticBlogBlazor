using System.ComponentModel.DataAnnotations;

namespace StaticBlog3.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string FeaturedImage { get; set; }
        public int Views { get; set; }
        public string Thumbnail { get; set; }
        public List<string> Tags { get; set; } // Change to a list of tags

        // Constructor to initialize the Tags list
        public BlogPost()
        {
            Tags = new List<string>();
        }
    }

}
