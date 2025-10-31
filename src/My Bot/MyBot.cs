using ChessChallenge.API;
using System;

/*

CHUSS Chess BOT

 1018 / 1024 tokens wow perfect limit, that was not intentional at all
I just reailsd  comments don't take up tokens lolol

Anyways, ....

          _____                    _____                    _____                    _____                    _____          
         /\    \                  /\    \                  /\    \                  /\    \                  /\    \         
        /::\    \                /::\____\                /::\____\                /::\    \                /::\    \        
       /::::\    \              /:::/    /               /:::/    /               /::::\    \              /::::\    \       
      /::::::\    \            /:::/    /               /:::/    /               /::::::\    \            /::::::\    \      
     /:::/\:::\    \          /:::/    /               /:::/    /               /:::/\:::\    \          /:::/\:::\    \     
    /:::/  \:::\    \        /:::/____/               /:::/    /               /:::/__\:::\    \        /:::/__\:::\    \    
   /:::/    \:::\    \      /::::\    \              /:::/    /                \:::\   \:::\    \       \:::\   \:::\    \   
  /:::/    / \:::\    \    /::::::\    \   _____    /:::/    /      _____    ___\:::\   \:::\    \    ___\:::\   \:::\    \  
 /:::/    /   \:::\    \  /:::/\:::\    \ /\    \  /:::/____/      /\    \  /\   \:::\   \:::\    \  /\   \:::\   \:::\    \ 
/:::/____/     \:::\____\/:::/  \:::\    /::\____\|:::|    /      /::\____\/::\   \:::\   \:::\____\/::\   \:::\   \:::\____\
\:::\    \      \::/    /\::/    \:::\  /:::/    /|:::|____\     /:::/    /\:::\   \:::\   \::/    /\:::\   \:::\   \::/    /
 \:::\    \      \/____/  \/____/ \:::\/:::/    /  \:::\    \   /:::/    /  \:::\   \:::\   \/____/  \:::\   \:::\   \/____/ 
  \:::\    \                       \::::::/    /    \:::\    \ /:::/    /    \:::\   \:::\    \       \:::\   \:::\    \     
   \:::\    \                       \::::/    /      \:::\    /:::/    /      \:::\   \:::\____\       \:::\   \:::\____\    
    \:::\    \                      /:::/    /        \:::\__/:::/    /        \:::\  /:::/    /        \:::\  /:::/    /    
     \:::\    \                    /:::/    /          \::::::::/    /          \:::\/:::/    /          \:::\/:::/    /     
      \:::\    \                  /:::/    /            \::::::/    /            \::::::/    /            \::::::/    /      
       \:::\____\                /:::/    /              \::::/    /              \::::/    /              \::::/    /       
        \::/    /                \::/    /                \::/____/                \::/    /                \::/    /        
         \/____/                  \/____/                  ~~                       \/____/                  \/____/         
                                                                                                                                                                                                                                                       
*/

/* This bot is built to absolutely dominate the Chess Segment of SDSTEMCON 2025
It uses a negamax algorithm at its heart, with quiescence search (bujhte ato shomoy lagse eita) and move ordering
using the LVV-MVA heuristic (pawn rani khaile good)

I also implemented alpha beta pruning and aspiration windows, which makes it a little bit faster and smarter

The evaluation function is just a VERY SQUISHED (i mean very very squished) PST table
also it calculates mobility by calling bitboard.GetPieceAttacks() this is an inbuilt very helpful
function by Sebastian lague and it makes life so much better as a dev

I wanted to do transposition tables but im out of tokens :C

Also, I optimized token by mainly two ways
One is comma chaining, when you do int something = a; int b = something;

instead of that which takes up like 10 tokens just do int something = a, b = something;
^
THIS achieves the same result but exploits a small hack in the token counting function 
I felt quite ecstatic when i discovered this

Also: Instead of IF statements i tried to use ternary operators as much as possible 

This is particularly noted in the PSTBairKor function when multiple ternaries are chained.

Some variable names may seem nonsensical because i have chipped them off to save valuable tokens
I quoted some of the resources I used modified to my need, (its legal to use resources according to competition rules)

Anyways ENJOY and I hope I WIN :)

*/

public class MyBot :  IChessBot
{
    Board board;
    ulong[] saucyWeights = {
        0x8C9A9C9A9C9A9C9A, 0x7B8A7B8A7B8A7B8A,0x6A796A796A796A79,0x5968596859685968,
        0x4857485748574857,0x3746374637463746,0x2635263526352635,0x1524152415241524,
        0x8B9B8B9B8B9B8B9B,0x7A8B7A8B7A8B7A8B ,0x697A697A697A697A,0x5869586958695869,
         0x4758475847584758,0x3647364736473647, 0x2536253625362536,0x1425142514251425,
        0x9CAC9CAC9CAC9CAC , 0x8B9C8B9C8B9C8B9C ,0x7A8B7A8B7A8B7A8B,0x697A697A697A697A,
        0x5869586958695869,0x4758475847584758,0x3647364736473647,0x2536253625362536,
        0xACBCACBCACBCACBC,0x9BAC9BAC9BAC9BAC,0x8A9B8A9B8A9B8A9B ,0x798A798A798A798A,
        0x6879687968796879,0x5768576857685768,0x4657465746574657, 0x3546354635463546,
        0xBCCDBCCDBCCDBCCD, 0xBCCDBCCDBCCDBCCD
    };

    int[] pieceValues = {100, 320,330, 500,900, 10000 },
    mobilityBonus = {0, 5, 5,3, 2, 0},
    phaseWeights = {0, 1, 1,
    2,4, 0};

    int gamePhase;

    int Evaluate() // ashol jinish, the MOST IMPORTANT THING OF THIS ENGINE
    {
        if (board.IsInCheckmate()) // if khela shesh taile to ar kisu korar nai, return some big value
            return -10000000 ;

        if (board.IsDraw())
            return 0;

        int midgameScore = 0,
        endgameScore = 0;
        gamePhase= 0;

        foreach (bool isWhite in new[] { true, false })
        {
            int sign = isWhite ? 1 : -1; // sign encoding helps us save tokens later on

            for (int pieceType = 1; pieceType <= 6; pieceType++)
            {
                ulong bitboard =board.GetPieceBitboard((PieceType)pieceType, isWhite);

                while (bitboard!= 0) // bitboard = 0 na hoile mane 1 ase somewhere
                {
                    int square = BitboardHelper.ClearAndGetIndexOfLSB(ref bitboard);
                    int evalSquare = isWhite ? square : square ^ 56; // ternary to save tokens, black hoile ^56 karon it will mirror

                    int mgWeight = PSTBairKor(pieceType,evalSquare, false); // funny name
                    int egWeight = PSTBairKor(pieceType, evalSquare, true);

                    midgameScore += sign * (pieceValues[pieceType - 1] + mgWeight); // that sign encoding is kinda helping man
                    endgameScore += sign * (pieceValues[pieceType - 1]+ egWeight);

                    if (pieceType > 1 && pieceType< 6)
                    {
                        var attacks = BitboardHelper.GetPieceAttacks((PieceType)pieceType,
                            new Square(square), board, isWhite);
                        int mobility = BitboardHelper.GetNumberOfSetBits(attacks); // more place to move the better man
                        midgameScore += sign * mobility * mobilityBonus[pieceType - 1];
                        endgameScore += sign * mobility* mobilityBonus[pieceType - 1];
                    }

                    gamePhase+= phaseWeights[pieceType -1]; // so depending on the piece, gamephase is changed see line 82
                }
            }
        }

        gamePhase = Math.Min(gamePhase, 256); // limit korlam naile beshi barle jhamela index error dey for some reason
        int taperedScore = ((midgameScore * (256- gamePhase) + endgameScore * gamePhase) / 256) + 10; // + 10 centi pawns for the side that is moving

        return board.IsWhiteToMove ? taperedScore : -taperedScore;
    }

    int PSTBairKor(int pieceType, int square, bool endgame) // this thing is responsible for decoding that ugly blob of hexadecimal things
    {
        int index = (pieceType -(endgame && pieceType == 6 ? 0 : 1)) * 4 +
                    (square < 32 ? square : (7 - square / 8) * 8 + square % 8) / 16; // end game king is piece type 6, also i mirror the table to save some space

        ulong packed = saucyWeights[index];
        int weight = (int)((packed >>(square % 16 * 4))& 0xF); // rightshifting and then bitmasking will open the weight

        return (weight - 8) * 4;
    }

    int Quiescence(int alpha, int beta) // fancy jinish, eita dekho https://www.chessprogramming.org/Quiescence_Search#Pseudo_Code
    {
        // essential for removing horizon effect
        int standPat= Evaluate(); // stand pat get its name, becaause you just stand and see what you have without doing anything at all

        if (standPat >= beta)
            return beta;

        if (standPat > alpha)
            alpha =standPat;

        Move[] captures = board.GetLegalMoves(true);
        OrderMoves(captures);

        foreach (Move capture in captures)
        {
            int deltaMargin= gamePhase > 128 ?100 : 200, // i added a little something called delta pruingin https://www.chessprogramming.org/Delta_Pruning#Sample_Code
            captureValue = pieceValues[(int)capture.CapturePieceType - 1];
            if (standPat + captureValue + deltaMargin < alpha || (captureValue < pieceValues[(int)capture.MovePieceType - 1] && standPat < alpha - 150))
                continue;

            board.MakeMove(capture) ;
            int score = -Quiescence(-beta,-alpha);
            board.UndoMove(capture);
            // board.GameRepetitionHistory()

            if (score>standPat)
            {
                standPat=score;
                if (standPat> alpha)
                    alpha = standPat;
                // beta = score;
                if (standPat>=beta)
                    break;
            }
        }

        return standPat;
    }

    void OrderMoves(Move[] moves) => Array.Sort(moves, (a, b) =>
    {
        // very squished down, but inline function that gives higher preference when capturing high value piece with low value piece, MVV-LVA
        int Score(Move m) => m.IsCapture ? pieceValues[(int)(m.CapturePieceType - 1)] * 10 - pieceValues[(int)(m.MovePieceType - 1)] : 0;
        if (a.IsCapture != b.IsCapture) return b.IsCapture.CompareTo(a.IsCapture);
        if (a.IsCapture) return Score(b).CompareTo(Score(a));
        // if (a.IsCapture) return Score(a).CompareTo(Score(b));
        return 0 ;
    });

    int Negamax(int depth, int alpha, int beta)
    {
        if (board.IsDraw() || board.IsInCheckmate())
            return Evaluate(); // agei call kortese naile hudai shomoy nosto korbe
            
        if (depth <= 0)
            return Quiescence(alpha, beta);

        if (board.IsInCheck())
            depth++;

        int bestScore =-10000000 ;
        Move bestMove;

        Move[] moves = board.GetLegalMoves();
        OrderMoves(moves);

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -Negamax(depth - 1, -beta, -alpha);
            board.UndoMove(move);

            if (score >  bestScore)
            {
                bestScore = score;
                bestMove = move;
            }

            alpha = Math.Max(alpha, score);
            if (alpha>= beta)
                break;
        }

        return bestScore ;
    }

    public Move Think(Board b , Timer timer) // i suck at code but pro coder and ektu c# janle i think this could have been further compressed
    { // not my proudest code
        board = b;
        Move bestMove =default;
        int bestScore = 0;

        int allocatedTime = timer.MillisecondsRemaining / 40; // set a hard limit
        int maxDepth = 24; // i wanted to do 67 for atef but beshi boro hoye jay for 1 minute bullet games

        for (int searchDepth=1; searchDepth <= maxDepth;searchDepth++)
        {
            if (timer.MillisecondsElapsedThisTurn > allocatedTime)
                break;

            Move currentBestMove = default;
            bool searchCompleted = true, // again comma chaining here
            researchNeeded = false;

            int onekBhaloScore =-10000000,
            windowSize = 40 + searchDepth * 2,
            alpha = bestScore - windowSize, // aspiration window implementation
            beta = bestScore + windowSize;

            Move[] moves = board.GetLegalMoves();
            OrderMoves(moves);

            foreach (Move move in moves)
            {
                if (timer.MillisecondsElapsedThisTurn > allocatedTime)
                {
                    searchCompleted =false;
                    break;
                }

                board.MakeMove(move);
                int score = -Negamax(searchDepth - 1, -beta, -alpha); // start the negamax man
                board.UndoMove(move);

                if (score > onekBhaloScore)
                {
                    onekBhaloScore = score;
                    currentBestMove = move;
                }

                if (score <= alpha|| score >= beta)
                {
                    researchNeeded= true;
                    break;
                }
            }

            if (researchNeeded&& searchCompleted)
            {
                onekBhaloScore =-10000000;
                foreach (Move move in moves)
                {
                    if (timer.MillisecondsElapsedThisTurn > allocatedTime)
                    {
                        searchCompleted = false;
                        break;
                    }

                    board.MakeMove(move);
                    int score = -Negamax(searchDepth - 1, -10000000, 10000000);
                    board.UndoMove(move);

                    if (score > onekBhaloScore)
                    {
                        onekBhaloScore = score;
                        currentBestMove =move;
                    }
                }
            }

            if (searchCompleted)
            {
                bestMove =currentBestMove;
                bestScore = onekBhaloScore;

                if (Math.Abs(bestScore) > 50000) // abs mane absolute modulus function
                    break;
            }
            else
            {
                break;
            }
        }

        return bestMove == default ? board.GetLegalMoves()[0] : bestMove; // this line kono karone fail korle we are cooked but it should never fail
    }
}

/*
https://www.youtube.com/watch?v=dQw4w9WgXcQ&list=RDdQw4w9WgXcQ&start_radio=1
*/

