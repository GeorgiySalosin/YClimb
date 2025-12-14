using System.Windows.Controls;
using YClimb.Utilities;

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