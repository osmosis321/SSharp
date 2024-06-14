using System;
using System.Collections.Generic;
using Crestron.SimplSharp;

namespace SimplChessMod
{
    internal partial class ChessBoard
    {
        internal List<ChessMove> legalMoves;
        internal void ComputeLegalMoves(PieceColors turn)
        {
            CrestronConsole.PrintLine("ChessBoard ComputeLegalMoves begin turn {0}", turn);
            try
            {
                // reset the en passant moves
                if (turn < PieceColors.WHITE_COLOR || turn > PieceColors.BLACK_COLOR)
                    throw new Exception("ChessBoard ComputeLegalMoves invalid turn color");

                // count pieces for both colors because king vs king is always a draw
                int numberOfWhitePieces = 0;
                int numberOfBlackPieces = 0;

                // find all possible moves, this may include some illegal moves that have to be removed later
                List<ChessMove> PossibleMoves = new List<ChessMove>();
                threatmap.CreateMap(board);

                for (byte idx = 0; idx < 64; idx++)
                {
                    Pieces piece = board[idx];
                    PieceColors color = PieceColor(piece);

                    switch (color)
                    {
                        case PieceColors.WHITE_COLOR: numberOfWhitePieces++; break;
                        case PieceColors.BLACK_COLOR: numberOfBlackPieces++; break;
                    }

                    if (color != turn)
                        continue;

                    byte y = idxtoy(idx), x = idxtox(idx);
                    int yp1 = y + 1, yp2 = y + 2, ym1 = y - 1, ym2 = y - 2;
                    int xp1 = x + 1, xp2 = x + 2, xm1 = x - 1, xm2 = x - 2;

                    PieceTypes ptype = PieceType(piece);
                    PieceColors ocolor = otherColor(color);

                    if (piece == Pieces.WHITE_PAWN)
                    {
                        if (y < 7)
                        {
                            // 1 square up
                            if (board[yxtoidx(yp1, x)] == Pieces.NO_PIECE)
                            {
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, x), piece));
                                // 2 squares up
                                if (y == 1 && board[yxtoidx(yp2, x)] == Pieces.NO_PIECE)
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp2, x), piece));
                            }
                            // captures
                            if (x > 0 && isBlack(yp1, xm1))
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xm1), piece, board[yxtoidx(yp1, xm1)]));

                            if (x < 7 && isBlack(yp1, xp1))
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xp1), piece, board[yxtoidx(yp1, xp1)]));

                            // en passant
                            if (y == 4)
                            {
                                if (x > 0 && blackPawnMoved[xm1])
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xm1), piece, board[yxtoidx(y, xm1)], true));

                                if (x < 7 && blackPawnMoved[xp1])
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xp1), piece, board[yxtoidx(y, xp1)], true));
                            }
                        }
                    }
                    else if (piece == Pieces.BLACK_PAWN)
                    {
                        if (y > 0)
                        {
                            if (board[yxtoidx(ym1, x)] == Pieces.NO_PIECE)
                            {
                                // 1 square down
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, x), piece));
                                // 2 squares down
                                if (y == 6 && board[yxtoidx(ym2, x)] == Pieces.NO_PIECE)
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym2, x), piece));
                            }
                            // captures
                            if (x > 0 && isWhite(ym1, xm1))
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xm1), piece, board[yxtoidx(ym1, xm1)]));

                            if (x < 7 && isWhite(ym1, xp1))
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xp1), piece, board[yxtoidx(ym1, xp1)]));

                            // en passant
                            if (y == 3)
                            {
                                if (x > 0 && whitePawnMoved[xm1])
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xm1), piece, board[yxtoidx(y, xm1)], true));

                                if (x < 7 && whitePawnMoved[xp1])
                                    PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xp1), piece, board[yxtoidx(y, xp1)], true));
                            }
                        }
                    }
                    else if (ptype == PieceTypes.KNIGHT_TYPE)
                    {
                        // down
                        if (y > 1)
                        {
                            if (x > 0 && PieceColor(board[yxtoidx(ym2, xm1)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym2, xm1), piece, board[yxtoidx(ym2, xm1)]));

                            if (x < 7 && PieceColor(board[yxtoidx(ym2, xp1)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym2, xp1), piece, board[yxtoidx(ym2, xp1)]));
                        }
                        // up
                        if (y < 6)
                        {
                            if (x > 0 && PieceColor(board[yxtoidx(yp2, xm1)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp2, xm1), piece, board[yxtoidx(yp2, xm1)]));

                            if (x < 7 && PieceColor(board[yxtoidx(yp2, xp1)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp2, xp1), piece, board[yxtoidx(yp2, xp1)]));
                        }
                        // left
                        if (x > 1)
                        {
                            if (y > 0 && PieceColor(board[yxtoidx(ym1, xm2)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xm2), piece, board[yxtoidx(ym1, xm2)]));

                            if (y < 7 && PieceColor(board[yxtoidx(yp1, xm2)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xm2), piece, board[yxtoidx(yp1, xm2)]));
                        }
                        // right
                        if (x < 6)
                        {
                            if (y > 0 && PieceColor(board[yxtoidx(ym1, xp2)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(ym1, xp2), piece, board[yxtoidx(ym1, xp2)]));

                            if (y < 7 && PieceColor(board[yxtoidx(yp1, xp2)]) != turn)
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(yp1, xp2), piece, board[yxtoidx(yp1, xp2)]));
                        }
                    }
                    else if (ptype == PieceTypes.KING_TYPE)
                    {
                        // straight moves
                        if (y > 0)
                        {
                            byte i = yxtoidx(ym1, x);
                            Pieces tp = board[i];
                            if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            // diagonal moves
                            if (x > 0)
                            {
                                i = yxtoidx(ym1, xm1);
                                tp = board[i];
                                if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                    PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            }
                            if (x < 7)
                            {
                                i = yxtoidx(ym1, xp1);
                                tp = board[i];
                                if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                    PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            }
                        }
                        if (y < 7)
                        {
                            byte i = yxtoidx(yp1, x);
                            Pieces tp = board[i];
                            if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            // diagonal moves
                            if (x > 0)
                            {
                                i = yxtoidx(yp1, xm1);
                                tp = board[i];
                                if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                    PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            }
                            if (x < 7)
                            {
                                i = yxtoidx(yp1, xp1);
                                tp = board[i];
                                if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                    PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                            }
                        }
                        if (x > 0)
                        {
                            byte i = yxtoidx(y, xm1);
                            Pieces tp = board[i];
                            if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                        }
                        if (x < 7)
                        {
                            byte i = yxtoidx(y, xp1);
                            Pieces tp = board[i];
                            if (PieceColor(tp) != color && !threatmap.isThreat(i, ocolor))
                                PossibleMoves.Add(new ChessMove(idx, i, piece, tp));
                        }

                        // castles
                        if (!ChessThreatMap.isInCheck(board, turn))
                        {
                            int cpi = colorAsIndex(turn);
                            if (!castlePieces[cpi, (int)CastlePieces.KING])
                            {
                                int castley = (turn == PieceColors.WHITE_COLOR) ? 0 : 7;
                                Pieces kingpiece = board[yxtoidx(castley, 4)];

                                // queen side castle
                                if (!castlePieces[cpi, (int)CastlePieces.LEFT_ROOK])
                                {
                                    Pieces rookpiece = board[yxtoidx(castley, 0)];
                                    PieceColors rookcolor = PieceColor(rookpiece);
                                    PieceTypes rooktype = PieceType(rookpiece);
                                    byte a = yxtoidx(castley, 1), b = yxtoidx(castley, 2), c = yxtoidx(castley, 3);
                                    // is the right piece in the square
                                    if (rookcolor == turn && rooktype == PieceTypes.ROOK_TYPE)
                                    {
                                        // are the squares NO_PIECE
                                        if (isNO_PIECE(a) && isNO_PIECE(b) && isNO_PIECE(c))
                                        {
                                            // are none of the squares under attack by black
                                            if (!(threatmap.isThreat(a, ocolor) || threatmap.isThreat(b, ocolor) || threatmap.isThreat(c, ocolor)))
                                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(castley, 2), kingpiece, Pieces.NO_PIECE, false, CastleTypes.LEFT_CASTLE));
                                        }
                                    }
                                }
                                // king side castle
                                if (!castlePieces[cpi, (int)CastlePieces.RIGHT_ROOK])
                                {
                                    Pieces rookpiece = board[yxtoidx(castley, 7)];
                                    PieceColors rookcolor = PieceColor(rookpiece);
                                    PieceTypes rooktype = PieceType(rookpiece);
                                    byte a = yxtoidx(castley, 5), b = yxtoidx(castley, 6);
                                    // is the right piece in the square
                                    if (rookcolor == turn && rooktype == PieceTypes.ROOK_TYPE)
                                    {
                                        // are the squares NO_PIECE
                                        if (isNO_PIECE(a) && isNO_PIECE(b))
                                        {
                                            // are none of the squares under attack by black
                                            if (!(threatmap.isThreat(a, ocolor) || threatmap.isThreat(b, ocolor)))
                                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(castley, 6), kingpiece, Pieces.NO_PIECE, false, CastleTypes.RIGHT_CASTLE));
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        // rook moves
                        if (ptype == PieceTypes.ROOK_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                        {
                            // down
                            for (int r = ym1; r >= 0; r--)
                            {
                                Pieces tp = board[yxtoidx(r, x)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, x), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // up
                            for (int r = yp1; r < 8; r++)
                            {
                                Pieces tp = board[yxtoidx(r, x)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, x), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // left
                            for (int f = xm1; f >= 0; f--)
                            {
                                Pieces tp = board[yxtoidx(y, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(y, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // right
                            for (int f = xp1; f < 8; f++)
                            {
                                Pieces tp = board[yxtoidx(y, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(y, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                        }
                        // bishop moves
                        if (ptype == PieceTypes.BISHOP_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                        {
                            // down left
                            for (int r = ym1, f = xm1; r >= 0 && f >= 0; r--, f--)
                            {
                                Pieces tp = board[yxtoidx(r, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // down right
                            for (int r = ym1, f = xp1; r >= 0 && f < 8; r--, f++)
                            {
                                Pieces tp = board[yxtoidx(r, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // up left
                            for (int r = yp1, f = xm1; r < 8 && f >= 0; r++, f--)
                            {
                                Pieces tp = board[yxtoidx(r, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                            // up right
                            for (int r = yp1, f = xp1; r < 8 && f < 8; r++, f++)
                            {
                                Pieces tp = board[yxtoidx(r, f)];
                                PieceColors tc = PieceColor(tp);
                                // stop at pieces with the same color
                                if (tc == turn) break;
                                PossibleMoves.Add(new ChessMove(idx, yxtoidx(r, f), piece, tp));
                                // stop at enemy pieces
                                if (tp != Pieces.NO_PIECE) break;
                            }
                        }
                    }
                }

                legalMoves.Clear();
                CrestronConsole.PrintLine("ChessBoard ComputeLegalMoves removing illegal moves from {0} possible moves", PossibleMoves.Count);

                // remove all illegal moves
                foreach (ChessMove move in PossibleMoves)
                {
                    // castling moves already cannot happen if the king is in check or if the king would end up in check
                    if (move.isCastle)
                    {
                        legalMoves.Add(move);
                    }
                    else  // do the move temporarily
                    {
                        if (((move.piece == Pieces.WHITE_PAWN) || (move.piece == Pieces.BLACK_PAWN)) && move.enPassant)
                        {
                            board[move.from] = Pieces.NO_PIECE;
                            board[move.to] = move.piece;
                            // remove the black pawn
                            board[yxtoidx(move.fromY, move.toX)] = Pieces.NO_PIECE;
                        }
                        else
                        {
                            board[move.from] = Pieces.NO_PIECE;
                            board[move.to] = move.piece;
                        }

                        if (PieceColor(move.piece) == PieceColors.WHITE_COLOR) //if it's white
                        {
                            if (!ChessThreatMap.isInCheck(board, PieceColors.WHITE_COLOR)) // check whether the white king is in check
                                legalMoves.Add(move);

                        }
                        else if (PieceColor(move.piece) == PieceColors.BLACK_COLOR)// Else if it's black
                            if (!ChessThreatMap.isInCheck(board, PieceColors.BLACK_COLOR)) // check whether the black king is in check
                                legalMoves.Add(move);

                        // undo the move
                        if (((move.piece == Pieces.WHITE_PAWN) || (move.piece == Pieces.BLACK_PAWN)) && move.enPassant)
                        {
                            board[move.from] = move.piece;
                            board[move.to] = move.capture;
                            // restore the black pawn
                            board[yxtoidx(move.fromY, move.toX)] = move.piece;
                        }
                        else
                        {
                            board[move.from] = move.piece;
                            board[move.to] = move.capture;
                        }
                    }
                }

                // check status
                if (ChessThreatMap.isInCheck(board, PieceColors.WHITE_COLOR))
                    checkStat = CheckStats.WHITE_CHECK;
                else if (ChessThreatMap.isInCheck(board, PieceColors.BLACK_COLOR))
                    checkStat = CheckStats.BLACK_CHECK;
                else
                    checkStat = CheckStats.NO_CHECK;

                CrestronConsole.PrintLine("ChessBoard ComputeLegalMoves {0} legal moves found", legalMoves.Count);

                // check mate detection
                if (legalMoves.Count == 0)
                {
                    if (turn == PieceColors.WHITE_COLOR)
                    {
                        if (ChessThreatMap.isInCheck(board, PieceColors.WHITE_COLOR))
                            gameState = GameStates.BLACKWIN;
                        else
                            gameState = GameStates.DRAW;
                    }
                    else if (turn == PieceColors.BLACK_COLOR)
                    {
                        {
                            if (ChessThreatMap.isInCheck(board, PieceColors.BLACK_COLOR))
                                gameState = GameStates.WHITEWIN;
                            else
                                gameState = GameStates.DRAW;
                        }
                    }
                }
                else if (numberOfWhitePieces == 1 && numberOfBlackPieces == 1)
                    gameState = GameStates.DRAW;
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("ChessBoard ComputeLegalMoves caught exception {0}", e.Message);
            }

            CrestronConsole.PrintLine("ChessBoard ComputeLegalMoves checking game status == {0}", gameState);
        }
    }
}
