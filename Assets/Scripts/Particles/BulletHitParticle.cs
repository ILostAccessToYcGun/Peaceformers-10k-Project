using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BulletHitParticle : MonoBehaviour
{
    private void Awake()
    {
        Invoke("Die", 1f);
    }


    private void Die()
    {
        Destroy(gameObject);
    }

}
