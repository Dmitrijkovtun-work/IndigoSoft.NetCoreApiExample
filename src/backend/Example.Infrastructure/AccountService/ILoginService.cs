namespace Example.Infrastructure.AccountService
{
    public interface ILoginService
    {
        public void LoginAsync(string username, string password);
    }
}
