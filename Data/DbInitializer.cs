public static class DbInitializer
    {
        public static void Initialize(ServerContext context)
        {
            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            context.SaveChanges();
        }
    }