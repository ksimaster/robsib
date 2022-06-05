using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class MoveToggle : MonoBehaviour
    {
        public Toggle moveToggle;
        
        void Awake()
        {
            moveToggle.isOn = PlayerPrefs.GetInt(PlayerConstants.MoveMode) == 0;
        }
    }
}
