# 🧠 ZEN-EYE Pro

**ZEN-EYE Pro** is a Unity-based VR platform that integrates **eye-tracking, focus training, working memory experiments, and vision training**.  
It is designed to assess and enhance cognitive performance by combining **gaze tracking, blink metrics, real-time feedback, and interactive VR tasks**.

---

## 🚀 Features

### 🔹 ZEN-EYE Pro (Brain Fatigue Assessment)
- Collects **3D gaze coordinates, pupil diameter, and blink metrics** using VR eye-tracking.
- Real-time **heatmap visualization** of gaze patterns.
- Structured CSV export for machine learning and statistical analysis.
- Designed for **fatigue detection** and **cognitive state monitoring**.

### 🔹 Focus Training
- VR breathing exercises guided by a visual breathing sphere and calming audio.
- Adaptive difficulty to sustain attentional focus.
- Supports **biofeedback integration** (gaze, pupil, and optionally heart rate).
- Tracks performance engagement across sessions.

### 🔹 Working Memory Performance (Flash Mental Arithmetic)
- Timed arithmetic tasks with four conditions:
  1. **Normal** (no distraction)
  2. **Audio distraction**
  3. **Visual distraction**
  4. **Combined audio + visual distraction**
- Managed by **AnswerManager.cs** and **FlashManager.cs**.
- Logs **accuracy, reaction time, and error patterns** for performance evaluation.

### 🔹 Vision Training
- Gaze-based interactive tasks for **pursuit tracking, fixation stability, and distractor filtering**.
- Dynamic difficulty adjustment to fit user skill level.
- Ideal for **sports training, rehabilitation, and attention enhancement**.

---

## 📂 Project Structure

```plaintext
ZEN-EYE-Pro/
│── Scripts/                         # Core Unity C# scripts
│   ├── AnswerManager.cs             # Handles user answers during tasks
│   ├── FlashManager.cs              # Controls arithmetic sessions
│   ├── FocusExperimentManager.cs    # Runs focus training experiments
│   ├── EyeTrackingController.cs     # Collects gaze + pupil data
│   ├── GazeHeatmapAnalyzer.cs       # Generates gaze heatmaps
│   ├── VisionTrainingController.cs  # Controls vision training sessions
│   ├── VisualDistractionManager.cs  # Manages visual distractions
│   ├── WebAPIController.cs          # API endpoints for data transfer
│   ├── SceneManagerScript.cs        # Scene transitions
│   └── ... (other controllers, utilities, and audio scripts)
│
│── Data/                            # Exported CSV logs (gaze, blinks, performance)
│── Media/                           # VR assets, videos, and training animations
│── README.md                        # Project documentation


🧩 Usage
1. Brain Fatigue Experiments

Open the ZEN-EYE Pro Scene.

Run with a VR headset that supports eye-tracking.

Data (gaze + blinks) saved automatically in Data/.

2. Focus Training

Open the FocusSphere Scene.

User follows the breathing animation with gaze and rhythm.

Logs engagement metrics per session.

3. Working Memory Performance

Open the Flash Mental Arithmetic Scene.

Choose distraction condition (Normal, Audio, Visual, Combined).

Accuracy and response time stored in CSV files.

4. Vision Training

Open the Vision Training Scene.

Select task type (pursuit, fixation, distractor filtering).

Results logged for progress tracking.


📊 Data Output

Each experiment generates structured CSVs with:

Timestamps

Gaze Coordinates (X, Y, Z)

Pupil Diameter

Blink Metrics (duration, frequency, bursts, intervals)

Task Performance (accuracy, reaction time, scores)
