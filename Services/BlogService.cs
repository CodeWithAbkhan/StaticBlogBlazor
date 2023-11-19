using Markdig;
using StaticBlog3.Models;
using System.Text;
using System.Text.Json;
using System.Xml;
using Microsoft.Extensions.Configuration;
namespace StaticBlog3.Services
{
    public class BlogService
    {
        
        private readonly string _blogDirectory = "wwwroot/blogs";

        public IEnumerable<BlogPost> GetBlogPosts()
        {
            var blogPosts = new List<BlogPost>();

            foreach (var file in Directory.GetFiles(_blogDirectory, "*.md"))
            {
                var slug = Path.GetFileNameWithoutExtension(file); // Extract the slug from the file name
                var markdown = File.ReadAllText(file);

                var jsonSeparator = markdown.IndexOf("---", 3);

                if (jsonSeparator >= 0)
                {
                    var jsonMetadata = markdown.Substring(3, jsonSeparator - 3);
                    //var content = markdown.Substring(jsonSeparator);

                    var blogPost = JsonSerializer.Deserialize<BlogPost>(jsonMetadata, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    blogPost.Slug = slug; // Set the slug based on the file name

                    //blogPost.Content = Markdown.ToHtml(content);

                    blogPosts.Add(blogPost);
                }
            }

            return blogPosts.OrderByDescending(post => post.Date);
        }

        public BlogPost GetBlogPostBySlug(string slug)
        {
            string blogFilePath = Path.Combine(_blogDirectory, slug + ".md");

            if (File.Exists(blogFilePath))
            {
                var markdown = File.ReadAllText(blogFilePath);
                var jsonSeparator = markdown.IndexOf("---", 3);

                if (jsonSeparator >= 0)
                {
                    var jsonMetadata = markdown.Substring(3, jsonSeparator - 3);

                    var content = markdown.Substring(jsonSeparator);
                    var blogPost = JsonSerializer.Deserialize<BlogPost>(jsonMetadata, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Parse and store the content (if needed)
                    blogPost.Content = Markdown.ToHtml(content);

                    return blogPost;
                }
            }

            return null; // Blog post not found
        }
        public void UpdateViewCount(string slug)
        {
            string blogFilePath = Path.Combine(_blogDirectory, slug + ".md");

            if (File.Exists(blogFilePath))
            {
                var markdown = File.ReadAllText(blogFilePath);
                var jsonSeparator = markdown.IndexOf("---", 3);

                if (jsonSeparator >= 0)
                {
                    var jsonMetadata = markdown.Substring(3, jsonSeparator - 3);
                    var content = markdown.Substring(jsonSeparator);

                    var blogPost = JsonSerializer.Deserialize<BlogPost>(jsonMetadata, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                   // Update the view count
                    blogPost.Views++;

                    // Serialize the updated blog post to JSON
                    var updatedJsonMetadata = JsonSerializer.Serialize(blogPost, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true // Optional: format JSON for better readability
                    });

                    // Create the updated Markdown content
                    var updatedMarkdown = "---\n" + updatedJsonMetadata + content;

                    // Save the updated content back to the file
                    File.WriteAllText(blogFilePath, updatedMarkdown);
                }
            }
        }
        public IEnumerable<BlogPost> GetPopularBlogPostsByViews(int numberOfPosts)
        {
            var blogPosts = GetBlogPosts();

            // Sort the blog posts by views in descending order
            var popularBlogPosts = blogPosts.OrderByDescending(post => post.Views);

            // Take the top N blog posts based on the given numberOfPosts parameter
            var topBlogPosts = popularBlogPosts.Take(numberOfPosts);

            return topBlogPosts;
        }

        public string GenerateSitemap()
        {

            var blogPosts = GetBlogPosts();

            var sitemap = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (var writer = XmlWriter.Create(sitemap, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var blogPost in blogPosts)
                {
                    writer.WriteStartElement("url");

                    writer.WriteElementString("loc", "https://example.com/" + blogPost.Slug); // Replace with your actual blog URL
                    writer.WriteElementString("lastmod", blogPost.Date.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    writer.WriteElementString("changefreq", "monthly"); // You can adjust this frequency as needed
                    writer.WriteElementString("priority", "0.8"); // You can adjust this priority as needed

                    writer.WriteEndElement(); // Close the "url" element
                }

                writer.WriteEndElement(); // Close the "urlset" element
                writer.WriteEndDocument();
            }

            return sitemap.ToString();
        }

    }
}
