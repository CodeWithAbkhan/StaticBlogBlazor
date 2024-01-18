using Microsoft.Extensions.FileProviders;
using StaticBlog3.Components;
using StaticBlog3.Services;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddAntiforgery(options => { options.Cookie.SecurePolicy = CookieSecurePolicy.Always; });
// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddScoped<BlogService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); 
app.UseAntiforgery();
app.MapRazorComponents<App>();
app.MapGet("/sitemap.xml", async (HttpContext context) =>
{
    var blogService = context.RequestServices.GetRequiredService<BlogService>();
    var sitemap = blogService.GenerateSitemap();

    context.Response.Headers.Add(HeaderNames.ContentType, "application/xml");
    await context.Response.WriteAsync(sitemap);
});
app.MapGet("/robots.txt", // ?? return a valid robots.txt
    () => """
          User-agent: *
          Allow: /
          """).ShortCircuit(200);
app.Run();
