using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int baseDmg;

    public void OnTriggerEnter(Collider target)
    {
        if (target == null) return;

        Healthbar hp = target.GetComponentInChildren<Healthbar>();
        if (hp != null)
        {
            hp.LoseHealth(baseDmg);
        }
        print(target.name);
        Destroy(this.gameObject);
    }

    Healthbar FindHealthbarInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Healthbar health = child.GetComponent<Healthbar>();
            if (health != null)
                return health;

            health = FindHealthbarInChildren(child);
            if (health != null)
                return health;
        }

        return null;
    }
}
