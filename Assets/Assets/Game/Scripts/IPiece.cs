using System.Collections.Generic;


namespace Chess {

    public interface IPiece {

        void Select();
        List<Square> LegalSquares();
        void Move();
    }
}