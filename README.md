# Chuss
A chess bot written in c# to dominate SD STEMCON Chess Bot Segment 2025 Go to src/My Bot/MyBot.cs
Update: The bot achieved 1st POSITION at SDSTEMCON 2025 on 11 November, 2025. I tested it against some of the upper tier bots, like Stockfish 10.0, Lc0 and the rating is about 3300. (but weaker in endgames)

# Author's comments
Overall, the bot is very time efficient, specially for bullet games. It's also extremely aggressive sometimes overly so but this is okay.

# Further enhancements
Going past 1024 tokens, my suggestions to anyone willing to continue on this base, would be at first to implement transposition tables with depth updating.
Things like null move pruning, and some other creative ways that I don't know of yet may also be possible. Also, one can implement a zobrist key based opening book to save time at the start.

Personally, I would like to implement an NNUE (efficiently updatable neural network) much like alpha zero in the evaluation function of this engine instead of using those ugly hexademical pst packing. Theoratically, a NNUE paired with a very strong search function can realistically compete with some of the finest engines like the latest version of Stockfist at the moment of writing (stockfist 17). I hope to continue this project some other time, perhaps after I have completed my Harvard CS50 AI course. Uptil then, peace.
