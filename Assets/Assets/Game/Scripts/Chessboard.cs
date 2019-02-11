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

        public GameObject WhitePiece, BlackPiece;

        public int BoardSize;

        private List<Square> m_board;

        private Vector2 m_bounds;

        private int m_selectedId = -1;

        private int m_turnNumber;

        private int[] m_currectTargeted;

        private Piece m_selectedPiece;

        private List<Piece> m_whiteTeam = new List<Piece>(), m_blackTeam = new List<Piece>();
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

            for (int i = 0; i < BoardSize * 2; i++) {
                m_whiteTeam.Add(Instantiate(WhitePiece, transform).GetComponent<Piece>());
                m_whiteTeam[i].CurrentSquare = m_board[i];
                m_whiteTeam[i].transform.localScale /= BoardSize / 8;

                m_blackTeam.Add(Instantiate(BlackPiece, transform).GetComponent<Piece>());
                m_blackTeam[i].CurrentSquare = m_board[(BoardSize * BoardSize - 1) - i];
                m_blackTeam[i].transform.localScale /= BoardSize / 8;
            }
            
            NextTurn();

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

            m_whiteTeam.Clear();
            m_blackTeam.Clear();
        }

#endregion

#region Public

        public void SelectSquareAt(Vector3 position) {
            
            //Test closest square
            Square closest = null;
            float minimumDist = float.PositiveInfinity;
            m_board.ForEach(square => {
                var dist = (square.Position - position).sqrMagnitude;
                if (dist < minimumDist) {
                    minimumDist = dist;
                    closest = square;
                }
            });
            //--------------------

            if (closest != null) {
                //in case the player is already with a piece selected
                if (m_selectedPiece != null) {

                    //in case the closest square is targeted by the selected piece
                    if (closest.Targeted) {
                        //move to the square
                        if (closest.Piece) {
                            if (closest.Piece.WhiteTeam != m_selectedPiece.WhiteTeam) {
                                Destroy(closest.Piece.gameObject);
                                closest.Piece = null;
                                m_selectedPiece.CurrentSquare = closest;
                                NextTurn();
                            }
                        }
                        else {
                            m_selectedPiece.CurrentSquare = closest;
                            NextTurn();
                        }                                            
                    }

                    //either way, unselect and untarget all squares 
                    foreach (var t in m_currectTargeted) {
                        if (t == -1)
                            continue;
                        m_board[t].Selected = false;
                        m_board[t].Targeted = false;
                    }

                    //and deselect the piece
                    m_selectedPiece = null;
                    m_currectTargeted = null;
                    SetSelectedSquare(null);
                    return;
                }
                
                //in case the selected square has a piece
                if (closest.Piece != null) {

                    if (closest.Piece.WhiteTeam && m_turnNumber % 2 == 1 ||
                        !closest.Piece.WhiteTeam && m_turnNumber % 2 == 0) {
                        //unselects last piece and selects the new one
                        SetSelectedSquare(closest);
                        m_selectedPiece = closest.Piece;
                        m_currectTargeted = m_selectedPiece.TargetedSquares;
                        if (m_currectTargeted != null) {
                            foreach (var t in m_currectTargeted) {
                                if (t == -1)
                                    continue;
                                m_board[t].Selected = true;
                                m_board[t].Targeted = true;
                            }
                        }
                        else {
                            m_selectedPiece = null;
                            SetSelectedSquare(null);
                        }

                        if (OnSquareSelected != null) {
                            OnSquareSelected();
                        }
                    }
                }
            }          
        }

        private void SetSelectedSquare(Square square) {

            if (m_selectedId != -1) {
                m_board[m_selectedId].Selected = false;
            }                
            
            if (square == null) {             
                m_selectedId = -1;
                return;
            }
            square.Selected = true;
            m_selectedId = square.Id;
        }

        private void NextTurn() {
            m_turnNumber++;

            Action<Piece> debugHighlight = p => {
                p.UpdateTargetedSquares();
                var targets = p.TargetedSquares;
                for (int i = 0; i < targets.Length; i++) {
                    if (targets[i] != -1)
                        m_board[targets[i]].DebugTargeted = true;
                }
            };
            if (m_turnNumber % 2 == 1) {
                m_whiteTeam.ForEach(debugHighlight);
                
            }
            else {
                m_blackTeam.ForEach(debugHighlight);
            }
            print(m_turnNumber);            
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
                if (square.DebugTargeted) {
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

        public bool DebugTargeted;

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