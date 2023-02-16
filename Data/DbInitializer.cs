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
                new User { UserName = "Admin", UUID = Guid.NewGuid().ToString(), Email = "admin@test.com", Password = "123456"},
                new User { UserName = "Test", UUID = Guid.NewGuid().ToString(), Email = "test@test.com", Password = "123456"}
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }