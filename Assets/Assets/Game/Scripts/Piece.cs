using UnityEngine;
using System.Collections;

namespace Chess {

    public class Piece : MonoBehaviour {

        public Square.NeighbourDirection MoveDirection;

        private Square m_currentSquare;

        public bool WhiteTeam;


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
            int[] targetIds = new int[2];

            var target = m_currentSquare.QueryNeighbour(MoveDirection);
            Square target2 = null;
            if (target != null) {
                target2 = target.QueryNeighbour(MoveDirection);
                targetIds[0] = target.Id;
            }
            else {
                targetIds[0] = -1;
            }
            if (target2 != null) {
                targetIds[1] = target2.Id;
            }
            else {
                targetIds[1] = -1;
            }
            return targetIds;
        }
    }
}