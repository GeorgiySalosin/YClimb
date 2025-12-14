using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Entities
{
    public class Route
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Связь с пользователем
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Изображение маршрута (только одно)
        public virtual RouteImage? Image { get; set; }
    }

    public class RouteImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] OriginalImageData { get; set; } = Array.Empty<byte>();
        public byte[] EditedImageData { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;

        // Данные для слоев (сериализованные круги)
        public string? LayerData { get; set; }

        // Связь с маршрутом
        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;
    }

    [Serializable]
    public class CircleLayer
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public string MainColor { get; set; }       // Основной цвет (красный/желтый)
        public string ContrastColor { get; set; }   // Контрастный цвет (белый/черный)
        public double MainThickness { get; set; }   // Толщина основной линии
        public double ContrastThickness { get; set; } // Толщина контрастной линии
        public string ToolType { get; set; } = "circle";
    }

    [Serializable]
    public class LineLayer
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public string MainColor { get; set; }       // Основной цвет (красный/желтый)
        public string ContrastColor { get; set; }   // Контрастный цвет (белый/черный)
        public double MainThickness { get; set; }   // Толщина основной линии
        public double ContrastThickness { get; set; } // Толщина контрастной линии
        public string ToolType { get; set; } = "line";
    }


    
}
