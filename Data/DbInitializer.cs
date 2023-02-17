public static class DbInitializer
    {
        public static void Initialize(ServerContext context)
        {
            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }