namespace DashBackend.Helpers
{
    public static class AuthHelper
    {
        public static string HashPassword(string password, int workFactor = 12)
        {
            // Use bcrypt which generates and stores its own salt inside the returned hash string.
            // Requires the BCrypt.Net-Next (BCrypt.Net) NuGet package.
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}