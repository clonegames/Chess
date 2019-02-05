using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess {

    [RequireComponent(typeof(BoxCollider))]
    public class Chessboard : Utilities.Singleton<Chessboard> {

        [SerializeField]
        private int m_boardSize;


        private List<BoardSquare> m_board;

        private Vector2 m_bounds;

        private int temp_pivot;

        private void Awake() {
            
            var collider = GetComponent<BoxCollider>();
            m_bounds = new Vector2(collider.bounds.extents.x, collider.bounds.extents.z);
            var offset = m_bounds - (m_bounds / m_boardSize);
            
            m_board = new List<BoardSquare>();
            for (int i = 0; i < m_boardSize; i++) {
                for (int j = 0; j < m_boardSize; j++) {
                    var position = new Vector2(m_bounds.x * ((float)j / m_boardSize) * 2, m_bounds.y * ((float)i / m_boardSize) * 2) - offset;
                    m_board.Add(new BoardSquare(j + (i * m_boardSize), position));   
                }                        
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

        private void OnDrawGizmos() {
            m_board.ForEach(square => {
                Gizmos.DrawSphere(square.Position, 0.2f);
            });
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
            return m_board[squareId];
        }
    }

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

        public Vector3 Position { get; set; }

        public IPiece Piece;

        public BoardSquare(int id, Vector2 position) {
            Id = id;
            Position = new Vector3(position.x, 0, position.y);
        }

        public BoardSquare QueryNeighbour(NeighbourDirection direction) {
            return Chessboard.Instance.GetNeighbourSquare(Id, direction);
        }

    }

}