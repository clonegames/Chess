using UnityEngine;
using System.Collections;

namespace Chess {

    public class Piece : MonoBehaviour {

        public Square.NeighbourDirection MoveDirection;

        private Square m_currentSquare;

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


        public int[] GetTargetSquaresIds() {
            int[] targetIds = new int[1];

            var target = m_currentSquare.QueryNeighbour(MoveDirection);
            if (target != null) {
                targetIds[0] = target.Id;
                return targetIds;
            }
            return null;
        }
    }
}