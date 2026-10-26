# Lettra

A mobile word puzzle game built with Unity and C#.

<img width="421" height="755" alt="image" src="https://github.com/user-attachments/assets/447ff7a6-fd11-447d-943a-f9b303241059" />

Lettra challenges players to create valid words from a limited letter board, complete stage objectives, manage limited word attempts, and use Bonus Letters to build score and trigger special gameplay phases.

## Features

- Word creation from a letter grid
- Dictionary-based word validation
- Weighted letter generation
- Score based on word length
- Limited word attempts
- Stage-specific letter objectives
- 3-star score progression
- Three Bonus Letter types:
  - **+50 Points** — adds 50 points
  - **+30% Points** — increases the word score by 30%
  - **+Gauge** — increases Bonus Gauge gain by 50%
- Bonus Letters can stack when multiple bonuses are used in the same word
- 10-second Bonus Phase
- Automatic longest-word generation during Bonus Phase
- Stage Completion reward sequence
- Stage Complete and Stage Fail screens
- Animated UI using DOTween

## Gameplay

1. Select letters from the board to form a word.
2. Press **Check** to validate the word.
3. Valid words award points and replace their used letters.
4. Invalid words return the selected letters to their original positions.
5. Complete the stage's objectives before running out of normal word attempts.
6. Use Bonus Letters to increase score and Bonus Gauge.
7. Fill the Bonus Gauge to activate the Bonus Phase.

## Bonus Phase

When the Bonus Gauge reaches 100%, the Bonus Phase begins for 10 seconds.

During the Bonus Phase:

- The gauge remains full.
- Normal gauge generation is disabled.
- The player selects one letter from the board.
- Lettra automatically finds the longest valid word that can be made using that letter and the available board letters.
- The word is automatically filled and checked after a short delay.
- The action does not consume a normal word attempt.

## Stage Completion

Completing all stage objectives starts a final reward sequence instead of immediately ending the level.

Remaining normal word attempts are converted into Bonus Letters, which are automatically processed to create additional words and score opportunities.

After the sequence finishes, the **Stage Complete** screen appears.

## Tech Stack

- Unity
- C#
- TextMeshPro
- Newtonsoft.Json
- DOTween
- GUI PRO Kit – Casual Game

## Development History

The project was developed incrementally from a basic prototype into a complete stage-based word game.

| Date | Commit | Development |
|---|---|---|
| 2026-08-31 | `e640660` | Initial project setup |
| 2026-08-31 | `a3527cd` | Core gameplay prototype |
| 2026-09-05 | `f6e04b8` | Bonus Gauge adjustments and GUI/DOTween integration |
| 2026-09-05 | `cda0ba1` | Project/content updates |
| 2026-09-10 | `f6e75d2` | Stage unlocking and presentation work |
| 2026-09-10 | `43948b5` | Added gameplay mechanics documentation |
| 2026-09-24 | `b28664c` | UI overhaul |
| 2026-09-24 | `be2ceff` | Star progression pop animation |
| 2026-09-24 | `39f78e0` | Removed PSD files |

## Repository

[GitHub Repository](https://github.com/ducthang140/Typewriter)
