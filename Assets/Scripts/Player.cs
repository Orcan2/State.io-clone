using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class _Player
{
    public int Id;          
    public string Name;
    public bool IsAI;

    protected _Player(int id, string name, bool isAI)
    {
        Id = id;
        Name = name;
        IsAI = isAI;
    }
}


public class PlayerHuman : _Player
{
    
    
    private static PlayerHuman _instance;
    public static PlayerHuman Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerHuman(0, "Human Player");
            return _instance;
        }
    }

    private PlayerHuman(int id, string name) : base(id, name, false) { }

    public void HandleInput(ref State selectedState, ref bool isDragging)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("dokunuldu");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,+10));
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                Debug.Log(hit.name);
                State clickedState = hit.GetComponent<State>();
                if (clickedState != null && clickedState.Owner == this)
                {
                    selectedState = clickedState;
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, +10));
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                State targetState = hit.GetComponent<State>();
                if (targetState != null && targetState != selectedState)
                {
                    _Attack(selectedState, targetState);
                }
            }

            selectedState = null;
            isDragging = false;
        }
    }

    private void _Attack(State fromState, State targetState)
    {
        if (fromState.unitCount < 1) return;

        int soldiersToSend = fromState.unitCount;
        

        if (targetState.Owner != this)
        {
            GameManager.Instance.Attack(soldiersToSend,fromState.transform.position,targetState.transform.position);
            fromState.unitCount -= soldiersToSend;
            targetState.unitCount -= soldiersToSend;
            if (targetState.unitCount <= 0)
            {
                targetState.unitCount = Mathf.Abs(targetState.unitCount);
                targetState.SetOwner(this, State.StateSituation.Player);
            }
        }
        else if(targetState.Owner == this)
        {
            fromState.unitCount -= soldiersToSend;
            GameManager.Instance.Attack(soldiersToSend, fromState.transform.position, targetState.transform.position);
            targetState.unitCount += soldiersToSend;
        }

        

        fromState.UpdateUnitText();
        targetState.UpdateUnitText();
        targetState.UpdateColor();
    }
}



public class PlayerAI : _Player
{
    public PlayerAI(int id, string name) : base(id, name, true) { }

    public void MakeMove()
    {
        var allStates = Object.FindObjectsByType<State>(FindObjectsSortMode.None).ToList();
        if (allStates.Count < 2) return;

        
        State fromState = allStates[Random.Range(0, allStates.Count)];
        State toState = allStates[Random.Range(0, allStates.Count)];

      
        if (fromState == toState) return;

        if (fromState.unitCount < 1) return;

        int soldiersToSend = Mathf.Max(1, fromState.unitCount / 2);

       
        if (fromState.Owner != this) return;

        
        if (toState.Owner == this)
        {
            fromState.unitCount -= soldiersToSend;
            GameManager.Instance.Attack(soldiersToSend, fromState.transform.position, toState.transform.position);
            toState.unitCount += soldiersToSend;
        }
        else
        {
           
            GameManager.Instance.Attack(soldiersToSend, fromState.transform.position, toState.transform.position);
            fromState.unitCount -= soldiersToSend;
            toState.unitCount -= soldiersToSend;

            if (toState.unitCount <= 0)
            {
                toState.unitCount = Mathf.Abs(toState.unitCount);
                toState.SetOwner(this, State.StateSituation.Enemy);
            }
        }

        fromState.UpdateUnitText();
        toState.UpdateUnitText();
        toState.UpdateColor();
    }
}

