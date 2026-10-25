# Unlock All Backgrounds

Unlocks every main-menu background in SPT 4.1.x, and adds live EFT's **Server
Room** background. Install it and they are all selectable; no profile edit, no
new game, no config.

## What it unlocks

The `EnvironmentUI` customisation category - the backdrop behind your character
on the main menu. The mod reads the category from the live database rather than
hardcoding IDs, so it grants whatever is present, including backgrounds added by
other mods.

Vanilla SPT 4.1.x ships six:

| Background | Granted to a fresh profile? |
|---|---|
| Random | yes |
| Factory | yes |
| Wood | yes |
| Laboratory | yes |
| The Unheard Edition | **no** |
| Cyber | **no** |

Note that `AvailableAsDefault` in `customization.json` is *not* what gates
ownership. The real grant list is `templates/customisationStorage.json`, which
contains only the four above; The Unheard Edition has `AvailableAsDefault: true`
yet is still absent from it.

### Backgrounds added by other mods

`WTT-ContentBackport` adds five more and extends the client's
`EEnvironmentUIType` enum via a BepInEx prepatcher so the client can render
them. All five ship locked, and this mod unlocks them too:

| Background | Notes |
|---|---|
| Hotline Tarkov (`MiamiEnvironmentUiType`) | |
| Ending: And Survived | `HiddenByDefault` |
| Ending: For Humanity | `HiddenByDefault` |
| Ending: In The Darkness | `HiddenByDefault` |
| Ending: Didn't Escape From Yourself | `HiddenByDefault` |

The four ending backgrounds carry a `HiddenByDefault` flag that SPT 4.1.3's
`CustomizationProperties` model does not know about, so it passes through
`ExtensionData` to the client untouched. Whether the client still hides them
from the selector once owned is not verified. They are also video-backed rather
than static images.

Load order is what makes this work: `WTTContentBackport` runs at
`TypePriority=100002` and this mod at `1000000` (`PostLoad`), so those entries
are already in the database when the mod runs.

### Server Room

The mod adds live EFT's server room background (live scene
`EnvironmentUIScene_Season_LAB`): a looping panorama of a dim server room with
an animated radio. This is amos's
[Server Room Background](https://github.com/14amos/ServerRoomBackground)
(MIT), included with amos's permission (given 2026-10-25):

- `ServerRoom.cs` (server): the customization item under `EnvironmentUI`,
  named "Server Room" in every locale, at `Preload + 3`, so the unlock pass
  below picks it up.
- `ServerRoom/Prepatch`: a BepInEx patcher that adds
  `EEnvironmentUIType.ServerRoomEnvironmentUiType = 11` (WTT uses 6-10).
- `ServerRoom/Plugin`: registers the scene with the menu, loads its bundles
  when picked, and swaps the random camera rotation for the scene's own
  left/right slide.
- `ServerRoom/Scripts`: five live-only scripts the scene uses, rebuilt. The
  bundles reference this assembly by name, so it must stay
  `ServerRoomBackground.Scripts`.

The two bundles and the video are Battlestate's assets, repacked by amos for
the 4.1.6 client. They are not committed: the build downloads amos's pinned
v1.0.0 release, checks its SHA-256, and copies them into `ServerRoom/assets/`.

If amos's standalone mod is also installed, it wins: the client plugin
declares `BepInIncompatibility` on `com.amos.serverroombackground`, and the
server item uses the same id, so the background is added once.

Nothing outside `EnvironmentUI` is touched - hideout walls, floors, ceilings,
shooting-range targets, mannequin poses, gestures and dog tags stay locked.

## How it works

`CustomizationController.GetCustomisationStorage` returns:

    clone(templateTable.CustomisationStorage) + profile.CustomisationUnlocks

The first list is a shared template served to every profile on every request.
The mod appends the missing backgrounds to it at `OnLoadOrder.PostLoad`.

That means:

- **No save file is ever written.** Existing profiles are covered too.
- **Uninstalling re-locks them.** Nothing is left behind in your profile.

## Building

Requires the **.NET 10 SDK** (the server targets `net10.0`; the client
projects `netstandard2.x`). One build covers the server DLL and the Server
Room client projects; the first build downloads the Server Room assets.

    dotnet build -c Release -p:SPTPath="H:\SPT4.1.X"

Build and copy straight into the install (server mod, plugin, patcher):

    dotnet build -c Release -p:SPTPath="H:\SPT4.1.X" -p:DeployToSPT=true

Produce a drop-in zip in `dist\`:

    dotnet build -c Release -p:SPTPath="H:\SPT4.1.X" -t:PackageRelease

## Installing

Extract the zip over the SPT folder. It contains:

    BepInEx\patchers\UnlockAllBackgrounds\
    BepInEx\plugins\UnlockAllBackgrounds\
    SPT_Runtime\user\mods\UnlockAllBackgrounds\

Since 1.0.1 there is a client part (for the Server Room). With Fika, every
player needs the `BepInEx` folders, not just the host: a client without them
receives a background type it does not know. The client plugin references no
`spt-*` assembly, so the 4.1.3 plugin version gate does not apply.

## Credits

- **amos**: [Server Room Background](https://github.com/14amos/ServerRoomBackground),
  the server room scene repack and its code (MIT, `ServerRoom/LICENSE.txt`),
  included with their permission.
- **Battlestate Games**: the scene, models, textures, video and sounds.
