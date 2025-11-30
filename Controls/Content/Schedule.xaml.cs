using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YClimb.Utilities;
using Microsoft.EntityFrameworkCore;

namespace YClimb.Controls.Content
{
    public partial class Schedule : UserControl
    {
        private readonly ScheduleService _scheduleService;

        public Schedule()
        {
            InitializeComponent();
            _scheduleService = new ScheduleService(new ApplicationContext());
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            // Initialize sample data (remove this after first run)
            _scheduleService.InitializeSampleSchedule();

            var weeklySchedule = _scheduleService.GetWeeklySchedule();
            ScheduleItemsControl.ItemsSource = weeklySchedule;
        }
    }
}
