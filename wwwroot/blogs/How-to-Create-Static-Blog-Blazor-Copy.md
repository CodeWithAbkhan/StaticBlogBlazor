---
{
  "Id": 0,
  "Title": "How to Create Static Blog Blazor .Net 8",
  "Description": "This is a simple MD blog post that demonstrates how to use How to Create Static Blog Blazor .Net 8 Markdown for text content",
  "Date": "2023-10-30T00:00:00",
  "Content": null,
  "Slug": null,
  "FeaturedImage": "/images/blog2.jpg",
  "Views": 24,
  "Author": "Abkhan",
  "AuthorImg": "/profile/user1.png",
  "Thumbnail": "/images/blog2_thumbnail.jpg",
  "Tags": [
    ".Net 8",
    "Blazor",
    "Static Site Generator",
    "MarkDown"
  ]
}---


# How to Create Static Blog Blazor .Net 8

Blazor .NET 8 provides a powerful platform for building web applications, and one interesting use case is creating a static blog. In this blog post, we will explore how to create a static blog using Blazor .NET 8 and Markdown for text content.

## Prerequisites

Before we get started, make sure you have the following prerequisites:

1. Visual Studio or Visual Studio Code with .NET 8 installed.
2. Basic knowledge of Blazor and Markdown.

## Steps to Create a Static Blog with Blazor .NET 8

1. **Project Setup**:
   - Create a new Blazor project or use an existing one.
   - Organize your project structure to manage blog posts and assets.

2. **Markdown Content**:
   - Write your blog posts in Markdown format. Markdown is a lightweight and easy-to-use markup language that is perfect for creating content.

3. **Metadata**:
   - For each blog post, include metadata in JSON format at the beginning of the Markdown file. This metadata can include details like the title, description, date, author, featured image, tags, and views.

4. **Read Markdown Files**:
   - Create a service in your Blazor project that reads the Markdown files, extracts the metadata, and parses the content.

5. **Display Blog Posts**:
   - Use Blazor components to display the blog posts. You can format the metadata and content as needed.

6. **Styling and Theming**:
   - Apply styling and theming to make your blog visually appealing. Blazor allows you to use CSS or other styling techniques.

7. **Deploy as Static Website**:
   - Build the Blazor project and generate static HTML files for each blog post.

8. **Hosting**:
   - Host your static blog on a web server or a static site hosting platform. Popular options include GitHub Pages, Netlify, or Azure Storage.

9. **Custom Features**:
   - Depending on your needs, you can add custom features such as a search bar, navigation, or comments.
```cs
@page "/blog/{Slug}"

@using StaticBlog3.Models

@inject BlogService BlogService

<h1>@blogPost.Title</h1>
<p>Published on @blogPost.Date.ToShortDateString() by @blogPost.Author</p>
<img src="@blogPost.FeaturedImage" alt="@blogPost.Title" />

<div>
    @((MarkupString)blogPost.Content)
</div>

@code {
    private BlogPost blogPost;

    [Parameter] public string Slug { get; set; }

    protected override void OnInitialized()
    {
        blogPost = BlogService.GetBlogPostBySlug(Slug);

        if (blogPost != null)
        {
            // Call the UpdateViewCount method to increment the view count
            BlogService.UpdateViewCount(Slug);
        }
    }
}
```