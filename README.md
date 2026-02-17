# TracingAppv2

VR tracing application for Meta Quest focused on motor-control assessment and training. Users trace displayed patterns and receive scoring for accuracy and coverage.

## Stack
- Unity 6000.3.7f1 (URP)
- Meta XR SDK (Quest)
- Unity Input System

## Repository layout
- `TracingApp/` Unity project root
- `generate_patterns.py` pattern generator script
- `TracingApp/Assets/Resources/Patterns/` generated pattern textures used at runtime
- `PROJECT.md` project specification and roadmap

## Quick start
1. Open `TracingApp/` in Unity Hub using Unity `6000.3.7f1`.
2. Ensure Android build support is installed in Unity Hub.
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press Play in Editor or build to device.

## Regenerate patterns
From repo root:

```powershell
C:/rod_w/TracingAppv2/.venv/Scripts/python.exe generate_patterns.py
```

This writes PNGs into:

- `TracingApp/Assets/Resources/Patterns/`

Then in Unity, reimport that folder if needed.

## Build and run on Quest
1. Enable Developer Mode on headset and USB debugging.
2. In Unity: **File > Build Settings > Android > Switch Platform**.
3. In Player settings, use **IL2CPP** and **ARM64**.
4. Add scenes to Build Settings.
5. Click **Build And Run**.

## Open-source notes
- License: MIT (see `LICENSE`)
- Excluded from version control: Unity-generated folders (`Library`, `Temp`, etc.) and local tool caches.

## Status
See `PROJECT.md` for detailed feature/status tracking.
