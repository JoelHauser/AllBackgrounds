# Unlock All Backgrounds

Unlocks every main-menu background in SPT 4.1.x. Install it and they are all
selectable; no profile edit, no new game, no config.

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

Requires the **.NET 10 SDK** (the server targets `net10.0`).

Note: the .NET 10 SDK on this machine is a **user-local install** at
`%USERPROFILE%\.dotnet`, which is *not* on PATH — a bare `dotnet` resolves to
`C:\Program Files\dotnet` and only has SDK 8, which fails with `NETSDK1045`.
Invoke it by full path:

    & "$env:USERPROFILE\.dotnet\dotnet.exe" build -c Release -p:SPTPath="H:\SPT4.1.X"

Build and copy straight into the install:

    & "$env:USERPROFILE\.dotnet\dotnet.exe" build -c Release -p:SPTPath="H:\SPT4.1.X" -p:DeployToSPT=true

Produce a drop-in zip in `dist\`:

    & "$env:USERPROFILE\.dotnet\dotnet.exe" build -c Release -p:SPTPath="H:\SPT4.1.X" -t:PackageRelease

## Installing

Copy `UnlockAllBackgrounds.dll` to:

    <SPT install>\SPT_Runtime\user\mods\UnlockAllBackgrounds\

Server-side only — there is no client plugin, so the 4.1.3 plugin version gate
does not apply.
