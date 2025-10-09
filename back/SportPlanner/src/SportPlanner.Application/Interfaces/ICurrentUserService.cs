namespace SportPlanner.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();
    Guid GetSubscriptionId();
    string GetUserEmail();
    bool IsAuthenticated { get; }
}
