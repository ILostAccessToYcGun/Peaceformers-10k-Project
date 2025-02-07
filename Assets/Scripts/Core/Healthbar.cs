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
    [SerializeField]private float maxHealth = 100f;
    private float currentHealth;

    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;
    

    void Start()
    {
        float newScaleX = maxHealth * 0.0022f;
        mainBar.localScale = new Vector3(newScaleX, mainBar.localScale.y, mainBar.localScale.z);

        currentHealth = maxHealth;
        UpdateHealthbar();
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthbar();
    }

    public void LoseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthbar();
    }

    public void GainHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        float healthRatio = currentHealth / maxHealth;

        healthbarFill.fillAmount = healthRatio;

        if (currentHealth > (maxHealth / 2))
            healthbarFill.color = goodHealthColor;
        else if (currentHealth > (maxHealth / 4))
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
