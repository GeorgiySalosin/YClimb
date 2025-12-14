using System;
using System.Collections.Generic;
using System.Linq;
using YClimb.Entities;
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

                // Convert templates to sessions for this date
                var sessions = dayTemplates.Select(template => new TrainingSessionViewModel
                {
                    TrainingGroupId = template.TrainingGroupId,
                    TrainingGroup = template.TrainingGroup,
                    TrainerId = template.TrainerId,
                    Trainer = template.Trainer,
                    StartTime = date.Add(template.StartTime),
                    EndTime = date.Add(template.EndTime),
                    // Храним ID шаблона для возможных операций
                    TemplateId = template.Id
                }).ToList();

                dailySchedules.Add(new DailySchedule
                {
                    Date = date,
                    Sessions = sessions
                });
            }

            return dailySchedules;
        }

        // FOR INIT - Создаем только шаблоны
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
                templates.Add(new ScheduleTemplate
                {
                    DayOfWeek = day,
                    TrainingGroupId = group1.Id,
                    TrainerId = trainer1.Id,
                    StartTime = TimeSpan.FromHours(9.5),
                    EndTime = TimeSpan.FromHours(12)
                });
                templates.Add(new ScheduleTemplate
                {
                    DayOfWeek = day,
                    TrainingGroupId = group2.Id,
                    TrainerId = trainer3.Id,
                    StartTime = TimeSpan.FromHours(17),
                    EndTime = TimeSpan.FromHours(22)
                });
            }

            // Tuesday, Thursday
            var ttDays = new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday };
            foreach (var day in ttDays)
            {
                templates.Add(new ScheduleTemplate
                {
                    DayOfWeek = day,
                    TrainingGroupId = group3.Id,
                    TrainerId = trainer2.Id,
                    StartTime = TimeSpan.FromHours(10),
                    EndTime = TimeSpan.FromHours(13)
                });
                templates.Add(new ScheduleTemplate
                {
                    DayOfWeek = day,
                    TrainingGroupId = group2.Id,
                    TrainerId = trainer3.Id,
                    StartTime = TimeSpan.FromHours(18),
                    EndTime = TimeSpan.FromHours(21)
                });
            }

            // Saturday
            templates.Add(new ScheduleTemplate
            {
                DayOfWeek = DayOfWeek.Saturday,
                TrainingGroupId = group1.Id,
                TrainerId = trainer1.Id,
                StartTime = TimeSpan.FromHours(11),
                EndTime = TimeSpan.FromHours(14)
            });

            _context.ScheduleTemplates.AddRange(templates);
            _context.SaveChanges();
        }

        // Методы для управления шаблонами
        public void AddScheduleTemplate(ScheduleTemplate template)
        {
            _context.ScheduleTemplates.Add(template);
            _context.SaveChanges();
        }

        public void UpdateScheduleTemplate(ScheduleTemplate template)
        {
            _context.ScheduleTemplates.Update(template);
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

        public ScheduleTemplate? GetTemplateById(int id)
        {
            return _context.ScheduleTemplates
                .Include(st => st.TrainingGroup)
                .Include(st => st.Trainer)
                .FirstOrDefault(st => st.Id == id);
        }
    }


    public class TrainingSessionViewModel
    {
        public int TemplateId { get; set; }
        public int TrainingGroupId { get; set; }
        public virtual TrainingGroup? TrainingGroup { get; set; }
        public int TrainerId { get; set; }
        public virtual Trainer? Trainer { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class DailySchedule
    {
        public DateTime Date { get; set; }
        public List<TrainingSessionViewModel> Sessions { get; set; } = new List<TrainingSessionViewModel>();
    }
}