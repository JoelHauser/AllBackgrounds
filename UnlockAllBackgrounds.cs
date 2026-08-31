using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace UnlockAllBackgrounds;

/// <summary>
/// Grants every main-menu background (the EnvironmentUI customisation category)
/// to every profile.
///
/// CustomizationController.GetCustomisationStorage returns
/// clone(templateTable.CustomisationStorage) + profile.CustomisationUnlocks, so
/// appending to the shared template list reaches new and existing profiles alike
/// without ever writing to a save. Uninstalling the mod re-locks them.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostLoad)]
public class UnlockAllBackgrounds(
    TemplateTable templateTable,
    ISptLogger<UnlockAllBackgrounds> logger
) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var storage = templateTable.CustomisationStorage;
        if (storage is null)
        {
            logger.Warning("[UnlockAllBackgrounds] CustomisationStorage table is null - nothing unlocked.");
            return Task.CompletedTask;
        }

        var alreadyGranted = storage.Select(entry => entry.Id).ToHashSet();

        var backgrounds = templateTable.Customization?
            .Values
            .Where(item =>
                item.Type == "Item"
                && item.Parent == CustomisationTypeId.ENVIRONMENT_UI)
            .ToList() ?? [];

        var added = 0;
        foreach (var background in backgrounds)
        {
            if (!alreadyGranted.Add(background.Id))
            {
                continue;
            }

            storage.Add(new CustomisationStorage
            {
                Id = background.Id,
                Source = CustomisationSource.DEFAULT,
                Type = CustomisationType.ENVIRONMENT,
            });
            added++;

            logger.Debug($"[UnlockAllBackgrounds] Unlocked {background.Properties?.Name ?? background.Name}");
        }

        logger.Success(
            $"[UnlockAllBackgrounds] {backgrounds.Count} background(s) available, {added} newly unlocked.");

        return Task.CompletedTask;
    }
}
