using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YClimb.Entities
{
    public class User
    {
        public User(string nickname, string email, string password) 
        {
            Nickname = nickname;
            Email = email;
            Password = password;
        }
        public User(string nickname, string email, string password, bool isAdmin): this(nickname, email, password)
        {
            IsAdmin = isAdmin;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public string Nickname { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public bool? IsAdmin { get; set; }


        public byte[]? Avatar { get; set; }
    }
}
