using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Chess {

    [RequireComponent(typeof(BoxCollider))]
    public class Chessboard : Utilities.Singleton<Chessboard> {

        public event Action OnSquareSelected;

        #region Fields
        [SerializeField]
        private GameObject m_selector;

        [SerializeField]
        private int m_boardSize;

        private List<Square> m_board;

        private Vector2 m_bounds;

        private int m_selectedId;
        #endregion

        #region TemporaryFields
        public bool DrawGizmos;
        #endregion

        #region Initializers

        private void Awake() {
            PrepareBoard();
        }

        private void OnValidate() {
            if (m_boardSize % 2 != 0) {
                m_boardSize++;
            }
        }

        [ContextMenu("Prepare Board")]
        private void PrepareBoard() {

            if (m_boardSize % 2 != 0) {
                m_boardSize++;
            }

            var collider = GetComponent<BoxCollider>();
            m_bounds = new Vector2(collider.bounds.extents.x, collider.bounds.extents.z);
            var offset = m_bounds - (m_bounds / m_boardSize);

            Clear();

            m_board = new List<Square>();
            for (int i = 0; i < m_boardSize; i++) {
                for (int j = 0; j < m_boardSize; j++) {
                    var position = new Vector2(m_bounds.x * ((float)j / m_boardSize) * 2, m_bounds.y * ((float)i / m_boardSize) * 2) - offset;
                    m_board.Add(new Square(j + (i * m_boardSize), position, 10f / m_boardSize, m_selector, transform));
                }
            }
            m_board[m_selectedId].Selected = true;

            var material = GetComponent<Renderer>().sharedMaterial;
            material.SetInt("_GridSize", m_boardSize / 2);
        }

        [ContextMenu("Clear")]
        private void Clear() {

            var childs = transform.childCount;

            for (int i = childs - 1; i >= 0; i--) {
#if UNITY_EDITOR
                if (Application.isEditor) {
                    
                    DestroyImmediate(transform.GetChild(i).gameObject);
                    continue;
                }
#endif
                Destroy(transform.GetChild(i).gameObject);
            }

            if (m_board != null)
                m_board.Clear();
        }

#endregion

#region Public

        public void SelectSquareAt(Vector3 position) {
            Square closest = null;
            float minimumDist = float.PositiveInfinity;
            m_board.ForEach(square => {
                var dist = (square.Position - position).sqrMagnitude;
                if (dist < minimumDist) {
                    minimumDist = dist;
                    closest = square;
                }
            });
            if (closest != null) {
                m_board[m_selectedId].Selected = false;
                closest.Selected = true;
                m_selectedId = closest.Id;
            }
            if (OnSquareSelected != null) {
                OnSquareSelected();
            }
        }

        public Square GetNeighbourSquare(int id, Square.NeighbourDirection direction) {

            int squareId = -1;

            switch (direction) {
                case Square.NeighbourDirection.Up:
                    if (id > m_boardSize * m_boardSize - (m_boardSize + 1)) {
                        return null;
                    }
                    squareId = id + m_boardSize;
                    break;
                case Square.NeighbourDirection.UpperRight:
                    if (id > m_boardSize * m_boardSize - (m_boardSize + 1) || id % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = (id + m_boardSize) + 1;
                    break;
                case Square.NeighbourDirection.Right:
                    if (id % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = id + 1;
                    break;
                case Square.NeighbourDirection.DownRight:
                    if (id < m_boardSize || id % m_boardSize == m_boardSize - 1) {
                        return null;
                    }
                    squareId = id - m_boardSize + 1;
                    break;
                case Square.NeighbourDirection.Down:
                    if (id < m_boardSize) {
                        return null;
                    }
                    squareId = id - m_boardSize;
                    break;
                case Square.NeighbourDirection.DownLeft:
                    if (id < m_boardSize || id % m_boardSize == 0) {
                        return null;
                    }
                    squareId = id - m_boardSize - 1;
                    break;
                case Square.NeighbourDirection.Left:
                    if (id % m_boardSize == 0) {
                        return null;
                    }
                    squareId = id - 1;
                    break;
                case Square.NeighbourDirection.UpperLeft:
                    if (id > m_boardSize * m_boardSize - (m_boardSize + 1) || id % m_boardSize == 0) {
                        return null;
                    }
                    squareId = (id + m_boardSize) - 1;
                    break;
            }
            if (squareId == -1) {
                Debug.LogWarning("Something went wrong when searching the square at direction " + direction + "!");
                return null;
            }

            return m_board[squareId];
        }
#endregion

#region Visual

        private void OnDrawGizmos() {
            if (!DrawGizmos)
                return;

            if (m_board == null) {
                return;
            }

            m_board.ForEach(square => {
                if (square.Selected) {
                    Gizmos.color = Color.green;
                }
                else {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(square.Position + Vector3.up * 0.2f, 0.2f);
            });
        }

#endregion

    }

    public class Square {

        public enum NeighbourDirection {
            Up,
            UpperRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpperLeft,
            None
        }

        public int Id { get; private set; }

        public Vector3 Position { get; set; }

        public bool Selected {
            get {
                return m_selectedIndicator.activeInHierarchy;
            }
            set {
                m_selectedIndicator.SetActive(value);
            }
        }

        private GameObject m_selectedIndicator;

        public IPiece Piece;

        public Square(int id, Vector2 position, float size, GameObject selected, Transform board) {
            Id = id;
            Position = new Vector3(position.x, 0, position.y);
            m_selectedIndicator = GameObject.Instantiate(selected, Position, Quaternion.identity, board);
            m_selectedIndicator.transform.localScale = new Vector3(size * 0.75f, 0.2f, size * 0.75f);
            m_selectedIndicator.SetActive(false);
        }

        public Square QueryNeighbour(NeighbourDirection direction) {
            return Chessboard.Instance.GetNeighbourSquare(Id, direction);
        }
    }
}