namespace SolarisPlatform.Tests.Integration.Common;
public static class TestConstants
{
    public const string AdminEmail       = "admin@solaris.local";
    public const string AdminPassword    = "Admin123!#";
    public const string TestUserEmail    = "testuser@solaris.local";
    public const string TestUserPassword = "Test@123456";
    public static class Emails
    {
        public const string Admin    = AdminEmail;
        public const string TestUser = TestUserEmail;
        public const string Invalid  = "noexiste@solaris.com";
    }
    public static class Seed
    {
        public const long EmpresaId  = 1;
        public const long ProyectoId = 1;
        public const long UsuarioId  = 1;
    }
}
