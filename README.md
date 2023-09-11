
# Quill Framework
<p align="center">
  <img src="./docs/images/Quill_Framework_Logo.png" />
</p>

Quill Framework is a feature-rich classic Sonic-inspired 2D framework for the Unity Engine. This framework originally started off as a fun sandbox project for me to learn how to program in C#. Like any sandbox-type project, I didn't have an end goal in mind, but I think where it is right now is the perfect starting point for other people.

The main focus of this framework was to keep things as "Unity" as possible while being as close to Sonic as I could vision. This means if you understand a bit of Unity stuff like the Animator system, 2D colliders, and all that good Unity stuff, this project will be less foreign to you.

## 🚀 Key Features
- Classic Sonic 360-styled physics in a way familiar to Unity
- 4 Playable sonic characters with unique abilities
- A special click-and-drop editor for those who just want to make their own levels
- Uncapped Speeds, you can make Sonic and his friends go as fast as you want to go with no restrictions
- Half-pipe special stages with a completely different foundation
- Parallax Scrolling
- Multiple Familiar Badniks
- Player Hazards
- Underwater Physics
- Multiple shields and shield abilities
- A Debug Mode that lets you manipulate the world in real-time
- Multiple Save Slots and a No save slot if that's what you're into
- Multiple Acts to play around with
- Cutscenes at the start and end of Acts
- Pad and Keyboard support
- Mult-platform support
- And Many More!

## ℹ️ Prerequisites
- Install [Unity 2021.3.8f](https://unity.com/releases/editor/whats-new/2021.3.8)
- Install [Git(Windows)](https://git-scm.com/download/win)

## 🎮 Controls 
```
These gamepad controls assume the user is using a controller that follows the Xbox control scheme

```

### Default Control Scheme
| Action | Keyboard | Gamepad |
| :---         |     :---:      |         :---:  |
| Jump  |![Static Badge](https://staging.shields.io/badge/Space-8A2BE2)|![Static Badge](https://staging.shields.io/badge/W-blue)|
| Transform     |![Static Badge](https://staging.shields.io/badge/W-8A2BE2)       |![Static Badge](https://staging.shields.io/badge/Y-blue)|
| Pause     |![Static Badge](https://staging.shields.io/badge/Enter-8A2BE2)      | ![Static Badge](https://staging.shields.io/badge/Start-blue)|


### Debug Control Scheme
| Action | Keyboard | Gamepad |
| :---         |     :---:      |          :---:   |
| Jump  |![Static Badge](https://staging.shields.io/badge/Space-8A2BE2)|![Static Badge](https://staging.shields.io/badge/A-blue)|
| Transform     |![Static Badge](https://staging.shields.io/badge/W-8A2BE2)|![Static Badge](https://staging.shields.io/badge/Y-blue)|
| Debug Ring Mode     |![Static Badge](https://staging.shields.io/badge/Q-8A2BE2)|![Static Badge](https://staging.shields.io/badge/X-blue)|
| Pause  |![Static Badge](https://staging.shields.io/badge/Enter-8A2BE2)|![Static Badge](https://staging.shields.io/badge/Start-blue)|
| Moon Jump     |![Static Badge](https://staging.shields.io/badge/Left%20Ctrl-8A2BE2)|![Static Badge](https://staging.shields.io/badge/LT-blue)|
| Palette Swap     |![Static Badge](https://staging.shields.io/badge/Left%20Shift-8A2BE2)|![Static Badge](https://staging.shields.io/badge/LB-blue)|
| Toggle Debug UI |![Static Badge](https://staging.shields.io/badge/←%20Backspace-8A2BE2)|![Static Badge](https://staging.shields.io/badge/RT-blue)|
| Swap Character    |![Static Badge](https://staging.shields.io/badge/Right%20Shift-8A2BE2)|![Static Badge](https://staging.shields.io/badge/RB-blue)|
| All Emeralds     |![Static Badge](https://staging.shields.io/badge/Right%20Ctrl-8A2BE2)|![Static Badge](https://staging.shields.io/badge/Options-blue)|
| Grant Regular Shield  |![Static Badge](https://staging.shields.io/badge/0-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|
| Grant Bubble Shield     |![Static Badge](https://staging.shields.io/badge/1-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|
| Grant Electric Shield     |![Static Badge](https://staging.shields.io/badge/2-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|
| Grant Flame Shield  |![Static Badge](https://staging.shields.io/badge/3-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|
| Grant Invincibility    |![Static Badge](https://staging.shields.io/badge/4-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|
| Grant Speed Shoes     |![Static Badge](https://staging.shields.io/badge/5-8A2BE2)|![Static Badge](https://staging.shields.io/badge/N%2FA-gray)|

## 🌟 Abilities

### Sonic
- While Sonic has no shield holding down the Jump button in the air will trigger the drop dash
- With Shield active, hitting the Jump button causes different actions based on the Shield active
  - Flame Shield: This will cause Sonic to dash forward.
  - Bubble Shield: This will cause Sonic to move his velocity towards the ground and bounce off the terrain.
  - Electric Shield: This Will grant Sonic an extra Jump.

### Tails
- Tails play similarly to Sonic except with no shield abilities or drop dash
- On the other hand, while in the air pressing the Jump button again will cause Tail's fly upwards as long as he has the stamina for it
- While flying Tail's Tails' have the ability to deflect bullets and destroy Badniks
- When underwater, Tails will swim instead of flying

### Knuckles
- While Knuckles is jumping, pressing down the Jump button again will cause Knuckles to glide
- When Knuckles is in his gliding state, if he runs into a wall, he will begin Climbing the wall upwards or downwards based on the Players input
- Knuckles can also change his direction at any point while Gliding by hitting the direction opposite to where he is currently Gliding
- Knuckles fists also have the ability to deflect bullets and destroy Badniks while gliding
- Knuckles also has the ability to smash through walls without having to roll into a ball

### Super Sonic
- Super Sonic is similar to Sonic, but he instead of shield abilities and drop dashes, in this form Sonic is giving access to the homing attack, which instantly moves Super Sonic in the direction of enemies or gimmicks
- If there are no targets, Super Sonic will simply dash forward in the direction he is facing

## 📬 Discord
If you have any questions you don't want to ask here or just need help, [Discord](https://discord.gg/mht9ys9xxZ) is always an option

## 📷 Media
![Media 1](./docs/images/Quill_framework_Image_0.png)
![Media 2](./docs/images/Quill_framework_Image_1.png)
![Media 3](./docs/images/Quill_framework_Image_2.png)
![Media 4](./docs/images/Quill_Framework_Image_3.png)
![Media 5](./docs/images/Quill_Framework_Image_4.png)
![Media 6](./docs/images/Quill_Framework_Image_5.png)

## 📇 Credits

| Area             | People                                                                |
| ----------------- | ------------------------------------------------------------------ |
| Project Lead| DamiTheHuman|
| Special Thanks & Inspiration| Giometric, Triangly, Nihil (Nullspace), Lake Fepard|
| Sounds | Sega Sound Team, Yasuhisa Watanabe |
| Additional Programming | Giometric, DW, Damizean, Nihil(Nullspace), Azu, Lark SS, Team Orbinaut |
| Sprites | DamiTheHuman, Random Talking Brush, SonicDash57,DBurraki, AkumaTH, Darkness3313, Cyclone, AshuraMoon, Homingmissile333 |
| Beta Testers | Foxeh, Strix, ShadowMasterx5, Frenzy-Kun, MetalMadness2006|

## 📜 Terms of Use
- Please give Credit to everyone who has helped develop & inspire this framework. Without them, it would not be possible.
- Feel free to use this framework for your projects, but DO NOT commercialise anything "Sonic The Hedgehog" related.
