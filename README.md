# vsa-consumer-unity

A Unity 6000.6 project that installs the UPM package `com.umpire.unity` ([vsa-unity](https://github.com/MgCohen/vsa-unity)) from its git URL. It runs the dice sample of vsa-unity as a consumer does. It is the install check of the package.

## Install

`Packages/manifest.json` has the git URL with the package folder and a release tag:

```json
"com.umpire.unity": "https://github.com/MgCohen/vsa-unity.git?path=/Packages/com.umpire.unity#<commit or tag>"
```

To test another version of the package, change the part after `#` to a commit, a branch or a tag. Unity installs the package again when the manifest changes.

## Content

| Path | Content |
|---|---|
| `Assets/DiceGame/` | The dice sample of vsa-unity, copied 1:1 with its `.meta` files. `Definition/DiceArchitecture.cs` is its own definition. The app has the roots `App/`, `Definition/` and `Views/`. |
| `Assets/DiceGame/Scenes/MainMenu.unity` | The first scene: a Start button |
| `Assets/DiceGame/Scenes/DiceGame.unity` | The game scene: the shell `DiceShell`, a Roll button, a label and a roll list |
| `docs/images/` | Captures of the check |

## Check

1. Open the project in Unity 6000.6.6f1.
2. Open `Assets/DiceGame/Scenes/MainMenu.unity` and enter Play Mode.
3. Select Start, then select Roll three times.
4. The label shows `Rolls: 3` and the last roll. The roll list shows one line for each roll: a prefab factory makes each line. The Console has no errors.

![The dice sample after three rolls](docs/images/dice-preview-24.png)
