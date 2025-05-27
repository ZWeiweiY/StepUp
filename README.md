
# 🏆 StepUp - Hygiene Hero Cup 🏆

**A motion-driven educational game empowering hygiene through play.**  
Created by [**Team StepUp**](https://projects.etc.cmu.edu/step-up) @ Carnegie Mellon University Entertainment Technology Center, in collaboration with the [**World Shoe Fund**](https://www.worldshoefund.org/).

---

## 🚀 Overview

**Hygiene Hero Cup** is a 3D educational game designed for Android tablets, blending fantasy adventure with real-world health education. It teaches essential hygiene practices—like handwashing, foot care, and the importance of wearing shoes—to children ages 10–14 in underserved communities, using interactive gameplay and sports-inspired mini-games.

This game was developed during a semester-long project with a team of six graduate students, and I served as the **sole programmer** responsible for every gameplay system, feature, and technical decision.

Explore the project details [here](https://projects.etc.cmu.edu/step-up/about)

---

## 🎮 Game Features & Technical Highlights

### Mini 1: High-Jump Handwashing Challenge 🏃🏿‍♂️‍➡️
> Combining traditional African sports with hygiene steps

- **Mechanic**: A physics-based jump system designed using a parabolic motion equation:
  ```
  y = a * x^2 + b * x + c
  ```
- **Implementation**: Trajectory was calculated in real-time using 3D distance, player input, and game context to adjust jump height and speed.
- **Design Consideration**: Fine-tuned for natural motion response and embedded in a gameplay state machine.

### Mini 2: Soap Surfing Adventure 🧼
> Players surf across a giant foot on a bar of soap, learning to clean thoroughly while having fun

- **Movement System**:
  - Custom-built controller driven by **surface normals** and **camera-relative movement vectors**.
  - Aligned gravity with terrain using downward **raycasts** and adapted dynamically to simulate slope-based physics.
  - Integrated **wall climbing** and **momentum preservation** for responsive motion.
- **Visuals**:
  - Character tilts and reacts to turns with dynamic lean animations.
- **Other Mechanics**:
  - Enemy spawns, collisions, and scoring system all tied to Unity’s event-driven system.

### Mini 3: Shoe Dash Challenge 👟
> Race against time in a modular endless runner

- **Level System**:
  - Built with reusable **abstract classes and inheritance** for coins, power-ups, and obstacles.
  - **Terrain spawning** and **stage progression** handled via a modular system.
- **Design Tools**:
  - Used **UnityEvents** to allow designers to control interactions visually in-editor.
  - Enabled drag-and-drop level design without requiring code changes.

---

## 🔧 Engineering Systems

- **Design Patterns Used**:
  - **Observer Pattern** for UI and gameplay feedback using Unity Events.
  - **Flyweight Pattern** using ScriptableObjects to manage reusable configuration.
  - **Singletons** for managing global managers (with planned evolution to Dependency Injection).
- **State Management**:
  - All minigames use clear state-driven architecture to manage transitions, win/loss conditions, and inputs.
- **Video Playback Fix**:
  - Solved Unity’s video stuttering issue with an asynchronous preloading system for a smooth player experience.

---

## 🌍 Social Impact

StepUp’s mission goes beyond entertainment. It’s a partnership with the [**World Shoe Fund**](https://www.worldshoefund.org/), aimed at tackling preventable diseases through education. Through engaging mini-games and intuitive lessons, the Hygiene Hero Cup game reinforces:
- **UNICEF WASH principles**
- Importance of wearing protective footwear
- Foot hygiene and handwashing techniques
- Empowerment through play in schools and campaign centers

---

## 📸 Media

View our:
- [🎥 Final Trailer](https://projects.etc.cmu.edu/step-up/media/)
- [📜 Poster & Half-Sheet](https://projects.etc.cmu.edu/step-up/media/)
- [📝 Weekly Development Blog](https://projects.etc.cmu.edu/step-up/blog/)

---

## 👨‍💻 About Me

This project was personally meaningful to me as the **sole engineer** on the team. Every system, from physics to UI, was built with scalability, iteration speed, and future extensibility in mind. I used this opportunity to sharpen my skills in:
- Gameplay architecture
- Physics and math modeling
- Real-time input systems
- Cross-disciplinary collaboration with designers and artists

And most importantly—I got to see the game bring joy and education to children. That’s the power of games.

---

## 📱 Platform & Tools

- **Engine**: Unity (C#)
- **Target**: Android Tablet
- **Team Size**: 6
- **Development Time**: 1 semester (~14 weeks)

---

## 📂 Project Structure

```
Assets/
├── Scripts/
│   ├── Helper/
│   ├── Input/
│   ├── Interactables/
│   ├── Manager/
│   ├── Player/
│   ├── UI/
│   ├── Utilities/
├── Prefabs/
├── Scenes/
├── ScriptableObjects/
 
 ...
```

---

## 🙌 Acknowledgments

- The [**World Shoe Fund**](https://www.worldshoefund.org/) for their inspiring mission and partnership
- [**CMU ETC**](https://www.etc.cmu.edu) faculty for guidance and playtesting support
- All the kids who played our game—you made it worth building 👶🏿

---

## 🔗 Learn More

🌐 [**Project Website**](https://projects.etc.cmu.edu/step-up) 

🎓 [**Carnegie Mellon University - ETC Press**](https://www.etc.cmu.edu/blog/stepup-world-shoe-fund/)

🌉 [**Hot Metal Campus Press**](https://www.hotmetalcampus.com/news-research/stepping-toward-change-stepup-amp-the-world-shoe-fund)

👣 [**World Shoe Fund**](https://www.worldshoefund.org/)

---

*Empowering hygiene through play, one jump, surf, and dash at a time.*
