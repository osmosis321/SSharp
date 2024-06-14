using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Crestron.SimplSharp;

namespace SimplChessMod
{
	internal partial class ChessBoard
	{
		internal Pieces[] board;
		private ChessThreatMap threatmap;

		internal GameStates gameState;
		internal PieceColors currentTurn;
		internal CheckStats checkStat;

		internal byte[] kingPos;

		internal Pieces[,] captures;

		private bool[,] castlePieces;

		private bool[] whitePawnMoved;
		private bool[] blackPawnMoved;

		internal ChessBoard()
		{
			CrestronConsole.PrintLine("ChessBoard basic constructor");
			board = new Pieces[ 64 ];
			captures = new Pieces[ 2, 8 ];
			whitePawnMoved = new bool[ 8 ];
			blackPawnMoved = new bool[ 8 ];
			castlePieces = new bool[ 2, 3 ];
			kingPos = new byte[2];

			// initialize the legal moves
			legalMoves = new List<ChessMove>();

			// initialize the threat map
			threatmap = new ChessThreatMap();
		}

		internal void Reset()
		{
			CrestronConsole.PrintLine("ChessBoard Reset begin");

			try
			{

				Array.Copy(DefaultPieces, board, 64);

				// reset the en passant moves
				for( int i = 0; i < 8; i++ )
				{
					whitePawnMoved[ i ] = false;
					blackPawnMoved[ i ] = false;
				}

				// Reset the captures
				for( int i = 0; i < 2; i++ )
					for( int j = 0; j < 8; j++ )
						captures[ i, j ] = Pieces.NO_PIECE;

				// Reset the castling flags
				for( int i = 0; i < 2; i++ )
					for( int j = 0; j < 3; j++ )
						castlePieces[ i, j ] = false;

				kingPos[0] = 4;
				kingPos[1] = 60;

				gameState = GameStates.ONGOING;
				currentTurn = PieceColors.WHITE_COLOR;
				checkStat = CheckStats.NO_CHECK;

				ComputeLegalMoves(currentTurn);
			}
			catch( Exception e )
			{
				CrestronConsole.PrintLine("ChessBoard Reset generated exception {0}", e.Message);
			}

			CrestronConsole.PrintLine("ChessBoard Reset finish");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static PieceColors PieceColor(Pieces piece)
		{
			if( piece < Pieces.NO_PIECE || piece > Pieces.BLACK_KING )
				throw new ArgumentException("PieceColor parameter out of bounds", "piece");

			return ( piece == Pieces.NO_PIECE ) ? PieceColors.NO_COLOR : ( ( piece <= Pieces.WHITE_KING ) ? PieceColors.WHITE_COLOR : PieceColors.BLACK_COLOR );
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool isWhite(int y, int x)
		{
			if( y < 0 || y > 7 )
				throw new ArgumentException("isWhite parameter out of bounds", "y");
			if( x < 0 || x > 7 )
				throw new ArgumentException("isWhite parameter out of bounds", "x");

			return PieceColor(board[ yxtoidx(y, x) ]) == PieceColors.WHITE_COLOR;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool isWhite(int idx)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("isWhite parameter out of bounds", "idx");

			return PieceColor(board[ idx ]) == PieceColors.WHITE_COLOR;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool isBlack(int y, int x)
		{
			if( y < 0 || y > 7 )
				throw new ArgumentException("isBlack parameter out of bounds", "y");
			if( x < 0 || x > 7 )
				throw new ArgumentException("isBlack parameter out of bounds", "x");

			return PieceColor(board[ yxtoidx(y, x) ]) == PieceColors.BLACK_COLOR;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool isBlack(int idx)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("isBlack parameter out of bounds", "idx");

			return PieceColor(board[ idx ]) == PieceColors.BLACK_COLOR;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool isNO_PIECE(int idx)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("isNO_PIECE parameter out of bounds", "idx");

			return board[ idx ] == Pieces.NO_PIECE;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte idxtoy(int idx)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("idxtoy parameter out of bounds", "idx");

			return (byte)( idx / ChessBoard.boardwid );
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte idxtox(int idx)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("idxtox parameter out of bounds", "idx");

			return (byte)( idx % ChessBoard.boardwid );
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte yxtoidx(int y, int x)
		{
			if( y < 0 || y > 7 )
				throw new ArgumentException("yxtoidx parameter out of bounds", "y");
			if( x < 0 || x > 7 )
				throw new ArgumentException("yxtoidx parameter out of bounds", "x");

			return (byte)( y * ChessBoard.boardwid + x );
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void SetPiece(int idx, Pieces piece)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("SetPiece parameter out of bounds", "idx");
			if( piece < Pieces.NO_PIECE || piece > Pieces.BLACK_KING )
				throw new ArgumentException("SetPiece parameter out of bounds", "piece");

			board[ (byte)idx ] = piece;
			if( SimplChessClasslib.SPSetPiece != null )
				SimplChessClasslib.SPSetPiece((ushort)idx, (ushort)piece);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static PieceTypes PieceType(Pieces piece)
		{
			if( piece < Pieces.WHITE_PAWN || piece > Pieces.BLACK_KING )
				throw new ArgumentException("PieceType parameter out of bounds", "piece");

			return ( piece >= Pieces.WHITE_PAWN && piece <= Pieces.WHITE_KING ) ? (PieceTypes)piece : (PieceTypes)( piece - ( Pieces.BLACK_PAWN - Pieces.WHITE_PAWN ) );
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static PieceColors otherColor(PieceColors color)
		{
			if( color < PieceColors.WHITE_COLOR || color > PieceColors.BLACK_COLOR )
				throw new ArgumentException("otherColor parameter out of bounds color =" + color);

			return ( color == PieceColors.WHITE_COLOR ) ? PieceColors.BLACK_COLOR : PieceColors.WHITE_COLOR;
		}
		internal void capturePiece(Pieces piece)
		{
			CrestronConsole.PrintLine("ChessBoard capturePiece begin {0}", piece);

			if( piece < Pieces.WHITE_PAWN || piece > Pieces.BLACK_KING )
				throw new ArgumentException("capturePiece parameter out of bounds piece =" + piece);

			if( PieceType(piece) == PieceTypes.PAWN_TYPE )
				return;

			PieceColors color = PieceColor(piece);
			int idx = colorAsIndex(color);

			for( int i = 0; i < 8; i++ )
			{
				if( captures[ idx, i ] == Pieces.NO_PIECE )
				{
					captures[ idx, i ] = piece;
					SimplChessClasslib.SPSetCapture((ushort)idx, (ushort)i, (ushort)piece);
					break;
				}
			}
			CrestronConsole.PrintLine("ChessBoard capturePiece end", piece);
		}

		internal void makeMove(ChessMove move)
		{
			CrestronConsole.PrintLine("ChessBoard MakeMove begin from {0} to {1} capture {2} enpassant {3} castle {4}", move.from, move.to, move.capture, move.enPassant, move.castle);
			try
			{
				PieceColors color = PieceColor(move.piece);
				int fy = move.fromY;
				int fx = move.fromX;
				int ty = move.toY;
				int tx = move.toX;

				if( move.isCastle )
				{
					if( fy == 0 || fy == 7 )
					{
						int rookx = (move.castle == CastleTypes.LEFT_CASTLE) ? 0 : 7;
						int rookidx = yxtoidx(fy, rookx);

						if( move.castle == CastleTypes.LEFT_CASTLE )
						{
							SetPiece(rookidx + 3, board[ rookidx ]);
							SetPiece(rookidx, Pieces.NO_PIECE);

							SetPiece(move.from - 2, board[ move.from ]);
							SetPiece(move.from, Pieces.NO_PIECE);
						}
						else if( move.castle == CastleTypes.RIGHT_CASTLE )
						{
							SetPiece(rookidx - 2, board[ rookidx ]);
							SetPiece(rookidx, Pieces.NO_PIECE);

							SetPiece(move.from + 2, board[ move.from ]);
							SetPiece(move.from, Pieces.NO_PIECE);
						}
					}
					else
						throw new ArgumentException("makeMove detected invalid castle fy " + fy);
				}
				else
				{
					if( move.piece == Pieces.WHITE_PAWN && move.toY == 7 )
						SetPiece(move.to, Pieces.WHITE_QUEEN);
					else if( move.piece == Pieces.BLACK_PAWN && move.toY == 0 )
						SetPiece(move.to, Pieces.BLACK_QUEEN);
					else
						SetPiece(move.to, board[ move.from ]);

					SetPiece(move.from, Pieces.NO_PIECE);

					if( move.isEnPassant )
					{
						if( move.piece == Pieces.WHITE_PAWN || move.piece == Pieces.BLACK_PAWN )
							// remove the enemy pawn
							SetPiece(yxtoidx(move.fromY, move.toX), Pieces.NO_PIECE);

						else throw new ArgumentException("makeMove detected invalid enpassant piece" + move.piece);
					}
				}

				if( move.capture != Pieces.NO_PIECE )
					capturePiece(move.capture);

				int cidx = colorAsIndex(color);

				// set castling flags
				if( fy == 0 || fy == 7)
				{
					if( fx == 4 ) castlePieces[ cidx, (int)CastlePieces.KING ] = true;
					if( fx == 0 ) castlePieces[cidx, (int)CastlePieces.LEFT_ROOK ] = true;
					if( fx == 7 ) castlePieces[cidx, (int)CastlePieces.RIGHT_ROOK ] = true;
				}

				if (PieceType(move.piece) == PieceTypes.KING_TYPE)
					kingPos[cidx] = move.to;

				if( color == PieceColors.BLACK_COLOR )
				{
					// set en passant flags
					if( move.piece == Pieces.BLACK_PAWN && fy == 6 && move.toY == 4 )
						blackPawnMoved[ fx ] = true;
					// reset en passant flags for opposite color
					for( int i = 0; i < 8; i++ )
						whitePawnMoved[ i ] = false;
				}
				else if( color == PieceColors.WHITE_COLOR )
				{
					// set en passant flags
					if( move.piece == Pieces.WHITE_PAWN && fy == 1 && move.toY == 3 )
						whitePawnMoved[ fx ] = true;
					// reset en passant flags for opposite color
					for( int i = 0; i < 8; i++ )
						blackPawnMoved[ i ] = false;
				}
			}
			catch( Exception e )
			{
				CrestronConsole.PrintLine("SimplChessClassLib MakeMove generated exception {0}", e.Message);
			}

			CrestronConsole.PrintLine("ChessBoard MakeMove end");
		}

		internal int colorAsIndex(PieceColors color)
		{
			if( color < PieceColors.WHITE_COLOR || color > PieceColors.BLACK_COLOR )
				throw new ArgumentException("colorAsIndex parameter out of bounds color =" + color);

			return (int)color - 1;
		}

		internal void nextTurn()
		{
			currentTurn = otherColor(currentTurn);
		}
	}
}