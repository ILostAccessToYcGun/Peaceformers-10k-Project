using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Bullet : MonoBehaviour
{
    public int baseDmg;
    public Transform source;
    public GameObject p;

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
                StationaryEnemy gun = source.GetComponent<StationaryEnemy>();
                if (gun.parentSettlement != null)// aka if it's a settlement enemy
                {
                    gun.parentSettlement.GetComponent<Settlement>().GainMeter(baseDmg / 20f); //steal half the upkeep you deduct
                }
                
            }
                
        }
        print(target.name);

        Material mat = target.GetComponentInChildren<Renderer>().material;

        GameObject particleObject = Instantiate(p, this.transform.position, Quaternion.identity);

        particleObject.GetComponent<ParticleSystemRenderer>().material = mat;
        particleObject.GetComponent<ParticleSystemRenderer>().trailMaterial = mat;


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
