using Markdig;
using StaticBlog3.Models;
using System.Text.Json;

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


    }
}
