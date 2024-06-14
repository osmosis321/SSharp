namespace SimplChessMod
{
    internal class ChessMove
    {
        public byte from;
        public byte to;
        public Pieces piece;
        public Pieces capture;
        public bool enPassant;
        public CastleTypes castle;

        internal ChessMove(byte From, byte To, Pieces Piece, Pieces Capture = Pieces.NO_PIECE, bool EnPassant = false, CastleTypes Castle = CastleTypes.NO_CASTLE)
		{
            from = From;
            to = To;
            piece = Piece;
            capture = Capture;
            enPassant = EnPassant;
            castle = Castle;
        }

        internal bool Equals(ChessMove move)
        {
            if( from != move.from ) goto notequal;
            if( to != move.to ) goto notequal;
            if( piece != move.piece ) goto notequal;
            if( capture != move.capture ) goto notequal;
            if( enPassant != move.enPassant ) goto notequal;
            if( castle != move.castle ) goto notequal;
            return true;
        notequal: return false;
        }

        internal byte fromY { get => ChessBoard.idxtoy(from); }
        internal byte fromX { get => ChessBoard.idxtox(from); }
        internal byte toY { get => ChessBoard.idxtoy(to); }
        internal byte toX { get => ChessBoard.idxtox(to); }

        // returns true if the move is a castle move
        internal bool isCastle { get => castle != CastleTypes.NO_CASTLE; }
        internal bool isEnPassant { get => enPassant; }
    };
}
