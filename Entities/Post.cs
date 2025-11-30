using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Entities
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Связь с пользователем
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Коллекция изображений
        public virtual ICollection<PostImage> Images { get; set; } = new List<PostImage>();
    }

    public class PostImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public int Order { get; set; }

        // Связь с постом
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
    }


}
