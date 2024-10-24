# Tri-Tac-Toe!

The tic-tac-toe game now comes with a twist! (made entirely for learning purposes)

## 1. Game rules:
- Create a three-in-a-row of the same symbol to score a point. Any scored three-in-a-row will be clear off the board.
- When there are no empty tile left on the board, the game will clear all symbol on the board.
- After a certain time of board clear, the game will select a random rule of Sudden Death to break the balance status on the next time the board is full.
- Reach 3 points first to win.

## 2. Game (currently) includes:
- 2 game modes:
  + Local 2P: Player versus Player.
  + Versus AI: Player versus AI.
- 3 difficulties for versus AI:
  + Random: AI mark tiles randomly on the board.
  + Normal: Basic AI prioritize on defense.
  + Hard: Improved AI, marking tiles base on tile scoring:
    * Block move = 0.75f (for blocking any three-in-a-row; blocking n number of three-in-a-row would yield result as n * 0.75f)
    *	Score move = 1f (for make a three-in-a-row; a move in which creates more than 1 three-in-a-row would yield result as n * 1)
    *	Create move = 0.45f (for moves that would create a two-in-a-row - with an empty tile)
    *	Filler move = 0.25f (intended for moves that would just fill the empty tile of the board; actually not needed after careful consideration - but not refactored yet).
- Game animations / transitions.
- Sound effects on UI interactions.

## 3. Noticeable short-comings:
- Some button have hidden function that the player cannot (and would not) notice right away.

## 4. Others:
- Here is the UI designs (I created it myself - and it's a mess! Do notes that this is view-only):

``` 
https://www.figma.com/design/ziJJo5gkpCChhJkaQ2fLFW/TicTacToe?node-id=479-89&t=gYVdEwvZP92pqICs-1
```

## 5. Updates:
- v1.1:
  + Fixed SFX system; remove BGM sound config.
  + Removed 1 sudden death rule, since the removed rule didn't provide interesting gameplay and conflict with existing mechanics.
  + Tweaked UI animations for faster and smoother transitions.
  + Fixed score calculation bug in some special cases (happens during a sudden death rule execution).
  + Tweaked UI sprite for smoother visual.
  + Clean up some of the un-used files.
  + Configured to build on Web (HTML5). The game is now on itch.io!

```
https://datnt621.itch.io/tri-tac-toe
```

Please contact me with this email if you have any questions: tlunguyendat@gmail.com
