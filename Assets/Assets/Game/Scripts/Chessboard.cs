using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess {

    public class Chessboard : Utilities.Singleton<Chessboard> {

        [SerializeField]
        private int m_boardSize;

        public List<BoardSquare> Board { get; private set; }

        private int temp_pivot;

        private void Awake() {
            Board = new List<BoardSquare>();
            for (int i = 0; i < m_boardSize * m_boardSize; i++) {
                Board.Add(new BoardSquare(i));
            }
        }

        private void Update() {
            if (Input.anyKeyDown) {

                print("Estou no bloco: " + temp_pivot);
                var randomDir = (BoardSquare.NeighbourDirection)Random.Range(0, 8);
                print("Vou tentar a direção: " + randomDir);
                var square = GetNeighbourSquare(temp_pivot, randomDir);
                if (square != null) {
                    print("Achei o bloco " + square.Id + " na direção: " + randomDir);
                    temp_pivot = square.Id;
                }
                else {
                    print("Não tinha nenhum bloco na direção: " + randomDir);
                }
            }
        }

        public BoardSquare GetNeighbourSquare(int pivot, BoardSquare.NeighbourDirection direction) {

            int squareId = -1;

            switch (direction) {
                case BoardSquare.NeighbourDirection.Up: 
                    if (pivot < m_boardSize) {
                        return null;
                    }
                    squareId = pivot - m_boardSize;
                    break;
                case BoardSquare.NeighbourDirection.UpperRight:
                    if (pivot < m_boardSize || pivot % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = (pivot - m_boardSize) + 1;
                    break;
                case BoardSquare.NeighbourDirection.Right:
                    if(pivot % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = pivot + 1;
                    break;
                case BoardSquare.NeighbourDirection.DownRight:
                    if (pivot > m_boardSize * m_boardSize - m_boardSize || pivot % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = pivot + m_boardSize + 1;
                    break;
                case BoardSquare.NeighbourDirection.Down:
                    if (pivot > m_boardSize * m_boardSize - m_boardSize) {
                        return null;
                    }
                    squareId = pivot + m_boardSize;          
                    break;
                case BoardSquare.NeighbourDirection.DownLeft:
                    if (pivot > m_boardSize * m_boardSize - m_boardSize || pivot % m_boardSize == 0) {
                        return null;
                    }
                    squareId = pivot + m_boardSize - 1;
                    break;
                case BoardSquare.NeighbourDirection.Left:
                    if (pivot % m_boardSize == 0) {
                        return null;
                    }
                    squareId = pivot - 1;
                    break;
                case BoardSquare.NeighbourDirection.UpperLeft:
                    if (pivot < m_boardSize || pivot % m_boardSize == 0) {
                        return null;
                    }
                    squareId = (pivot - m_boardSize) - 1;
                    break;
            }
            return Board[squareId];
        }
    }

    [System.Serializable]
    public class BoardSquare {

        public enum NeighbourDirection {
            Up,
            UpperRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpperLeft
        }

        public int Id { get; private set; }

        public IPiece Piece;

        public BoardSquare(int id) {
            Id = id;
        }

        public BoardSquare QueryNeighbour(NeighbourDirection direction) {
            return Chessboard.Instance.GetNeighbourSquare(Id, direction);
        }

    }

}