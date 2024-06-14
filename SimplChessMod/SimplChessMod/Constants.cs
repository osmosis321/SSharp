namespace SimplChessMod
{
	internal enum GameStates : byte
	{
		ONGOING = 0,
		WHITEWIN = 1,
		BLACKWIN = 2,
		DRAW = 3
	}
	internal enum CheckStats: byte
	{
		NO_CHECK = 0,
		WHITE_CHECK = 1,
		BLACK_CHECK = 2
	}
	internal enum CastlePieces : byte
	{
		KING = 0,
		LEFT_ROOK = 1,
		RIGHT_ROOK = 2
	}
	internal enum PieceColors : byte
	{
		NO_COLOR = 0,
		WHITE_COLOR = 1,
		BLACK_COLOR = 2
	}
	internal enum Pieces : byte
	{
		NO_PIECE = 0,
		WHITE_PAWN = 1,
		WHITE_ROOK = 2,
		WHITE_KNIGHT = 3,
		WHITE_BISHOP = 4,
		WHITE_QUEEN = 5,
		WHITE_KING = 6,

		BLACK_PAWN = 7,
		BLACK_ROOK = 8,
		BLACK_KNIGHT = 9,
		BLACK_BISHOP = 10,
		BLACK_QUEEN = 11,
		BLACK_KING = 12
	}
	internal enum SquareStates : byte
	{
		SQ_UNSEL = 0,
		SQ_SEL = 1
	}
	internal enum PieceTypes : byte
	{
		PAWN_TYPE = Pieces.WHITE_PAWN,
		ROOK_TYPE = Pieces.WHITE_ROOK,
		KNIGHT_TYPE = Pieces.WHITE_KNIGHT,
		BISHOP_TYPE = Pieces.WHITE_BISHOP,
		QUEEN_TYPE = Pieces.WHITE_QUEEN,
		KING_TYPE = Pieces.WHITE_KING
	}
	internal enum CastleTypes : byte
	{
		NO_CASTLE = 0,
		LEFT_CASTLE = 1,
		RIGHT_CASTLE = 2
	}
	internal partial class ChessBoard
	{
		internal const byte boardhgt = 8;
		internal const byte boardwid = 8;

		private readonly Pieces[] DefaultPieces = new Pieces[64] {  Pieces.WHITE_ROOK, Pieces.WHITE_KNIGHT,   Pieces.WHITE_BISHOP,   Pieces.WHITE_QUEEN,    Pieces.WHITE_KING, Pieces.WHITE_BISHOP,   Pieces.WHITE_KNIGHT,   Pieces.WHITE_ROOK,
																	Pieces.WHITE_PAWN, Pieces.WHITE_PAWN,     Pieces.WHITE_PAWN,     Pieces.WHITE_PAWN,     Pieces.WHITE_PAWN, Pieces.WHITE_PAWN,     Pieces.WHITE_PAWN,     Pieces.WHITE_PAWN,
																	Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,
																	Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,
																	Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,
																	Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,   Pieces.NO_PIECE,       Pieces.NO_PIECE,       Pieces.NO_PIECE,
																	Pieces.BLACK_PAWN, Pieces.BLACK_PAWN,     Pieces.BLACK_PAWN,     Pieces.BLACK_PAWN,     Pieces.BLACK_PAWN, Pieces.BLACK_PAWN,     Pieces.BLACK_PAWN,     Pieces.BLACK_PAWN,
																	Pieces.BLACK_ROOK, Pieces.BLACK_KNIGHT,   Pieces.BLACK_BISHOP,   Pieces.BLACK_QUEEN,    Pieces.BLACK_KING, Pieces.BLACK_BISHOP,   Pieces.BLACK_KNIGHT,   Pieces.BLACK_ROOK };
	}
}
