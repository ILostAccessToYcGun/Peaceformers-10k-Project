using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private RectTransform mainBar;
    [SerializeField] private Image healthbarFill;
    [SerializeField] private Image healthbarLoss;
    [SerializeField] private float lossLerpSpeed = 2f; 
    [Space]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float M_maxHealth;
    protected float currentHealth;

    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;
    

    public virtual void Start()
    {
        M_maxHealth = maxHealth;
        ScaleUI();
    }

    public void ScaleUI()
    {
        float newScaleX = M_maxHealth * 0.0022f;
        mainBar.localScale = new Vector3(newScaleX, mainBar.localScale.y, mainBar.localScale.z);

        currentHealth = M_maxHealth;
        UpdateHealthbar();
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, -0.1f, M_maxHealth);
        UpdateHealthbar();
    }

    public virtual void LoseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, -0.1f, M_maxHealth);
        UpdateHealthbar();
    }

    public void GainHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, -0.1f, M_maxHealth);
        UpdateHealthbar();
    }

    public float GetCurrentHealth() { return currentHealth; }
    public void SetMaxHealth(float newMaxHealth) { M_maxHealth = newMaxHealth; ScaleUI(); }
    public float GetMaxHealth() { return M_maxHealth; }

    public void UpdateHealthbar()
    {
        float healthRatio;
        if (M_maxHealth == 0)
            healthRatio = 1f;
        else
            healthRatio = currentHealth / M_maxHealth;

        healthbarFill.fillAmount = healthRatio;

        if (currentHealth > (M_maxHealth / 2))
            healthbarFill.color = goodHealthColor;
        else if (currentHealth > (M_maxHealth / 4))
            healthbarFill.color = watchOutColor;
        else
            healthbarFill.color = criticalColor;
        
        StopAllCoroutines();
        StartCoroutine(LerpHealthbarLoss(healthRatio));
    }

    private IEnumerator LerpHealthbarLoss(float targetFill)
    {
        float startFill = healthbarLoss.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * lossLerpSpeed;
            healthbarLoss.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime);
            yield return null;
        }

        healthbarLoss.fillAmount = targetFill;
    }
}
