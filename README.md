# Regulation-Grounded Adaptive PCGML Framework for AR Construction Safety Training

## Overview

This project is a Unity-based research prototype for adaptive construction-safety training using Augmented Reality (AR).

The system combines:

- BIM/site-inspired context input
- Local Large Language Model (LLM) generation
- Structured JSON scenario generation
- Safety-rule validation
- Procedural AR scenario generation
- Hazard identification
- Safety-control questions
- Performance scoring and feedback

The current prototype focuses on two construction hazard categories:

- Struck-By
- Electrocution

The broader research goal is to develop a regulation-grounded adaptive PCGML framework for AR/VR construction-safety training.

---

## System Workflow

```text
BIM / Site Context Input
        ↓
Worker Level Selection
        ↓
Llama 3.2 3B via Ollama
        ↓
Structured JSON Scenario
        ↓
Safety Rule Validation
        ↓
Hazard Prefab Selection
        ↓
Procedural Spatial Variation
        ↓
AR Surface Placement
        ↓
Hazard Identification
        ↓
Safety-Control Question
        ↓
Scoring and Feedback
```

## Main Features
<ul> 
  <li> Unity 6.3 LTS </li>
  <li> AR Foundation </li>
  <li> Google ARCore XR Plugin </li>
  <li>Android deployment</li>
  <li> BIM-inspired site-context input</li>
  <li> Local LLM integration using Ollama</li>
  <li> Llama 3.2 3B</li>
  <li> JSON-based procedural scenario generation</li>
  <li> Rule-based scenario validation</li>
  <li> AR tabletop placement</li>
  <li> Hazard interaction through physics raycasting</li>
  <li> Multiple-choice safety-control questions</li>
  <li> Difficulty-based timing</li>
  <li> Performance scoring</li>
  <li> Deterministic fallback if LLM generation fails</li>
</ul>

## Current Hazard Scenarios
<h3> Struck-By </h3>

The trainee identifies an unsecured overhead object or similar struck-by hazard.

Example question:

```text
A. Walk quickly through the area
B. Secure or remove the overhead object and restrict access below
C. Continue working because the object has not fallen yet
```
Correct answer:

```text
B / Index 1
```

<h3> Electrocution </h3>

The trainee identifies a damaged electrical cable or similar electrical hazard.

Example correct control:

```text
Remove the damaged cord from service and correct the hazard before use.
```
Correct answer:

```text
A / Index 0
```

## Technology Stack
<h3> Unity </h3>
<ul> 
  <li> Unity 6.3 LTS </li>
  <li> AR Foundation </li>
  <li> Google ARCore XR Plugin </li>
  <li> Unity Input System </li>
  <li> TextMeshPro </li>
  <li> Android Build Support </li>
</ul>

<h3> AI / LLM </h3>
<ul>
  <li> Ollama </li>
  <li> Llama 3.2 3B </li> 
  <li> Local HTTP API </li>
  <li> Structured JSON generation </li> 
</ul>

<h3> Target Platform </h3>
<ul>
  <li> Android smartphone with ARCore support </li> 
</ul>

## Example LLM Output

```text
{
  "hazardType": "StruckBy",
  "difficulty": "Intermediate",
  "hazardOffsetX": 0.02,
  "hazardOffsetZ": 0.05,
  "workerOffsetX": -0.01,
  "workerOffsetZ": -0.03,
  "distractorCount": 1
}
```
The generated scenario is validated before it is instantiated in Unity.

## Site Context Inputs

The current prototype uses simplified BIM-inspired structured input.

Example inputs:
<ul>
  <li>Work Area</li>
  <li>Elevated Platform Present</li>
  <li>Electrical Equipment Present</li>
  <li>Overhead Work Present</li>
  <li>Worker Level</li>
</ul>

Example work areas:

<ul>
  <li>General Construction</li>
  <li>Electrical Maintenance</li>
  <li>Elevated Work</li>
</ul>

Worker levels:

<ul>
  <li>Beginner</li>
  <li>Intermediate</li>
</ul>

## Safety Validation

Example rules:

```text
Elevated Work + Overhead Work
→ Struck-By

Electrical Maintenance
→ Electrocution

Electrical Equipment Present
→ Electrocution
```
The validator can:

<ul> 
  <li>Repair invalid LLM hazard selections</li>
  <li>Enforce selected difficulty</li>
  <li>Clamp spatial offsets</li>
  <li>Limit distractor counts</li>
  <li>Use deterministic fallback generation</li>
</ul>
  
## Important Scripts
<h3> BIMContextManager </h3>

Reads construction-site context and worker level and sends the information for scenario generation.

<h3> LLMScenarioGenerator</h3>

Communicates with Ollama, receives JSON output, parses the result, and validates the generated scenario.

<h3> ARPlacement</h3>

Places the selected scenario prefab on a detected horizontal AR surface.

<h3> ScenarioRandomizer</h3>

Applies generated worker and hazard offsets to create procedural variation.

<h3> HazardTapDetector</h3>

Detects trainee touch input and identifies hazard objects using physics raycasting.

<h3> HazardTarget </h3>

Stores:

<ul>
  <li>Hazard name</li>
  <li>Hazard category</li>
  <li>Safety-control question</li>
  <li>Option A</li>
  <li>Option B</li>
  <li>Option C</li>
  <li>Correct option index</li>
</ul>

Answer indexing:

```text
A = 0
B = 1
C = 2
```
<h3> TrainingUIManager </h3>

Controls:

Hazard feedback
Safety-control questions
Answer buttons
Correct/incorrect feedback

<h3> TrainingSessionManager </h3>

Controls:

Session timer
Hazard score
Safety-control score
Time bonus
Final score

## Scoring System

<table>
  <tr> 
    <td>Activity	Score</td>
    <td> Score</td>
  </tr>

  <tr>
    <td>Correct hazard identification</td>
    <td>	50</td>
  </tr>

  <tr>
    <td>Correct safety-control selection</td>
    <td>	30</td>
  </tr>

  <tr>
    <td>Completion within time</td>
    <td>	20</td>
  </tr>

  <tr>
    <td>Maximum score</td>
    <td>	100</td>
  </tr>
</table>

Difficulty timing:

<table>
  <tr>
    <td>Difficulty/td>
    <td>Time</td>
  </tr>

  <tr>
    <td>Beginner/td>
    <td>45 seconds/td>
  </tr>

  <tr>
    <td>Intermediate/td>
    <td>30 seconds/td>
  </tr>
</table>

## Requirements

Install:

<ul>
  <li>Unity 6.3 LTS</li>
  <li>Android Build Support</li>
  <li>AR Foundation</li>
  <li>Google ARCore XR Plugin</li>
  <li>Ollama</li>
</ul>

The Android phone and development computer should be connected to the same Wi-Fi network when using the local LLM server.

## Ollama Setup

Install the model:

```text
ollama pull llama3.2:3b
```
Run the model:

```text
ollama run llama3.2:3b
```
Run Ollama for LAN access:

```text
pkill ollama
OLLAMA_HOST=0.0.0.0:11434 ollama serve
```
Find the Mac Wi-Fi IP address:

```text
ipconfig getifaddr en1
```
Example Unity API endpoint:

```text
http://192.168.1.104:11434/api/generate
```
Replace the IP address with the current IP address of the machine running Ollama.

## Unity Android Configuration

Recommended settings:

```text
Graphics API: OpenGLES3
Minimum API Level: 29 or higher
Active Input Handling: Input System Package (New)
```
Required AR scene components:

```text
AR Session
XR Origin (Mobile AR)
Main Camera
AR Plane Manager
AR Raycast Manager
EventSystem
```

<h3> Running the Project </h3>

<ol>
  <li>Start Ollama.</li>
  <li>Connect the Android phone and computer to the same Wi-Fi network.</li>
  <li>Verify the Ollama IP address in Unity.</li>
  <li>Build and run the project on Android.</li>
  <li>Allow camera access.</li>
  <li>Scan a horizontal surface.</li>
  <li>Select the site context and worker level.</li>
  <li>Press Generate.</li>
  <li>Wait for LLM scenario generation.</li>
  <li>Tap the detected surface to place the scenario.</li>
  <li>Identify the hazard.</li>
  <li>Answer the safety-control question.</li>
  <li>Review the score and feedback.</li>
</ol>

## GitHub and Large Files

Do not upload Unity-generated folders:

```text
Library/
Temp/
Logs/
Obj/
Build/
Builds/
UserSettings/
```
For large 3D assets and textures, use Git LFS:

```text
git lfs install
git lfs track "Assets/TextureHaven/**"```

```
Verify tracked files:

```text
git lfs ls-files
```

## Current Limitations

The current prototype:

<ul>
  <li>Supports only Struck-By and Electrocution</li>
  <li>Uses simplified BIM/site context</li>
  <li>Does not directly parse IFC files</li>
  <li>Uses a pretrained Llama 3.2 3B model</li>
  <li>Does not train a new PCGML model</li>
  <li>Uses rule-based scenario validation</li>
  <li>Uses tabletop AR</li>
  <li>Uses a limited 3D asset library</li>
  <li>Has not yet completed formal expert or user evaluation</li>
</ul>

The current implementation can be described as:

An LLM-conditioned procedural generation proof-of-concept for adaptive construction-safety AR training.

## Future Work

Planned extensions include:

<ul>
  <li>All OSHA Focus Four hazards</li>
  <li>BIM/IFC integration</li>
  <li>Regulation-grounded retrieval</li>
  <li>Accident-record integration</li>
  <li>More construction environments</li>
  <li>More procedural variation</li>
  <li>Adaptive trainee profiles</li>
  <li>Personalized difficulty</li>
  <li>Expert validation</li>
  <li>User studies</li>
  <li>VR support</li>
  <li>PCGML model training and comparison</li>
</ul>

## Research Question

<h4>How can a regulation-grounded PCGML framework automatically generate valid, diverse, and adaptive construction-safety training scenarios from multimodal project information?</h4>

Supporting questions:

<ol>
  <li>How can BIM/site context and safety information be transformed into controllable scenario representations?</li>
  <li>How can generated scenarios be automatically validated against safety regulations and expert knowledge?</li>
  <li>How can scenario generation adapt to trainee performance and difficulty requirements?</li>
</ol>

## References
<h3>OSHA Construction Focus Four</h3>

<a href="https://www.osha.gov/training/outreach/construction/focus-four"> Occupational Safety and Health Administration </a>

## PCGML

A. Summerville et al.,
“Procedural Content Generation via Machine Learning,”
IEEE Transactions on Games, vol. 10, no. 3, pp. 257–270, 2018.

DOI: 10.1109/TG.2018.2846639

## Project Status

Status: Functional Research Prototype

Implemented:

<ul>
  <li>AR plane placement</li>
  <li>LLM integration</li>
  <li>Structured scenario generation</li>
  <li>Safety validation</li>
  <li>Struck-By scenario</li>
  <li>Electrocution scenario</li>
  <li>Procedural spatial variation</li>
  <li>Hazard interaction</li>
  <li>Safety-control questions</li>
  <li>Scoring and feedback</li>
</ul>

## Video

<a href="https://youtube.com/shorts/xRqEMUW0bVk?si=m-CpdMVF10u1iwNH"> Prototype Video </a>





