using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace UnlockAllBackgrounds;

/// <summary>
/// Adds live EFT's server room background, the server half of amos's Server Room Background
/// (https://github.com/14amos/ServerRoomBackground, MIT; client half in ServerRoom/).
///
/// A customization item under EnvironmentUI naming the enum value the client prepatcher adds
/// (ServerRoomEnvironmentUiType = 11; WTT-ContentBackport uses 6-10), plus its display name in
/// every locale. Same id as amos's mod, so having both installed adds it once. Runs before
/// UnlockAllBackgrounds (PostLoad), which then unlocks it with the rest.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.Preload + 3)]
public class ServerRoom(
    TemplateTable templateTable,
    LocaleTable localeTable,
    ISptLogger<ServerRoom> logger
) : IOnLoad
{
    private const string Id = "6a5e7a11c0ffee00000b0d11";
    private const string TypeName = "ServerRoomEnvironmentUiType";
    private const string DisplayName = "Server Room";

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var id = new MongoId(Id);
        if (templateTable.Customization is null || templateTable.Customization.ContainsKey(id))
        {
            return Task.CompletedTask;
        }

        templateTable.Customization[id] = new CustomizationItem
        {
            Id = id,
            Name = "ServerRoomEnvironmentUI",
            Parent = CustomisationTypeId.ENVIRONMENT_UI,
            Type = "Item",
            Properties = new CustomizationProperties
            {
                AvailableAsDefault = false,
                Description = "ServerRoomEnvironmentUI",
                Name = "ServerRoomEnvironmentUI",
                ShortName = "ServerRoomEnvironmentUI",
                ProfileVersions = [],
                Side = ["Savage", "Bear", "Usec"],
                EnvironmentUIType = TypeName,
            },
        };

        foreach (var (_, locale) in localeTable.Global)
        {
            locale.AddTransformer(entries =>
            {
                if (entries is not null)
                {
                    entries[TypeName] = DisplayName;
                }

                return entries;
            });
        }

        logger.Success($"[UnlockAllBackgrounds] Server Room background added ({Id}).");

        return Task.CompletedTask;
    }
}
