using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using YClimb.Entities;

namespace YClimb.Utilities
{
    public class PostService
    {
        private readonly ApplicationContext _context;

        public PostService(ApplicationContext context)
        {
            _context = context;
        }

        // Feed
        public async Task<List<Post>> GetPostsWithUsersAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(string title, string content, int userId, List<byte[]>? images = null)
        {
            var post = new Post
            {
                Title = title,
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            if (images != null)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    post.Images.Add(new PostImage
                    {
                        ImageData = images[i],
                        FileName = $"image_{i}.jpg",
                        Order = i
                    });
                }
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }



        // Routes

        public async Task<List<Route>> GetRoutesWithUsersAsync()
        {
            return await _context.Routes
                .Include(r => r.User)
                .Include(r => r.Image)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Route> CreateRouteAsync(string title, int userId, byte[]? originalImage = null)
        {
            var route = new Route
            {
                Title = title,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            if (originalImage != null)
            {
                route.Image = new RouteImage
                {
                    OriginalImageData = originalImage,
                    EditedImageData = originalImage, // По умолчанию та же картинка
                    FileName = $"route_{DateTime.Now:yyyyMMddHHmmss}.jpg"
                };
            }

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            return route;
        }

        public async Task<bool> DeletePostAsync(int postId, int currentUserId, bool isAdmin)
        {
            var post = await _context.Posts
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return false;

            if (post.UserId != currentUserId && !isAdmin)
                return false;

            if (post.Images.Any())
            {
                _context.PostImages.RemoveRange(post.Images);
            }
            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRouteAsync(int routeId, int currentUserId, bool isAdmin)
        {
            var route = await _context.Routes
                .Include(r => r.Image)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
                return false;
            if (route.UserId != currentUserId && !isAdmin)
                return false;
            if (route.Image != null)
            {
                _context.RouteImages.Remove(route.Image);
            }
            _context.Routes.Remove(route);

            await _context.SaveChangesAsync();
            return true;
        }


        public BitmapImage ConvertByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return new BitmapImage();

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    
    }
}