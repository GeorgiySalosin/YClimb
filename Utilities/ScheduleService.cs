using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YClimb.Entities;
using YClimb.Utilities;
using Microsoft.EntityFrameworkCore;

namespace YClimb.Utilities
{
    public class ScheduleService
    {
        private readonly ApplicationContext _context;

        public ScheduleService(ApplicationContext context)
        {
            _context = context;
        }

        // Get schedule for the next 7 days including today
        public List<DailySchedule> GetWeeklySchedule()
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(7);

            var sessions = _context.TrainingSessions
                .Include(ts => ts.TrainingGroup)
                .Include(ts => ts.Trainer)
                .Where(ts => ts.StartTime >= today && ts.StartTime < endDate)
                .OrderBy(ts => ts.StartTime)
                .ToList();

            var dailySchedules = new List<DailySchedule>();

            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                var daySessions = sessions.Where(ts => ts.StartTime.Date == date).ToList();

                dailySchedules.Add(new DailySchedule
                {
                    Date = date,
                    Sessions = daySessions
                });
            }

            return dailySchedules;
        }

        // FOR INITIALIZING
        public void InitializeSampleSchedule()
        {
            if (_context.TrainingSessions.Any()) return;

            // Create sample trainers
            var trainer1 = new Trainer("Alexander Megos", "Head coach");
            var trainer2 = new Trainer("Ai Mori", "Youth coach");
            var trainer3 = new Trainer("Tomoa Narasaki", "Competition coach");

            // Create sample groups
            var group1 = new TrainingGroup("Beginner Group", "For beginners");
            var group2 = new TrainingGroup("Competition Group", "For competitors");
            var group3 = new TrainingGroup("Youth Group", "For young climbers");

            _context.Trainers.AddRange(trainer1, trainer2, trainer3);
            _context.TrainingGroups.AddRange(group1, group2, group3);
            _context.SaveChanges();

            // Create sample sessions for the next 7 days
            var sessions = new List<TrainingSession>();
            var startDate = DateTime.Today;

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);

                // Add different sessions based on day of week
                if (date.DayOfWeek == DayOfWeek.Monday || date.DayOfWeek == DayOfWeek.Wednesday || date.DayOfWeek == DayOfWeek.Friday)
                {
                    sessions.Add(new TrainingSession(group1.Id, trainer1.Id, date.AddHours(9.5), date.AddHours(12))); // 9:30-13:00
                    sessions.Add(new TrainingSession(group2.Id, trainer3.Id, date.AddHours(17), date.AddHours(22))); // 17:00-22:00
                }
                else if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Thursday)
                {
                    sessions.Add(new TrainingSession(group3.Id, trainer2.Id, date.AddHours(10), date.AddHours(13))); // 10:00-13:00
                    sessions.Add(new TrainingSession(group2.Id, trainer3.Id, date.AddHours(18), date.AddHours(21))); // 18:00-21:00
                }
                else
                {
                    sessions.Add(new TrainingSession(group1.Id, trainer1.Id, date.AddHours(11), date.AddHours(14))); // 11:00-15:00
                }
            }

            _context.TrainingSessions.AddRange(sessions);
            _context.SaveChanges();
        }
    }

    public class DailySchedule
    {
        public DateTime Date { get; set; }
        public List<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
    }
}
