# Before It Spoils

[English](README.md) | [简体中文](README.zh-CN.md)

A Unity 2D game prototype that connects food freshness, cooking, and virtual pet care. Store ingredients in a fridge, turn them into meals before they spoil, and help your pet grow.

Created by **Muzi Liu (刘木子)**.

## The idea

Before It Spoils makes the passage of time visible through changing food sprites and the consequences of cooking with ingredients at different freshness stages. Caring for a pet gives the player a reason to pay attention to what is left in the fridge.

## Features

- **Three connected pages:** navigate between ingredient selection, the fridge, and pet care with horizontal swipes.
- **Drag-and-drop storage:** move ingredients into the fridge and rearrange them.
- **Visible food freshness:** ingredients progress through fresh, stale, and rotten states.
- **Two-ingredient cooking:** combine ingredients to produce dishes based on their categories and freshness.
- **Pet progression:** meals affect growth and mood; reaching growth thresholds increases the pet's level and changes its appearance.
- **Animated feedback:** cooking, dish presentation, feeding, and sound effects support the interaction loop.

The project includes eight ingredient types: beef, chicken, fish, egg, milk, spinach, tomato, and blueberry. Ingredient definitions use Unity ScriptableObjects.

## Open the project

### Requirements

- Unity Hub and **Unity Editor 6000.4.8f1**, the version recorded in `ProjectSettings/ProjectVersion.txt`.
- Internet access for package installation when opening a fresh clone.

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/mz1519/before-it-spoils.git
   ```

2. In Unity Hub, add the cloned `before-it-spoils` folder as an existing project.
3. Open it with Unity **6000.4.8f1** and allow Unity to import assets and resolve packages.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play** and interact in the **Game** view.

The manifest declares Unity AI Assistant `2.9.0-pre.2`. Its embedded local copy is excluded from Git. A fresh clone relies on Package Manager to resolve this dependency; availability of that exact version has not been verified. If Unity reports a package error, check the Assistant dependency in Package Manager before continuing.

## How to play

| Action | Control |
| --- | --- |
| Change pages | Click and drag horizontally on the page background. |
| Store an ingredient | Drag it from the ingredient page toward the right edge to switch to the fridge, then release it on the fridge page. |
| Rearrange stored food | Drag an ingredient within the fridge. |
| Prepare an ingredient for cooking | Press and hold a stored ingredient for about one second. It moves into an available food slot on the pet page. |
| Cook a meal | Drag two ingredients from the pet page's food slots into the cooking pot. |
| Feed the pet | Wait for the finished dish to be displayed; the current scene feeds it automatically. |

### Freshness and recipes

The prototype uses accelerated timing: by default, placed food stays fresh for **5 seconds**, becomes stale for **5 seconds**, and then turns rotten. `FoodTimer` controls this timing; the `staleDays` and `rottenDays` fields in ingredient data do not currently drive the countdown.

Example combinations:

| Ingredients | Dish |
| --- | --- |
| Protein + vegetable | Stew |
| Egg + vegetable | Egg soup |
| Egg + milk | Creamy egg pudding |
| Fruit + milk | Smoothie |
| Two vegetables | Garden salad |

Stale ingredients halve the dish's growth and mood rewards. Any rotten ingredient produces a failed dish that reduces growth and mood.

## Project structure

| Path | Contents |
| --- | --- |
| `Assets/Scenes/` | Main playable scene. |
| `Assets/Script/` | Interaction, freshness, cooking, pet, animation, and audio scripts. |
| `Assets/FoodData/` | ScriptableObject ingredient definitions. |
| `Assets/FoodImage/` | Food artwork and freshness sprites. |
| `Assets/Prefabs/` | Reusable game objects. |
| `Assets/Animation/`, `Assets/Pet/`, `Assets/Audio/` | Animation, pet, and audio assets. |
| `Packages/manifest.json` | Package dependencies. |
| `ProjectSettings/` | Unity project configuration. |

The main gameplay systems are `FoodTimer` for freshness, `CookingPotManager` for recipes, `PetStatsManager` for progression, and `PageSwiper` for navigation.

## Prototype notes

- Game progress is held in memory; the current gameplay scripts do not implement a save/load system.
- Timers, rewards, and UI behavior are prototype values and can be adjusted in scripts or the Inspector.
- This repository contains the Unity source project. Generated builds, caches, logs, and local editor settings are excluded through `.gitignore`.
- Keep Unity `.meta` files alongside their assets when making changes; they preserve references used by scenes and prefabs.
