using System.Linq;
using FitAndFun.Models; 

namespace FitAndFun
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Proveravamo da li već postoje podaci u bazi
            if (context.Activities.Any())
            {
                return;   // Podaci su već inicijalizovani
            }

            var activities = new Activity[]
            {
                new Activity
                {
                    Name = "Trčanje",
                    UserId = 1,
                    Duration = 60,
                    Date = DateTime.Now,
                    Location = "Park",
                    ActivityType = "Aerobik",
                    AdditionalDescription = "Lagano trčanje u parku"
                },
                new Activity
                {
                    Name = "Plivanje",
                    UserId = 2,
                    Duration = 45,
                    Date = DateTime.Now.AddDays(-1),
                    Location = "Bazeni",
                    ActivityType = "Plivanje",
                    AdditionalDescription = "Plivanje u olimpijskom bazenu"
                },
                // Dodajte još aktivnosti po potrebi
            };

            context.Activities.AddRange(activities);
            context.SaveChanges();
        }
    }
}
