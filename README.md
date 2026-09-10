# \# Lettra – Main Gameplay Mechanics

# 

# \## Word Creation

# 

# The core gameplay of \*\*Lettra\*\* revolves around creating valid words from the letters available on the board.

# 

# Players select letters from the grid to build a word, then press \*\*Check\*\* to submit it.

# 

# \* Each letter can only be used once per word.

# \* Selected letters are temporarily removed from the grid while building the word.

# \* If the word is invalid, the selected letters are returned to their original positions.

# \* If the word is valid, the used letters are replaced with new random letters.

# \* Longer words award significantly more points.

# \* Each level has a limited number of normal word attempts.

# 

# The letter generator uses weighted letter frequencies, making common letters such as \*\*E, A, T, O, and I\*\* appear more frequently.

# 

# \---

# 

# \## Bonus Letters

# 

# \*\*Bonus Letters\*\* are special letters on the board that provide additional rewards when included in a valid word.

# 

# Bonus Letters are randomly generated and visually marked on the letter grid. When a Bonus Letter is used in a valid word, its effect is applied to the score or Bonus Gauge.

# 

# There are three types:

# 

# \### +50 Points

# 

# Adds \*\*50 flat points\*\* to the word's score.

# 

# \### +30% Points

# 

# Increases the final word score by \*\*30%\*\*.

# 

# Multiple +30% bonuses stack additively. For example, two +30% bonuses provide a total \*\*+60%\*\* score bonus.

# 

# \### +Gauge

# 

# Increases the amount of Bonus Gauge gained from the word.

# 

# Multiple +Gauge bonuses also stack. Each additional +Gauge bonus increases the gauge gain by another \*\*50% of the normal amount\*\*.

# 

# Bonus effects are calculated when the word is successfully validated.

# 

# Bonus Letters can be earned by meeting certain gameplay conditions, such as:

# 

# \* Creating a sufficiently long word.

# \* Scoring a high-value word.

# \* Creating multiple valid words consecutively.

# 

# Three Bonus Letters are also granted at the beginning of each level.

# 

# \---

# 

# \## Bonus Phase

# 

# The \*\*Bonus Phase\*\* is a temporary special gameplay state activated when the Bonus Gauge reaches 100%.

# 

# The Bonus Phase lasts for \*\*10 seconds\*\*.

# 

# During this phase:

# 

# \* The Bonus Gauge remains full.

# \* Normal gauge generation is temporarily disabled.

# \* The player can select \*\*one letter\*\* from the grid.

# \* The game automatically finds the \*\*longest possible valid word\*\* that can be created using that letter and the available board letters.

# \* The selected letter does not need to be the first letter of the generated word.

# \* The word is automatically filled into the answer grid.

# \* After a \*\*0.2-second delay\*\*, the word is automatically checked.

# \* The word does not consume a normal word attempt.

# 

# After the automatic word is validated, the used letters are replaced with new letters and the player can trigger another Bonus Phase word by selecting another letter.

# 

# The Bonus Phase therefore rewards the player for building the Bonus Gauge and temporarily changes the normal word-selection gameplay into a faster, automated word-combination mechanic.

# 

# \---

# 

# \## Stage Completion Sequence

# 

# Completing all of a level's required objectives does not immediately end the level.

# 

# Instead, \*\*Lettra\*\* enters a special \*\*Stage Completion Sequence\*\*.

# 

# Once the sequence begins:

# 

# \* Player interaction is disabled.

# \* Normal Bonus Letter generation is stopped.

# \* Any active Bonus Phase is ended.

# \* The player's remaining normal word attempts are converted into the same number of random Bonus Letters.

# \* The remaining word counter is set to zero.

# 

# The game then automatically processes the Bonus Letters one at a time.

# 

# For each Bonus Letter:

# 

# 1\. A Bonus Letter is randomly selected from the board.

# 2\. The game finds the longest valid word that can be made using that letter.

# 3\. The word is automatically filled into the answer grid.

# 4\. The game waits \*\*0.2 seconds\*\*.

# 5\. The word is automatically checked.

# 6\. The game waits another \*\*0.2 seconds\*\*.

# 7\. The next Bonus Letter is selected.

# 

# If none of the current Bonus Letters can be used to create a valid word, the game automatically \*\*re-shuffles the letter grid\*\* and continues the sequence.

# 

# The sequence continues until there are no Bonus Letters remaining on the board.

# 

# Only after all Bonus Letters have been processed does the \*\*Stage Complete\*\* screen appear.

# 

# This creates a final reward sequence where unused word attempts are converted into additional opportunities to score points and use Bonus Letter effects.



