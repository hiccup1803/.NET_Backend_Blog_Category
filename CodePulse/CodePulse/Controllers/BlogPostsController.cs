using CodePulse.API.Models.Domains;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(IBlogPostRepository _blogPostRepository, ICategoryRepository _categoryRepository)
        {
            blogPostRepository = _blogPostRepository;
            categoryRepository = _categoryRepository;
        }

        // POST: {apibaseurl}/api/blogposts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            // Convert DTO to Domain Model
            var blogPost = new BlogPost
            {
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                Author = request.Author,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if(existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            await blogPostRepository.CreateAsync(blogPost);

            // Convert Domain Model back to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                ShortDescription = blogPost.ShortDescription,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };

            return Ok(response);
        }

        // GET: {apibaseurl}/api/blogposts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();

            // Map Domain Model to Dto
            var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    PublishedDate = blogPost.PublishedDate,
                    UrlHandle = blogPost.UrlHandle,
                    IsVisible = blogPost.IsVisible,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Categories = blogPost.Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle,
                    }).ToList()
                });
            }
            return Ok(response);
        }

        // GET: {apibaseurl}/api/blogposts/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            var blogPost = await blogPostRepository.GetByIdAsync(id);

            if(blogPost is null)
            {
                return NotFound();
            }

            // Convert Domain model to Dto
            var response = new BlogPostDto
            {
                Id= blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                Author = blogPost.Author,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto 
                { 
                    Id = x.Id, 
                    Name = x.Name, 
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);
        }

        // PUT: {apibaseurl}/api/blogposts/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, UpdateBlogPostRequestDto request)
        {
            // Convert DTO to domain model
            var blogPost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                Author = request.Author,
                PublishedDate = request.PublishedDate,
                UrlHandle = request.UrlHandle,
                IsVisible = request.IsVisible,
                FeaturedImageUrl = request.FeaturedImageUrl,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);

                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            // Call Repository To Update BlogPost Domain Model
            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if(updatedBlogPost == null) 
            {
                return NotFound();
            }

            // Convert Domain Model back to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                Author = blogPost.Author,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        // DELETE: {apibaseurl}/api/blogposts/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            var deletedBlogPost = await blogPostRepository.DeleteAsync(id);

            if(deletedBlogPost == null)
            {
                return NotFound();
            }

            // Convert Domain Model to DTO
            var response = new BlogPostDto
            {
                Id = deletedBlogPost.Id,
                Title = deletedBlogPost.Title,
                ShortDescription = deletedBlogPost.ShortDescription,
                Content = deletedBlogPost.Content,
                Author = deletedBlogPost.Author,
                PublishedDate = deletedBlogPost.PublishedDate,
                UrlHandle = deletedBlogPost.UrlHandle,
                IsVisible = deletedBlogPost.IsVisible,
            };

            return Ok(response);
        }
    }
}
