using System;
using Crestron.SimplSharp;

namespace SimplChessMod
{

	public delegate void SetPieceDel(ushort idx, ushort value);
	public delegate void SetTurnDel(ushort turn);
	public delegate void SetGamestateDel(ushort state);
	public delegate void SetCheckStatDel(ushort check);
	public delegate void SetCaptureDel(ushort player, ushort idx, ushort piece);

	public class SimplChessClasslib
	{
		public static SetPieceDel SPSetPiece { get; set; }
		public static SetPieceDel SPSetSquare { get; set; }
		public static SetTurnDel SPSetTurn { get; set; }
		public static SetGamestateDel SPSetGamestate { get; set; }
		public static SetCheckStatDel SPSetCheckStat { get; set; }
		public static SetCaptureDel SPSetCapture { get; set; }

		private int cursor;
		private ChessBoard board;

		public SimplChessClasslib()
		{
			board = new ChessBoard();
			cursor = -1;
		}

		public void ResetGame()
		{
			try
			{
				UIResetCursor();
				board.Reset();

				UIUpdateGameState(board.gameState);
				UIUpdateTurn(board.currentTurn);
				UIUpdateCheckStat(board.checkStat);
				UITransferSquares();
				UITransferPieces();
				UITransferCaptures();
			}
			catch( Exception e )
			{
				CrestronConsole.PrintLine("SimplChessClassLib ResetGame generated exception {0}", e.Message);
			}
		}

		public void HandlePress(ushort mover, ushort pressidx)
		{
			CrestronConsole.PrintLine("SimplChessClassLib HandlePress started mover = {0} pressindex = {1} currentturn = {2} gamestate = {3}", mover, pressidx, board.currentTurn, board.gameState);
			try
			{
				// If not our turn, throw exception
				if( mover != (ushort)board.currentTurn )
					throw new Exception("SimplChessClassLib HandlePress mover != currentTurn");

				// If game is not on, exit
				if( board.gameState != GameStates.ONGOING )
					goto finishhandlepress;

				// Is the cursor already there?
				if( pressidx == cursor )
				{
					// reset the cursor and exit
					UIResetCursor();
					goto finishhandlepress;
				}

				Pieces presspiece = board.board[pressidx];
				PieceColors presscolor = ChessBoard.PieceColor(presspiece);

				// Did we click one of our own pieces?
				if( mover == (ushort)presscolor )
				{
					// Move the cursor to the new spot and exit
					UIPlaceCursor(pressidx);
					goto finishhandlepress;
				}

				// does a cursor exist?
				if( cursor > -1 )
				{
					// try to move from the cursor to the clicked square
					byte fromidx = (byte)cursor;
					Pieces frompiece = board.board[fromidx];
					//PieceColors fromcolor = ChessBoard.PieceColor(frompiece);

					// get legal moves
					// Is this a legal move?

					if( board.legalMoves.Count == 0 )
						throw new Exception("SimplChessClassLib HandlePress 0 legal moves exist");

					ChessMove move = board.legalMoves.Find(x => (x.from == fromidx) && (x.to == pressidx) && (x.piece == frompiece));
					if( move != null )
					{
						PieceColors ocolor = ChessBoard.otherColor( board.currentTurn);
						UIResetCursor();
						board.makeMove(move);
						board.ComputeLegalMoves(ocolor);
						UIUpdateCheckStat(board.checkStat);
						UIUpdateGameState(board.gameState);
						if( board.gameState == GameStates.ONGOING )
							board.nextTurn();
						else board.currentTurn = PieceColors.NO_COLOR;
						UIUpdateTurn(board.currentTurn);

						CrestronConsole.PrintLine("SimplChessClassLib HandlePress turn complete.  {0} has {1} legal moves", board.currentTurn, board.legalMoves.Count);
					}
					else
						CrestronConsole.PrintLine("SimplChessClassLib HandlePress no legal move found {0}", pressidx);
				}
				else
					CrestronConsole.PrintLine("SimplChessClassLib HandlePress cannot make move, no piece selected press {0} cursor {1}", pressidx, cursor);
				finishhandlepress:
				CrestronConsole.PrintLine("SimplChessClassLib HandlePress end");
				return;
			}
			catch( Exception e )
			{
				CrestronConsole.PrintLine("SimplChessClassLib HandlePress generated exception {0}", e.Message);
			}
		}
		private void UIResetCursor()
		{
			CrestronConsole.PrintLine("SimplChessClassLib ResetCursor");
			if( cursor > -1 )
				SPSetSquare((ushort)cursor, (ushort)SquareStates.SQ_UNSEL);
			cursor = -1;
		}
		private void UITransferPieces()
		{
			CrestronConsole.PrintLine("SimplChessClassLib TransferPieces");
			if( SPSetPiece != null )
			{
				for( ushort i = 0; i < 64; i++ )
					SPSetPiece(i, (ushort)board.board[ i ]);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UITransferPieces delegate is null");
		}
		private void UITransferSquares()
		{
			CrestronConsole.PrintLine("SimplChessClassLib TransferSquares");
			if( SPSetSquare != null )
			{
				for( ushort i = 0; i < 64; i++ )
					SPSetSquare(i, (ushort)( cursor == i ? 1 : 0 ));
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UITransferSquares delegate is null");
		}
		private void UITransferCaptures()
		{
			CrestronConsole.PrintLine("SimplChessClassLib TransferCaptures");
			if( SPSetSquare != null )
			{
				for( int i = 0; i < 2; i++ )
					for( int j = 0; j < 8; j++ )
						SPSetCapture((ushort)i, (ushort)j, (ushort)board.captures[ i, j ]);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UITransferCaptures delegate is null");
		}
		private void UIPlaceCursor(int idx)
		{
			if( idx < -1 || idx > 63 )
				throw new ArgumentException("placeCursor index out of range idx = " + idx);

			CrestronConsole.PrintLine("SimplChessClassLib UIPlaceCursor {0}", idx);
			if ( SPSetSquare != null )
			{
				if( cursor > -1 )
					SPSetSquare((ushort)cursor, (ushort)SquareStates.SQ_UNSEL);
				cursor = idx;
				if( cursor > -1 )
					SPSetSquare((ushort)cursor, (ushort)SquareStates.SQ_SEL);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UIPlaceCursor delegate is null");
		}
		private void UIUpdatePiece(int idx, Pieces piece)
		{
			if( idx < 0 || idx > 63 )
				throw new ArgumentException("UIUpdatePiece index out of range idx = " + idx);

			if( SPSetPiece != null )
			{
				SPSetPiece((ushort)idx, (ushort)piece);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UIUpdatePiece delegate is null");
		}
		internal static void UIUpdateTurn(PieceColors turn)
		{
			if( SPSetTurn != null )
			{
				SPSetTurn((ushort)turn);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UIUpdateTurn delegate is null");
		}
		private void UIUpdateGameState(GameStates state)
		{
			CrestronConsole.PrintLine("SimplChessClassLib UIUpdateGameState {0}", state);
			if( SPSetGamestate != null )
			{
				SPSetGamestate((ushort)state);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UIUpdateGameState delegate is null");
		}
		private void UIUpdateCheckStat(CheckStats check)
		{
			CrestronConsole.PrintLine("SimplChessClassLib UIUpdateCheckStat {0}", check);
			if( SPSetCheckStat != null )
			{
				SPSetCheckStat((ushort)check);
			}
			else
				CrestronConsole.PrintLine("SimplChessClassLib UIUpdateCheckStat delegate is null");
		}
	}
}
