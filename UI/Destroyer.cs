using UnityEngine;

namespace RPG.UI
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject objectToDestroy=null;

        public void DestroyObject()
        { Destroy(objectToDestroy); }
    }
}