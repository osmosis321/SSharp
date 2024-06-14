using System;

namespace SimplChessMod
{
    public class ChessThreatMap
    {
        private enum Threats : byte
        {
            NO_THREAT = 0,
            WHITE_THREAT = 1,
            BLACK_THREAT = 2,
            BOTH_THREAT = 4
        }

        private Threats[] map;

        internal ChessThreatMap()
        {
            map = new Threats[64];
        }

        private void Clear()
        {
            for (int i = 0; i < 64; i++)
                map[i] = Threats.NO_THREAT;
        }

        internal static bool isInCheck(Pieces[] board, PieceColors color)
        {
            if (color < PieceColors.WHITE_COLOR || color > PieceColors.BLACK_COLOR)
                throw new ArgumentException("isInCheck parameter out of bounds", "color");

            Pieces kingPiece = (color == PieceColors.WHITE_COLOR) ? Pieces.WHITE_KING : Pieces.BLACK_KING;
            PieceColors threatColor = ChessBoard.otherColor(color);

            for (int idx = 0; idx < 64; idx++)
            {
                Pieces piece = board[idx];
                if (piece == Pieces.NO_PIECE) continue;

                PieceColors pcolor = ChessBoard.PieceColor(piece);
                if (pcolor == color) continue;

                PieceTypes ptype = ChessBoard.PieceType(piece);

                byte y = ChessBoard.idxtoy(idx), x = ChessBoard.idxtox(idx);
                int yp1 = y + 1, yp2 = y + 2, ym1 = y - 1, ym2 = y - 2;
                int xp1 = x + 1, xp2 = x + 2, xm1 = x - 1, xm2 = x - 2;

                if (ptype == PieceTypes.PAWN_TYPE)
                {
                    if (y > 0 && pcolor == PieceColors.BLACK_COLOR)
                    {
                        if (x > 0 && board[ChessBoard.yxtoidx(ym1, xm1)] == kingPiece) goto ischeck;
                        if (x < 7 && board[ChessBoard.yxtoidx(ym1, xp1)] == kingPiece) goto ischeck;

                    }
                    else if (y < 7 && pcolor == PieceColors.WHITE_COLOR)
                    {
                        if (x > 0 && board[ChessBoard.yxtoidx(yp1, xm1)] == kingPiece) goto ischeck;
                        if (x < 7 && board[ChessBoard.yxtoidx(yp1, xp1)] == kingPiece) goto ischeck;
                    }
                }
                else if (ptype == PieceTypes.KNIGHT_TYPE)
                {
                    // up
                    if (y < 6)
                    {
                        if (x > 0 && board[ChessBoard.yxtoidx(yp2, xm1)] == kingPiece) goto ischeck;
                        if (x < 7 && board[ChessBoard.yxtoidx(yp2, xp1)] == kingPiece) goto ischeck;
                    }
                    // down
                    if (y > 1)
                    {
                        if (x > 0 && board[ChessBoard.yxtoidx(ym2, xm1)] == kingPiece) goto ischeck;
                        if (x < 7 && board[ChessBoard.yxtoidx(ym2, xp1)] == kingPiece) goto ischeck;
                    }
                    // left
                    if (x > 1)
                    {
                        if (y > 0 && board[ChessBoard.yxtoidx(ym1, xm2)] == kingPiece) goto ischeck;
                        if (y < 7 && board[ChessBoard.yxtoidx(yp1, xm2)] == kingPiece) goto ischeck;
                    }
                    // right
                    if (x < 6)
                    {
                        if (y > 0 && board[ChessBoard.yxtoidx(ym1, xp2)] == kingPiece) goto ischeck;
                        if (y < 7 && board[ChessBoard.yxtoidx(yp1, xp2)] == kingPiece) goto ischeck;
                    }
                }
                else
                {
                    Pieces tpiece;
                    // rook moves
                    if (ptype == PieceTypes.ROOK_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                    {
                        // up
                        for (int r = yp1; r < 8; r++)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, x)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // down
                        for (int r = ym1; r >= 0; r--)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, x)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // left
                        for (int f = xm1; f >= 0; f--)
                        {
                            tpiece = board[ChessBoard.yxtoidx(y, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // right
                        for (int f = xp1; f < 8; f++)
                        {
                            tpiece = board[ChessBoard.yxtoidx(y, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }
                    }

                    // bishop moves
                    if (ptype == PieceTypes.BISHOP_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                    {
                        // down left
                        for (int r = ym1, f = xm1; r >= 0 && f >= 0; r--, f--)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // down right
                        for (int r = ym1, f = xp1; r >= 0 && f < 8; r--, f++)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // up left
                        for (int r = yp1, f = xm1; r < 8 && f >= 0; r++, f--)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }

                        // up right
                        for (int r = yp1, f = xp1; r < 8 && f < 8; r++, f++)
                        {
                            tpiece = board[ChessBoard.yxtoidx(r, f)];
                            if (tpiece == kingPiece) goto ischeck;
                            // stop at any piece
                            if (tpiece != Pieces.NO_PIECE) break;
                        }
                    }
                }
            }
            return false;
        ischeck:
            return true;
        }

        internal bool isThreat(int idx, PieceColors color)
        {
            if (color < PieceColors.WHITE_COLOR || color > PieceColors.BLACK_COLOR)
                throw new ArgumentException("isThreat parameter out of bounds", "color");
            if (idx < 0 || idx > 63)
                throw new ArgumentException("isThreat parameter out of bounds", "idx");

            return (map[idx] & threatMask(color)) != Threats.NO_THREAT;
        }
        private Threats threatMask(PieceColors color)
        {
            if (color < PieceColors.WHITE_COLOR || color > PieceColors.BLACK_COLOR)
                throw new ArgumentException("threatMask parameter out of bounds", "color");

            return (color == PieceColors.WHITE_COLOR) ? Threats.WHITE_THREAT : Threats.BLACK_THREAT;
        }

        internal void CreateMap(Pieces[] board)
        {
            Clear();
            for (byte idx = 0; idx < 64; idx++)
            {
                Pieces piece = board[idx];
                if (piece == Pieces.NO_PIECE)
                    continue;

                PieceColors color = ChessBoard.PieceColor(piece);

                byte y = ChessBoard.idxtoy(idx), x = ChessBoard.idxtox(idx);
                int yp1 = y + 1, yp2 = y + 2, ym1 = y - 1, ym2 = y - 2;
                int xp1 = x + 1, xp2 = x + 2, xm1 = x - 1, xm2 = x - 2;

                if (piece == Pieces.WHITE_PAWN)
                {
                    if (y < 7)
                    {
                        if (x > 0) map[ChessBoard.yxtoidx(yp1, xm1)] |= Threats.WHITE_THREAT;
                        if (x < 7) map[ChessBoard.yxtoidx(yp1, xp1)] |= Threats.WHITE_THREAT;
                    }
                }
                else if (piece == Pieces.BLACK_PAWN)
                {
                    if (y > 0)
                    {
                        if (x > 0) map[ChessBoard.yxtoidx(ym1, xm1)] |= Threats.BLACK_THREAT;
                        if (x < 7) map[ChessBoard.yxtoidx(ym1, xp1)] |= Threats.BLACK_THREAT;
                    }
                }
                else
                {
                    PieceTypes ptype = ChessBoard.PieceType(piece);
                    Threats typethreat = threatMask(color);
                    if (ptype == PieceTypes.KING_TYPE)
                    {
                        if (y < 7) // up
                        {
                            map[ChessBoard.yxtoidx(yp1, x)] |= typethreat;
                            // up left and right
                            if (x > 0) map[ChessBoard.yxtoidx(yp1, xm1)] |= typethreat;
                            if (x < 7) map[ChessBoard.yxtoidx(yp1, xp1)] |= typethreat;
                        }
                        if (y > 0) // down
                        {
                            map[ChessBoard.yxtoidx(ym1, x)] |= typethreat;
                            // down left and right
                            if (x > 0) map[ChessBoard.yxtoidx(ym1, xm1)] |= typethreat;
                            if (x < 7) map[ChessBoard.yxtoidx(ym1, xp1)] |= typethreat;
                        }
                        // left and right
                        if (x > 0) map[ChessBoard.yxtoidx(y, xm1)] |= typethreat;
                        if (x < 7) map[ChessBoard.yxtoidx(y, xp1)] |= typethreat;
                    }
                    else if (ptype == PieceTypes.KNIGHT_TYPE)
                    {

                        if (y < 6) // up
                        {
                            if (x > 0) map[ChessBoard.yxtoidx(yp2, xm1)] |= typethreat;
                            if (x < 7) map[ChessBoard.yxtoidx(yp2, xp1)] |= typethreat;
                        }

                        if (y > 1) // down
                        {
                            if (x > 0) map[ChessBoard.yxtoidx(ym2, xm1)] |= typethreat;
                            if (x < 7) map[ChessBoard.yxtoidx(ym2, xp1)] |= typethreat;
                        }

                        if (x < 6) // right
                        {
                            if (y > 0) map[ChessBoard.yxtoidx(ym1, xp2)] |= typethreat;
                            if (y < 7) map[ChessBoard.yxtoidx(yp1, xp2)] |= typethreat;
                        }

                        if (x > 1) // left
                        {
                            if (y > 0) map[ChessBoard.yxtoidx(ym1, xm2)] |= typethreat;
                            if (y < 7) map[ChessBoard.yxtoidx(yp1, xm2)] |= typethreat;

                        }
                    }
                    else
                    {
                        // rook moves
                        if (ptype == PieceTypes.ROOK_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                        {
                            for (int r = yp1; r < 8; r++) // up
                            {
                                map[ChessBoard.yxtoidx(r, x)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, x)] != Pieces.NO_PIECE) break;
                            }
                            for (int r = ym1; r >= 0; r--) // down
                            {
                                map[ChessBoard.yxtoidx(r, x)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, x)] != Pieces.NO_PIECE) break;
                            }
                            for (int f = xp1; f < 8; f++) // right
                            {
                                map[ChessBoard.yxtoidx(y, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(y, f)] != Pieces.NO_PIECE) break;
                            }
                            for (int f = xm1; f >= 0; f--) // left
                            {
                                map[ChessBoard.yxtoidx(y, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(y, f)] != Pieces.NO_PIECE) break;
                            }
                        }
                        // bishop moves
                        if (ptype == PieceTypes.BISHOP_TYPE || ptype == PieceTypes.QUEEN_TYPE)
                        {
                            for (int r = ym1, f = xm1; r >= 0 && f >= 0; r--, f--) // down left
                            {
                                map[ChessBoard.yxtoidx(r, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, f)] != Pieces.NO_PIECE) break;
                            }
                            for (int r = ym1, f = xp1; r >= 0 && f < 8; r--, f++) // down right
                            {
                                map[ChessBoard.yxtoidx(r, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, f)] != Pieces.NO_PIECE) break;
                            }
                            for (int r = yp1, f = xm1; r < 8 && f >= 0; r++, f--) // up left
                            {
                                map[ChessBoard.yxtoidx(r, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, f)] != Pieces.NO_PIECE) break;
                            }
                            for (int r = yp1, f = xp1; r < 8 && f < 8; r++, f++) // up right
                            {
                                map[ChessBoard.yxtoidx(r, f)] |= typethreat;
                                // stop at any piece
                                if (board[ChessBoard.yxtoidx(r, f)] != Pieces.NO_PIECE) break;
                            }
                        }
                    }
                }
            }
        }
    }
}
