using UnityEngine;
using System.Collections;

namespace Chess {

    public class Piece : MonoBehaviour {

        public Square.NeighbourDirection MoveDirection;

        private Square m_currentSquare;

        public bool WhiteTeam;      
        
        public int[] TargetedSquares {
            get { return m_targetedSquares; }
        }

        private int[] m_targetedSquares;

        public Square CurrentSquare {
            get {
                return m_currentSquare;
            } set {

                if (m_currentSquare != null)
                    m_currentSquare.Piece = null;
                m_currentSquare = value;
                m_currentSquare.Piece = this;
                transform.position = m_currentSquare.Position;
            }
        }


        public void UpdateTargetedSquares() {
            m_targetedSquares = new int[2];

            var target = m_currentSquare.QueryNeighbour(MoveDirection);
            Square target2 = null;
            if (target != null) {
                target2 = target.QueryNeighbour(MoveDirection);
                m_targetedSquares[0] = target.Id;
            }
            else {
                m_targetedSquares[0] = -1;
            }
            if (target2 != null) {
                m_targetedSquares[1] = target2.Id;
            }
            else {
                m_targetedSquares[1] = -1;
            }
        }
    }
}