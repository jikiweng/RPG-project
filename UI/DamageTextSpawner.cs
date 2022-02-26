using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damage)
        {
            if (damageTextPrefab == null) return;

            DamageText instance=Instantiate(damageTextPrefab, transform);
            instance.SetValue(damage);
        }
    }
}