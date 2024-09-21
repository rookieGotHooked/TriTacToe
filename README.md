1. Game rules:
- Create a three-in-a-row of the same symbol to score a point. Any scored three-in-a-row will be clear off the board.
- When there are no empty tile left on the board, the game will clear all symbol on the board.
- After a certain time of board clear, the game will select a random rule of Sudden Death to break the balance status on the next time the board is full.
- Reach 3 points first to win.

2. Game (currently) includes:
- Local 2P: Player versus Player.
- Versus AI: Player versus AI.
- 3 difficulties for versus AI:
  +) Random: AI mark tiles randomly on the board.
  +) Normal: Basic AI prioritize on defense.
  +) Hard: Improved AI, marking tiles base on tile scoring.
- Game animations / transitions.
- Sound effects on UI interactions.

3. Noticeable short-comings:
- Long-duration UI animations may cause frustration due to excessive waiting time.
- Color-change of button between interactable and non-interactable also cause frustration in interaction.
- Some button have hidden function that the player cannot (and would not) notice right away.
- Only support 2340x1080 resolution; does not support other resolution (and possibly cause weird screen transitions).
- SFX Bug - SFX volume does not change on SFX volume slider value change in Settings.
- No BGM yet.
- Project still cluttered with unused classes and assets / prefab (no project-refactoring yet).
- Some game UIs / assets got pixelated due to import choices.
