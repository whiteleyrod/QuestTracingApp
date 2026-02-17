# VR Tracing App - Project Specification

## Project Overview

A Meta Quest VR application designed for fine motor control assessment and training through pattern tracing exercises. Users control a laser pointer with their head/gaze to trace various patterns with different complexity levels, line thicknesses, and scales.

---

## Core Concept

Users trace patterns displayed in VR using a head-controlled laser pointer. The application tracks accuracy, coverage, and time performance, providing immediate feedback and session summaries for assessment purposes.

---

## Key Features

### 1. Gaze-Based Laser Pointer Control
- **Origin**: Center of VR headset (gaze/head tracking)
- **Visual States**:
  - **Idle**: Transparent red laser pointer
  - **Active (Tracing)**: Bright red laser pointer
  - **Trail**: Leaves permanent "burn mark" showing traced path on the pattern
- **Control**: Activated/deactivated via trigger button on either hand controller

### 2. Recording Indicator
- **Visual**: Flashing red circle (like a recording light)
- **Behavior**: Appears when tracing is active (trigger pressed)
- **Position**: TBD (likely corner of view or near pattern)

### 3. Pattern Library
- **Total Patterns**: 20 unique patterns across 5 complexity levels
- **Levels**:
  - **Level 1**: Horizontal Line, Vertical Line, Diagonal Line, Zigzag
  - **Level 2**: Square, Triangle, Rectangle, Cross
  - **Level 3**: Pentagon, Hexagon, Star, Arrow
  - **Level 4**: Circle, Oval, Semicircle, Wave
  - **Level 5**: Heart, Spiral, Figure Eight, S-Curve

### 4. Configurable Settings

#### Session Settings
- **Time Limit**: Adjustable in minutes (up/down controls)
- **Number of Patterns**: Adjustable count per session (up/down controls)
- **Randomize Option**: "Surprise me" button for random pattern selection

#### Pattern Settings
- **Complexity**: Slider from Simple (1) to Complex (5)
- **Line Thickness**: Slider from Thin (10px) to Thick (50px)
- **Scale**: Slider from Small (1) to Huge (50) - controls pattern display size
- **Randomize Option**: "Surprise me" button for random level settings

---

## User Flow

### 1. Session Start
```
Launch App
  ↓
User ID Entry Screen
  → Enter 3 letters + 3 numbers (e.g., "ABC123")
  → OR skip (saved as "Anon")
  ↓
Settings Screen
  → Configure time limit, pattern count, complexity, thickness, scale
  → OR use "Surprise me" for randomization
  ↓
Session Begins
```

### 2. Trial Flow (Per Pattern)
```
Pattern Displayed
  ↓
User aims laser pointer (transparent red)
  ↓
User presses trigger on either controller
  ↓
Recording starts:
  - Laser turns bright red
  - Flashing red recording indicator appears
  - Burn trail starts appearing on pattern
  ↓
User traces pattern (continuous recording until stopped)
  ↓
User presses trigger again
  ↓
Recording stops
  ↓
Results displayed for 10 seconds:
  - Time taken
  - % Correct (pixels inside target)
  - % Completed (coverage of target pattern)
  - "Next" button available to skip wait
  ↓
Next pattern OR Session end
```

### 3. Session End
```
All patterns completed OR time limit reached
  ↓
Session Summary Screen:
  - List of all trials with individual scores
  - Overall session score/performance
  - Summary statistics
  ↓
Save data to CSV
Save screenshot of each pattern with timestamp
  ↓
Return to main menu OR exit
```

---

## Scoring System

### Per-Trial Metrics
1. **Accuracy (% Correct)**
   - Calculation: `(pixels traced inside target / total pixels traced) × 100`
   - Measures precision - how much of the trace stayed within bounds

2. **Coverage (% Completed)**
   - Calculation: `(unique target pixels hit / total target pixels) × 100`
   - Measures completeness - how much of the pattern was traced

3. **Time (seconds)**
   - Duration from first trigger press to second trigger press
   - Measures speed of completion

### Session Score
- **Overall Score**: Average or weighted combination of all trial scores
- **Individual Trial Results**: Displayed in summary table
- **Statistics**: Min/max/average for each metric across session

---

## Data Storage

### CSV File Format

#### Trial Data File: `{UserID}_{SessionTimestamp}_trials.csv`
```csv
UserID,SessionID,TrialNumber,PatternName,Complexity,LineThickness,Scale,TimeStarted,TimeCompleted,Duration_Seconds,PixelsTraced,PixelsInTarget,PixelsOutTarget,AccuracyPercent,TargetPixelsTotal,TargetPixelsCovered,CoveragePercent,ScreenshotFilename
ABC123,20250214_143022,1,level1_horizontal,1,20,512x512,2025-02-14 14:30:22,2025-02-14 14:30:45,23.4,1250,1180,70,94.4,1000,950,95.0,ABC123_trial1_20250214_143022.png
ABC123,20250214_143022,2,level2_square,2,30,1024x1024,2025-02-14 14:30:55,2025-02-14 14:31:28,33.1,2100,1890,210,90.0,2000,1850,92.5,ABC123_trial2_20250214_143055.png
```

#### Session Summary File: `{UserID}_{SessionTimestamp}_summary.csv`
```csv
UserID,SessionID,SessionDate,TotalTrials,TotalDuration_Seconds,AvgAccuracy,AvgCoverage,AvgTime,SettingsComplexity,SettingsThickness,SettingsScale,SettingsTimeLimit,SettingsPatternCount
ABC123,20250214_143022,2025-02-14,5,180.5,91.2,93.8,36.1,1-3,20-40,512-1024,10,5
```

### Screenshot Storage
- **Location**: `/Screenshots/{UserID}/{SessionTimestamp}/`
- **Naming**: `{UserID}_trial{N}_{Timestamp}.png`
- **Content**: Pattern with user's traced burn marks visible
- **Format**: PNG

---

## Technical Specifications

### Platform
- **Device**: Meta Quest (Quest 2/3/Pro)
- **Engine**: Unity 6000.3.7f1 (URP 17.3.0)
- **VR SDK**: Meta XR All-in-One SDK v85.0.0
- **XR Management**: com.unity.xr.management v4.5.4
- **Oculus XR Plugin**: com.unity.xr.oculus v4.5.2
- **Input**: Unity Input System v1.18.0

### Input
- **Head Tracking**: 6DOF gaze-based laser pointer control
- **Controllers**: Trigger button (either hand) for start/stop recording
- **Hand Preference**: Ambidextrous - either controller trigger works

### Display
- **Pattern Distance**: TBD (recommend 1.5-2 meters from user)
- **Pattern Placement**: World-space canvas, face user at session start
- **UI Overlays**: Screen-space for settings, results, indicators

### Performance Targets
- **Frame Rate**: 72 FPS minimum (90 FPS target for Quest 2, 120 FPS for Quest 3)
- **Latency**: <20ms laser pointer response time
- **Loading**: <2 seconds pattern switching

---

## User Interface Elements

### 1. Main Menu
- User ID entry (3 letters + 3 numbers)
- Skip to "Anon" option
- Settings button
- Start Session button
- Exit button

### 2. Settings Screen (Reference: SettingsScreen.png)
**Top Section:**
- "How long to train?" label
- Time Limit (minutes) - up/down arrows
- Number of Patterns - up/down arrows
- "Surprise me (Randomise)" button

**Bottom Section (Levels):**
- Complexity slider: Simple (1) to Complex (5)
- Line Thickness slider: Thin (10) to Thick (50)
- Scale slider: Small (1) to Huge (50)
- "Surprise me (Randomise)" button

### 3. In-Session HUD
- Current pattern display (centered, world-space)
- Laser pointer (head-controlled ray)
- Recording indicator (flashing red circle when active)
- Trial counter: "Pattern X of Y"
- Time remaining (if time limit set)
- Current trial timer (optional)

### 4. Trial Results Screen (10-second display)
- Time Taken: "XX.X seconds"
- Accuracy: "XX.X% Correct"
- Coverage: "XX.X% Completed"
- "Next" button to skip wait
- Auto-advance after 10 seconds

### 5. Session Summary Screen
- Session statistics header
- Trial results table:
  - Pattern name/thumbnail
  - Time
  - Accuracy %
  - Coverage %
- Overall session score
- "New Session" button
- "Main Menu" button
- "Exit" button

---

## Project File Structure

```
TracingAppv2/
│
├── Assets/
│   ├── Resources/
│   │   └── Patterns/               # ✅ Generated pattern images (1000 files)
│   │       ├── scale_512x512_width_1_level1_horizontal.png
│   │       ├── scale_512x512_width_1_level1_vertical.png
│   │       └── ... (1000 total: 20 patterns × 5 widths × 10 scales)
│   │
│   ├── Scenes/                     # ✅ All 4 scenes created and populated
│   │   ├── MainMenu.unity          # Build index 0 — User ID entry
│   │   ├── Settings.unity          # Build index 1 — Session configuration
│   │   ├── TracingSession.unity    # Build index 2 — Main tracing gameplay
│   │   └── Results.unity           # Build index 3 — Session summary
│   │
│   ├── Scripts/
│   │   ├── Core/                   # ✅ Implemented
│   │   │   ├── GameManager.cs      # Persistent singleton, scene navigation, session state
│   │   │   ├── SessionManager.cs   # Session flow, pattern sequence, timing
│   │   │   └── TrialManager.cs     # Per-trial state machine, recording toggle, scoring
│   │   ├── Input/                  # ✅ Implemented (VR + desktop fallback)
│   │   │   ├── LaserPointer.cs     # Gaze-based raycast, UV hit detection, visual states
│   │   │   └── ControllerInput.cs  # Trigger/keyboard/mouse input handling
│   │   ├── Tracing/                # ✅ Implemented
│   │   │   ├── TracingRecorder.cs  # Pixel-level burn trail painting (Bresenham lines)
│   │   │   └── PatternValidator.cs # Real-time on/off-target detection
│   │   ├── Scoring/                # ✅ Implemented
│   │   │   ├── TrialScorer.cs      # Accuracy & coverage pixel comparison
│   │   │   └── SessionScorer.cs    # Session-level aggregation (min/max/avg)
│   │   ├── Data/                   # ✅ Implemented
│   │   │   ├── SessionSettings.cs  # Configurable session parameters
│   │   │   ├── SessionData.cs      # Session-level data container
│   │   │   ├── TrialData.cs        # Per-trial data with CSV serialization
│   │   │   ├── PatternInfo.cs      # Pattern metadata (20 patterns, 5 levels)
│   │   │   ├── CSVWriter.cs        # Trial + summary CSV export
│   │   │   └── ScreenshotCapture.cs # RenderTexture/Texture2D → PNG capture
│   │   ├── UI/                     # ✅ Implemented
│   │   │   ├── UserIDEntry.cs      # 3-letter + 3-number validation
│   │   │   ├── SettingsController.cs # Sliders, up/down, randomize buttons
│   │   │   ├── ResultsDisplay.cs   # 10-second trial results overlay
│   │   │   ├── SessionSummary.cs   # Session summary with scrollable trial list
│   │   │   ├── RecordingIndicator.cs # Flashing red REC indicator
│   │   │   └── SessionHUD.cs       # In-session HUD (counter, timer, events)
│   │   ├── Patterns/               # ✅ Implemented
│   │   │   ├── PatternLoader.cs    # Load from Resources, sprite creation
│   │   │   └── PatternSelector.cs  # Filter by complexity, random/sequential
│   │   └── Editor/                 # ✅ Scene setup utilities
│   │       ├── SceneSetup_MainMenu.cs
│   │       ├── SceneSetup_Settings.cs
│   │       ├── SceneSetup_TracingSession.cs
│   │       ├── SceneSetup_Results.cs
│   │       └── ConfigureBuildSettings.cs
│   │
│   ├── Prefabs/                    # 🔲 TODO: Extract prefabs from scenes
│   │   ├── LaserPointer.prefab
│   │   ├── PatternCanvas.prefab
│   │   ├── RecordingIndicator.prefab
│   │   └── UI_Canvas.prefab
│   │
│   ├── Materials/                  # 🔲 TODO: Create dedicated materials
│   │   ├── LaserMaterial_Idle.mat
│   │   ├── LaserMaterial_Active.mat
│   │   └── BurnTrail.mat
│   │
│   └── Settings/                   # ✅ URP render pipeline settings
│
├── Data/                           # Runtime data storage (auto-created at persistentDataPath)
│   ├── Sessions/
│   │   ├── {UserID}_{Timestamp}_trials.csv
│   │   └── {UserID}_{Timestamp}_summary.csv
│   └── Screenshots/
│       └── {UserID}/
│           └── {SessionTimestamp}/
│               └── {UserID}_trial{N}_{Timestamp}.png
│
├── generate_patterns.py            # ✅ Pattern generation script (already run)
├── SettingsScreen.png              # UI reference image
├── PROJECT.md                      # This file
└── README.md                       # Setup instructions
```

---

## Development Roadmap

### Phase 1: Core Setup ✅ COMPLETE
- [x] Generate pattern images (1000 patterns)
- [x] Unity 6000.3.7f1 project with URP, Input System
- [x] Basic scene structure (MainMenu, Settings, TracingSession, Results)
- [x] Build settings configured (MainMenu=0, Settings=1, TracingSession=2, Results=3)
- [x] All data models implemented (SessionSettings, SessionData, TrialData, PatternInfo)
- [x] Core managers implemented (GameManager singleton, SessionManager, TrialManager)
- [x] Pattern loading/selection system (PatternLoader, PatternSelector)
- [x] Input system with VR + desktop fallback (LaserPointer, ControllerInput)
- [x] Tracing system (TracingRecorder with Bresenham line painting, PatternValidator)
- [x] Scoring system (TrialScorer pixel comparison, SessionScorer aggregation)
- [x] Data persistence (CSVWriter, ScreenshotCapture)
- [x] All UI controllers (UserIDEntry, SettingsController, ResultsDisplay, SessionSummary, RecordingIndicator, SessionHUD)
- [x] All 4 scenes populated with UI hierarchies and wired components
- [x] Install Meta XR SDK packages:
  - com.meta.xr.sdk.all v85.0.0 (core, audio, voice, haptics, MR utility kit, platform, interaction)
  - com.unity.xr.management v4.5.4
  - com.unity.xr.oculus v4.5.2
- [x] Android Build Support module installed via Unity Hub (SDK, NDK, JDK)
- [x] Switch platform to Android (done by user)
- [x] Enable Oculus XR plugin (done by user)
- [x] Import patterns as Sprites (1000 textures: Sprite type, Read/Write, Uncompressed, no mipmaps, max 8192)
- [x] OVRCameraRig added to TracingSession scene (LaserPointer wired to CenterEyeAnchor camera)
- [x] Pattern images copied from parent directory into Unity project Assets/Resources/Patterns/

### Phase 2: Core Mechanics — Code Complete, VR Integrated
- [x] Implement head-tracked laser pointer (LaserPointer.cs — gaze raycast with mouse fallback)
- [x] Gaze-based raycast system (Physics.Raycast from camera forward)
- [x] Controller input (ControllerInput.cs — trigger + keyboard/mouse fallback)
- [x] Pattern display system (world-space canvas at 2m distance in TracingSession)
- [x] Recording start/stop functionality (TrialManager.ToggleRecording)
- [x] OVRCameraRig integrated — LaserPointer wired to CenterEyeAnchor camera
- [ ] Wire VR controller trigger via OVRInput or Input Action bindings
- [ ] Test laser pointer with actual VR headset tracking

### Phase 3: Tracing System — Code Complete, Needs Testing
- [x] Burn trail visual effect (TracingRecorder — pixel painting with configurable brush)
- [x] Pixel tracking system (inside/outside target via alpha comparison)
- [x] Coverage tracking (unique target pixels hit count)
- [x] Timer system (TrialManager.TrialTimer, SessionManager time limit)
- [x] Recording indicator (RecordingIndicator.cs — flashing red circle, top-right HUD)
- [ ] Test burn trail rendering on Quest hardware
- [ ] Tune brush radius for different pattern scales

### Phase 4: Scoring & Data — Code Complete, Needs Testing
- [x] Trial scoring algorithm (TrialScorer.ScoreTrial — accuracy + coverage)
  - [x] Accuracy calculation (pixels in target / total traced)
  - [x] Coverage calculation (target pixels covered / total target pixels)
  - [x] Time tracking (duration from start to stop recording)
- [x] Session scoring/aggregation (SessionScorer.CalculateStats — min/max/avg)
- [x] CSV export functionality (CSVWriter — trials + summary files)
- [x] Screenshot capture system (ScreenshotCapture — composite pattern+trace → PNG)
- [x] Data persistence (saves to Application.persistentDataPath)
- [ ] Verify CSV output format with sample data
- [ ] Test screenshot capture on Quest

### Phase 5: UI/UX — Code Complete, Scenes Built
- [x] User ID entry screen (3 letters + 3 numbers validation, skip to "Anon")
- [x] Settings screen implementation
  - [x] Time limit control (up/down buttons, 1-30 min)
  - [x] Pattern count control (up/down buttons, 1-20)
  - [x] Complexity slider (1-5, labeled Simple→Complex)
  - [x] Thickness slider (1-5, labeled Thin 10px→Very Thick 50px)
  - [x] Scale slider (1-10, labeled with resolution)
  - [x] Randomize buttons (both session and level)
- [x] Trial results display (10-second auto-advance with skip button)
- [x] Session summary screen (stats panel, scrollable trial list, navigation)
- [x] HUD elements (trial counter, timer, time remaining, REC indicator)
- [ ] Polish UI for VR readability (font sizes, contrast, world-space adaptation)

### Phase 6: Pattern Management — Code Complete
- [x] Pattern loading system (PatternLoader — Resources.Load with caching)
- [x] Pattern selection logic
  - [x] Sequential (default order within complexity level)
  - [x] Random (shuffle with repeat if needed)
  - [x] Filtered by settings (complexity level filter)
- [x] Pattern scaling system (10 scale sizes via SessionSettings)
- [x] Session configuration system (SessionSettings with Clone for immutability)

### Phase 7: Polish & Optimization — TODO
- [ ] Performance optimization (maintain 72+ FPS on Quest)
- [ ] Visual polish (laser effects, UI animations)
- [ ] Audio feedback (optional)
- [ ] Error handling and edge cases
- [ ] Extract prefabs from scenes
- [ ] Create dedicated laser/trail materials

### Phase 8: Testing & Deployment — TODO
- [ ] Install Meta XR SDK and configure for Quest
- [ ] Device testing (Quest 2/3)
- [ ] User testing
- [ ] Bug fixes
- [ ] Build optimization
- [ ] Final deployment to Quest

---

## Key Design Decisions

### Laser Pointer Origin: Headset Center (Gaze)
**Rationale**: Provides accessible control for all users regardless of hand motor control limitations. Head/gaze tracking is stable and intuitive.

### Either Controller Trigger
**Rationale**: Ambidextrous design allows users to use their preferred hand or switch hands during session without interruption.

### Continuous Recording (No Auto-Stop)
**Rationale**: Gives users full control over when to end trial, allowing for self-correction and multiple passes over difficult sections.

### 10-Second Result Display with Skip Option
**Rationale**: Balances providing adequate time to review results while allowing faster-paced sessions for experienced users.

### CSV Data Format
**Rationale**: Universal format easily imported into analysis tools (Excel, Python, R, SPSS) for research and clinical assessment.

### Anonymous Option
**Rationale**: Reduces friction for casual users or demo scenarios while still allowing proper identification for clinical/research use.

### Pattern Pre-Generation
**Rationale**: Ensures consistent, high-quality patterns across all sessions and devices. Eliminates runtime generation overhead.

---

## Future Enhancement Ideas

### Potential Features (Post-MVP)
- [ ] Multi-user profiles with progress tracking
- [ ] Difficulty auto-adjustment based on performance
- [ ] Training mode with guided tutorials
- [ ] Achievement system / gamification
- [ ] Cloud data sync for multi-device use
- [ ] Detailed analytics dashboard
- [ ] Export reports (PDF)
- [ ] Haptic feedback on controller
- [ ] Audio cues for on/off target
- [ ] Hand controller tracking option (vs. gaze)
- [ ] Custom pattern upload
- [ ] Accessibility options (color blind modes, etc.)
- [ ] Multi-language support

---

## Technical Notes

### Pixel Tracking Implementation
- Use render texture to capture laser pointer hits
- Compare hit positions against pattern mask
- Store hit coordinates for coverage calculation
- Accumulate in-bounds vs. out-bounds pixels

### Performance Considerations
- Batch pattern loading (not all 1000 at once)
- Use object pooling for burn trail segments
- Optimize raycast frequency (once per frame acceptable)
- Compress screenshots before saving

### VR Best Practices
- Keep UI elements at comfortable viewing distance (1.5-2m)
- Avoid rapid color changes (except intentional recording indicator)
- Maintain consistent frame rate
- Minimize latency in laser pointer response
- Follow Meta's VR comfort guidelines

---

## Questions & Decisions Log

### Resolved
1. **Laser pointer control method**: Gaze/head tracking ✓
2. **Off-pattern behavior**: Continue recording ✓
3. **Result display timing**: 10 seconds with skip option ✓
4. **Data format**: CSV ✓
5. **User management**: Single user with optional ID ✓
6. **Laser visual feedback**: Transparent red → bright red with burn trail ✓
7. **Recording indicator position**: Top-right corner of HUD ✓
8. **Pattern display distance**: 2m in front at 1.5m height (world-space canvas) ✓
9. **Session timeout behavior**: Auto-end session when time limit reached ✓
10. **Desktop testing**: Mouse + keyboard fallback (Space/click to toggle recording) ✓

### To Be Determined
1. **Audio feedback**: None currently, could add optional cues
2. **Haptic feedback**: Not specified, could enhance experience
3. **Data backup/sync**: Local only or cloud option?
4. **Pattern display distance fine-tuning**: 2m default, needs VR testing

---

## Contact & Documentation

**Project Lead**: [Your Name]
**Last Updated**: 2026-02-14
**Version**: 1.3
**Status**: Development - Phases 1-6 Code Complete (28 scripts, 4 scenes). Meta XR SDK v85 installed. OVRCameraRig integrated. 1000 patterns imported as Sprites. Pending: VR controller input wiring, on-device testing, polish.

---

## Appendix: Pattern Reference

### Pattern Naming Convention
```
scale_{width}x{height}_width_{linewidth}_level{N}_{patternname}.png

Examples:
- scale_512x512_width_1_level1_horizontal.png
- scale_1920x1080_width_3_level4_circle.png
- scale_7680x4320_width_5_level5_heart.png
```

### Pattern Categories
- **Scales**: 10 resolutions (512x512 to 7680x4320)
- **Line Widths**: 5 levels (10px, 20px, 30px, 40px, 50px)
- **Complexity**: 5 levels (20 patterns across 5 levels)
- **Total**: 1000 pattern variations

### Recommended Difficulty Progression
1. **Beginner**: Level 1-2, Thick lines (40-50px), Large scale
2. **Intermediate**: Level 3, Medium lines (30px), Medium scale
3. **Advanced**: Level 4-5, Thin lines (10-20px), Any scale
4. **Expert**: Level 5, Thin lines (10px), Small scale
