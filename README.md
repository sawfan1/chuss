# Chuss
A chess bot written in c# to dominate SD STEMCON Chess Bot Segment 2025 Go to src/My Bot/MyBot.cs
Update: The bot achieved 1st POSITION at SDSTEMCON 2025 on 11 November, 2025. I tested it against some of the upper tier bots, like Stockfish 10.0, Lc0 and the rating is about 3300. (but weaker in endgames)

# Author's comments
Overall, the bot is very time efficient, specially for bullet games. It's also extremely aggressive sometimes overly so but this is okay.

# Further enhancements
Going past 1024 tokens, my suggestions to anyone willing to continue on this base, would be at first to implement transposition tables with depth updating.
Things like null move pruning, and some other creative ways that I don't know of yet may also be possible. Also, one can implement a zobrist key based opening book to save time at the start.
