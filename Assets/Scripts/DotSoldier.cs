using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DotSoldier : MonoBehaviour
{
    public enum SoldierOwner { AI, Player }
    public SoldierOwner owner;

    [HideInInspector] public State spawnState;
     
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        DotSoldier otherSoldier = other.GetComponent<DotSoldier>();
        if (otherSoldier != null && otherSoldier.owner != this.owner)
        {
            
            this.gameObject.SetActive(false);
            otherSoldier.gameObject.SetActive(false);   //Başka takımın askeriyle çatışma havuza geri atma
            return; 
        }

        
        State state = other.GetComponent<State>();
        if (state == null) return; 
        if (spawnState == null) spawnState = state; //Kendi statetine çarpmayı algılamasın
        if (state == spawnState) return; 

        
        if (owner == SoldierOwner.AI && state.currentSituation == State.StateSituation.Player)
        {
            if (GameManager.Instance?.AIPlayer == null) return; 
            state.unitCount--;
            if (state.unitCount <= 0)
            {
                state.unitCount = Mathf.Abs(state.unitCount);
                state.SetOwner(GameManager.Instance.AIPlayer, State.StateSituation.Enemy);
            }
        }
      
        else if (owner == SoldierOwner.Player && state.currentSituation == State.StateSituation.Enemy)
        {
            state.unitCount--;
            if (state.unitCount <= 0)
            {
                state.unitCount = Mathf.Abs(state.unitCount);
                state.SetOwner(PlayerHuman.Instance, State.StateSituation.Player);
            }
        }
        
        else if ((owner == SoldierOwner.AI && state.currentSituation == State.StateSituation.Enemy) ||
                 (owner == SoldierOwner.Player && state.currentSituation == State.StateSituation.Player))
        {
            state.unitCount++;
        }
        
        else if (state.currentSituation == State.StateSituation.Neutral)
        {
            state.unitCount++;
            if (owner == SoldierOwner.Player)
                state.SetOwner(PlayerHuman.Instance, State.StateSituation.Player);
            else if (GameManager.Instance?.AIPlayer != null)
                state.SetOwner(GameManager.Instance.AIPlayer, State.StateSituation.Enemy);
        }

        state.UpdateUnitText();
        state.UpdateColor();

       
        this.gameObject.SetActive(false);
    }
}
