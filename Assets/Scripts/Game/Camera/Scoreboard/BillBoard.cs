using UnityEngine;

namespace Game.Player
{
    public class BillBoard : MonoBehaviour
    {
        private UnityEngine.Camera _cam;
    
        private void Update()
        {
            if (_cam == null)
                _cam = FindObjectOfType<UnityEngine.Camera>();
            if (_cam == null) return;
        
            transform.LookAt(_cam.transform);
        }
    }
}
