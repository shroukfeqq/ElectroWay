namespace ElctroWay.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            await RoleSeeder.SeedRolesAsync(services);
            await AdminSeeder.SeedAdminAsync(services);
        }
    }
}
