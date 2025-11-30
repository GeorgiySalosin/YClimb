using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YClimb.Utilities
{
    public static class AppSettings
    {
        private static readonly Settings Settings = Settings.Default;

        public static int? CurrentUserId
        {
            get => Settings.CurrentUserId > 0 ? Settings.CurrentUserId : null;
            set
            {
                Settings.CurrentUserId = value ?? 0;
                Settings.Save();
            }
        }

        public static void ClearUserSession()
        {
            CurrentUserId = null;
        }
    }
}
