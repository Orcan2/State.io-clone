using TMPro;
using UnityEngine;

public class State : MonoBehaviour
{
    [Header("State Properties")]
    public int stateID;
    public _Player Owner;
    public int unitCount = 0;
    public float productionRate = 1f; 

    [Header("References")]
    public TextMeshPro unitCountText;
    public SpriteRenderer spriteRenderer; 

    public int debugUnitMultiplier=1;
    private float timer;

    public enum StateSituation
    {
        Player,
        Enemy,
        Neutral
    }

    public StateSituation currentSituation = StateSituation.Neutral;

    void Start()
    {
        UpdateColor();
        UpdateUnitText();
    }

    void Update()
    {
        
        timer += Time.deltaTime;
        if (timer >= 1f / productionRate)
        {
            
            unitCount+=debugUnitMultiplier;
            timer = 0f;
            UpdateUnitText();
        }
    }

    public void SetOwner(_Player newOwner, StateSituation situation)
    {
        Owner = newOwner;
        currentSituation = situation;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (spriteRenderer == null) return;

        switch (currentSituation)
        {
            case StateSituation.Player:
                spriteRenderer.color = Color.blue;
                break;
            case StateSituation.Enemy:
                spriteRenderer.color = Color.red;
                break;
            case StateSituation.Neutral:
                spriteRenderer.color = Color.gray;
                break;
        }
    }

    public void UpdateUnitText()
    {
        if (unitCountText != null)
            unitCountText.text = unitCount.ToString();
    }
}
