using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreGround = null;
        [SerializeField] Health healthComponent = null;
        [SerializeField] Canvas rootCanvas = null;

        private void Update()
        {
            if (Mathf.Approximately(healthComponent.GetFraction(),1) || Mathf.Approximately(healthComponent.GetFraction(), 0))
            {
                rootCanvas.enabled=false;
                return;
            }
            rootCanvas.enabled=true;
            foreGround.localScale = new Vector3(healthComponent.GetFraction(), 1f, 1f);
        }
    }
}