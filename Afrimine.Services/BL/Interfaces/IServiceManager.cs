namespace Afrimine.Services.BL.Interfaces
{
    public interface IServiceManager
    {
        IUserService User {  get; }
        IDashboardService Dashboard { get; }
    }
}
