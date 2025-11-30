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

            // Добавляем изображения если есть
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

        public BitmapImage ConvertByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

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
