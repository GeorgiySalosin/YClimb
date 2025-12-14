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

            // Get all schedule templates
            var templates = _context.ScheduleTemplates
                .Include(st => st.TrainingGroup)
                .Include(st => st.Trainer)
                .ToList();

            var dailySchedules = new List<DailySchedule>();

            // Generate schedule for next 7 days based on templates
            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                var dayOfWeek = date.DayOfWeek;

                // Get templates for this day of week
                var dayTemplates = templates
                    .Where(t => t.DayOfWeek == dayOfWeek)
                    .ToList();

                // Convert templates to actual sessions for this date
                var sessions = dayTemplates.Select(template => new TrainingSession
                {
                    TrainingGroupId = template.TrainingGroupId,
                    TrainingGroup = template.TrainingGroup,
                    TrainerId = template.TrainerId,
                    Trainer = template.Trainer,
                    StartTime = date.Add(template.StartTime),
                    EndTime = date.Add(template.EndTime)
                }).ToList();

                dailySchedules.Add(new DailySchedule
                {
                    Date = date,
                    Sessions = sessions
                });
            }

            return dailySchedules;
        }

        // FOR INIT - Теперь создаем шаблоны вместо конкретных сессий
        public void InitializeSampleSchedule()
        {
            if (_context.ScheduleTemplates.Any()) return;

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

            // Create schedule templates for each day of week
            var templates = new List<ScheduleTemplate>();

            // Monday, Wednesday, Friday
            var mwfDays = new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday };
            foreach (var day in mwfDays)
            {
                templates.Add(new ScheduleTemplate(day, group1.Id, trainer1.Id,
                    TimeSpan.FromHours(9.5), TimeSpan.FromHours(12)));
                templates.Add(new ScheduleTemplate(day, group2.Id, trainer3.Id,
                    TimeSpan.FromHours(17), TimeSpan.FromHours(22)));
            }

            // Tuesday, Thursday
            var ttDays = new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday };
            foreach (var day in ttDays)
            {
                templates.Add(new ScheduleTemplate(day, group3.Id, trainer2.Id,
                    TimeSpan.FromHours(10), TimeSpan.FromHours(13)));
                templates.Add(new ScheduleTemplate(day, group2.Id, trainer3.Id,
                    TimeSpan.FromHours(18), TimeSpan.FromHours(21)));
            }

            // Saturday
            templates.Add(new ScheduleTemplate(DayOfWeek.Saturday, group1.Id, trainer1.Id,
                TimeSpan.FromHours(11), TimeSpan.FromHours(14)));

            // Sunday - no sessions (day off)

            _context.ScheduleTemplates.AddRange(templates);
            _context.SaveChanges();
        }

        // Метод для управления шаблонами расписания
        public void AddScheduleTemplate(ScheduleTemplate template)
        {
            _context.ScheduleTemplates.Add(template);
            _context.SaveChanges();
        }

        public void RemoveScheduleTemplate(int templateId)
        {
            var template = _context.ScheduleTemplates.Find(templateId);
            if (template != null)
            {
                _context.ScheduleTemplates.Remove(template);
                _context.SaveChanges();
            }
        }

        public List<ScheduleTemplate> GetAllTemplates()
        {
            return _context.ScheduleTemplates
                .Include(st => st.TrainingGroup)
                .Include(st => st.Trainer)
                .OrderBy(st => st.DayOfWeek)
                .ThenBy(st => st.StartTime)
                .ToList();
        }
    }

    public class DailySchedule
    {
        public DateTime Date { get; set; }
        public List<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
    }
}