using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Chess {

    [RequireComponent(typeof(BoxCollider))]
    public class Chessboard : Utilities.Singleton<Chessboard> {

        public event Action OnSquareSelected;

        #region Fields
        public GameObject SelectorPrefab;

        public GameObject Piece;

        public int BoardSize;

        private List<Square> m_board;

        private Vector2 m_bounds;

        private int m_selectedId;

        private int[] m_currectTargeted;

        private Piece m_selectedPiece;
        #endregion

        #region TemporaryFields
        public bool DrawGizmos;
        #endregion

        #region Initializers

        private void Awake() {
            PrepareBoard();
        }

        [ContextMenu("Prepare Board")]
        public void PrepareBoard() {

            if (BoardSize % 2 != 0) {
                BoardSize++;
            }

            var collider = GetComponent<BoxCollider>();
            m_bounds = new Vector2(collider.bounds.extents.x, collider.bounds.extents.z);
            var offset = m_bounds - (m_bounds / BoardSize);

            Clear();

            m_board = new List<Square>();
            for (int i = 0; i < BoardSize; i++) {
                for (int j = 0; j < BoardSize; j++) {
                    var position = new Vector2(m_bounds.x * ((float)j / BoardSize) * 2, m_bounds.y * ((float)i / BoardSize) * 2) - offset;
                    m_board.Add(new Square(j + (i * BoardSize), position, 10f / BoardSize, SelectorPrefab, transform));
                }
            }
            
            for (int i = 0; i < 16; i++) {
                var rand = UnityEngine.Random.Range(0, BoardSize * BoardSize - 1);
                var piece = Instantiate(Piece, transform).GetComponent<Piece>();
                piece.CurrentSquare = m_board[rand];
            }

            var material = GetComponent<Renderer>().sharedMaterial;
            material.SetInt("_GridSize", BoardSize / 2);
        }

        [ContextMenu("Clear")]
        public void Clear() {

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
                if (m_selectedPiece != null) {
                    if (closest.Targeted) {
                        m_selectedPiece.CurrentSquare = closest;                                         
                    }
                    for (int i = 0; i < m_currectTargeted.Length; i++) {
                        m_board[m_currectTargeted[i]].Selected = false;
                        m_board[m_currectTargeted[i]].Targeted = false;
                    }
                    m_selectedPiece = null;
                    m_currectTargeted = null;
                }

                m_board[m_selectedId].Selected = false;
                closest.Selected = true;
                m_selectedId = closest.Id;
                if (m_selectedPiece == null) {
                    if (closest.Piece != null) {
                        m_selectedPiece = closest.Piece;
                        m_currectTargeted = m_selectedPiece.GetTargetSquaresIds();
                        for (int i = 0; i < m_currectTargeted.Length; i++) {
                            m_board[m_currectTargeted[i]].Selected = true;
                            m_board[m_currectTargeted[i]].Targeted = true;
                        }
                    }
                }
            }
            if (OnSquareSelected != null) {
                OnSquareSelected();
            }
        }

        public Square GetNeighbourSquare(int id, Square.NeighbourDirection direction) {

            int squareId = -1;

            switch (direction) {
                case Square.NeighbourDirection.Up:
                    if (id > BoardSize * BoardSize - (BoardSize + 1)) {
                        return null;
                    }
                    squareId = id + BoardSize;
                    break;
                case Square.NeighbourDirection.UpperRight:
                    if (id > BoardSize * BoardSize - (BoardSize + 1) || id % BoardSize == BoardSize - 1) {
                        return null;
                    }
                    squareId = (id + BoardSize) + 1;
                    break;
                case Square.NeighbourDirection.Right:
                    if (id % BoardSize == BoardSize - 1) {
                        return null;
                    }
                    squareId = id + 1;
                    break;
                case Square.NeighbourDirection.DownRight:
                    if (id < BoardSize || id % BoardSize == BoardSize - 1) {
                        return null;
                    }
                    squareId = id - BoardSize + 1;
                    break;
                case Square.NeighbourDirection.Down:
                    if (id < BoardSize) {
                        return null;
                    }
                    squareId = id - BoardSize;
                    break;
                case Square.NeighbourDirection.DownLeft:
                    if (id < BoardSize || id % BoardSize == 0) {
                        return null;
                    }
                    squareId = id - BoardSize - 1;
                    break;
                case Square.NeighbourDirection.Left:
                    if (id % BoardSize == 0) {
                        return null;
                    }
                    squareId = id - 1;
                    break;
                case Square.NeighbourDirection.UpperLeft:
                    if (id > BoardSize * BoardSize - (BoardSize + 1) || id % BoardSize == 0) {
                        return null;
                    }
                    squareId = (id + BoardSize) - 1;
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

        public bool Targeted {
            get {
                return m_targeted;
            }
            set {
                m_targeted = value;
                if (m_targeted) {
                    m_selectedIndicator.GetComponent<Renderer>().material.color = Color.red;
                }
                else {
                    m_selectedIndicator.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }

        private bool m_targeted;

        private GameObject m_selectedIndicator;

        public Piece Piece;

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