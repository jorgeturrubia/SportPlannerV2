namespace SportPlanner.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
    bool IsAuthenticated { get; }
}
