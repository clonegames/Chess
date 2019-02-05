using System.Collections.Generic;


namespace Chess {

    public interface IPiece {

        void Select();
        List<BoardSquare> LegalSquares();
        void Move();
    }
}