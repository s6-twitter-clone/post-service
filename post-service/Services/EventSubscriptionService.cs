using post_service.Interfaces;
using post_service.Models.Events;

namespace post_service.Services;

public class EventSubscriptionService: BackgroundService
{
    private readonly IServiceProvider services;
    private readonly IEventService eventService;

    public EventSubscriptionService(IServiceProvider services, IEventService eventService)
    {
        this.services = services;
        this.eventService = eventService;

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        eventService.subscribe<AddUserEvent>("user-added", onUserAdded);
        eventService.subscribe<UpdateUserEvent>("user-updated", onUserUpdated);

        return Task.CompletedTask;
    }

    private void onUserAdded(AddUserEvent user)
    {
        using var scope = services.CreateScope();

        var userService =
            scope.ServiceProvider
                .GetRequiredService<UserService>();

        userService.Add(user.Id, user.DisplayName);
    }

    private void onUserUpdated(UpdateUserEvent user)
    {
        using var scope = services.CreateScope();

        var userService =
            scope.ServiceProvider
                .GetRequiredService<UserService>();

        userService.Update(user.Id, user.DisplayName);
    }
}
