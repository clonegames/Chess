#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using EzEditor;
using UnityEditor;
using UnityEngine;

namespace Chess {

    [CustomEditor(typeof(Chessboard))]
    public class ChessboardEditor : Editor {

        private Chessboard m_target;

        public override void OnInspectorGUI() {

            if (m_target == null) {
                m_target = target as Chessboard;
            }

            if (gui.EzButton("Refresh")){
                m_target.PrepareBoard();
            }
            if (gui.EzButton("Clear")) {
                m_target.Clear();
            }
            DrawFields();
          
        }

        private void DrawFields() {
            m_target.SelectorPrefab = gui.EzGameObjectField("Selector Prefab", m_target.SelectorPrefab, 0f);

            m_target.Piece = gui.EzGameObjectField("Piece Prefab", m_target.Piece, 0f);

            m_target.BoardSize = gui.EzIntField("Board Size", m_target.BoardSize);
            if (m_target.BoardSize % 2 != 0) {
                m_target.BoardSize++;
            }
        }
    }
}
#endif