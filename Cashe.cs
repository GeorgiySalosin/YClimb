using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb
{
    static internal class Cashe
    {
        static string UserPFP = "Images/DefaultUser.jpg";
        public static string GetUserPFP() { return UserPFP; }
        public static void SetUserPFP(string value) { UserPFP = value; }
    }
}
