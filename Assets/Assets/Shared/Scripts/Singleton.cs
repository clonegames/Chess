using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities {

    public class Singleton<T> : MonoBehaviour where T : Singleton<T> {

        private static T m_instance;

        public static T Instance {
            get {
                return m_instance != null ? m_instance : (m_instance = FindObjectOfType<T>());
            }
        }
    }
}