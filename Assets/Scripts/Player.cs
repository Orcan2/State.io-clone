using System.Linq;
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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, +10));
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
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
        fromState.unitCount -= soldiersToSend;

        GameManager.Instance.Attack(soldiersToSend, fromState.transform.position, targetState.transform.position, fromState);

        fromState.UpdateUnitText();
    }
}

public class PlayerAI : _Player
{
    public PlayerAI(int id, string name) : base(id, name, true) { }

    public void MakeMove()
    {
        //var allStates = Object.FindObjectsByType<State>(FindObjectsSortMode.None).ToList();
        var allStates = GameManager.allStates;
        if (allStates.Count < 2) return;

        State fromState = allStates[Random.Range(0, allStates.Count)];
        
        State toState = allStates[Random.Range(0, allStates.Count)];

        if (fromState == toState) return;
        if (fromState.unitCount < 2) return;
        if (fromState.Owner != this) return;

        int soldiersToSend = fromState.unitCount ;
        fromState.unitCount -= soldiersToSend;

        GameManager.Instance.Attack(soldiersToSend, fromState.transform.position, toState.transform.position, fromState);

        fromState.UpdateUnitText();
    }
}
