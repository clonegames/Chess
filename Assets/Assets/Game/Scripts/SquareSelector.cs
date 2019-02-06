using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess {

    public class SquareSelector : MonoBehaviour {

        private Camera m_camera;


        private void Awake() {
            m_camera = Camera.main;
        }

        private void Update() {
            if (Input.GetMouseButton(0)) {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.PositiveInfinity,
                    1 << LayerMask.NameToLayer(GameConstants.LAYER_BOARD))){
                    Chessboard.Instance.SelectSquareAt(hit.point);
                }
            }
        }
    }
}