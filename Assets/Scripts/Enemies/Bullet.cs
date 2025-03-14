using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int baseDmg;
    public Transform source;

    public void OnTriggerEnter(Collider target)
    {
        if (target == null) return;

        Healthbar hp = target.GetComponentInChildren<Healthbar>();
        Settlement set = target.GetComponent<Settlement>();
        if (hp != null)
        {
            hp.LoseHealth(baseDmg);
        }
        else if (set != null)
        {
            Debug.Log(source.gameObject.tag);
            if (source.gameObject.CompareTag("Target") == true)
            {
                set.LoseMeter(baseDmg / 10f);
                
            }
                
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
