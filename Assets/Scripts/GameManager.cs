using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public DotSoldier[] soldiers = new DotSoldier[50];
 
    [Header("States in Scene")]
    public State stateA;
    public State stateB;

    [Header("Players")]
    private PlayerHuman humanPlayer;
    private PlayerAI aiPlayer;

    private float aiTimer = 0f;
    private float aiMoveInterval = 2f; 

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
       
        humanPlayer = PlayerHuman.Instance;
        aiPlayer = new PlayerAI(1, "Enemy AI");

        
        stateA.SetOwner(humanPlayer, State.StateSituation.Player);
        stateB.SetOwner(aiPlayer, State.StateSituation.Enemy);
    }

    public void Attack(int soldiersCount, Vector2 startPos, Vector2 endPos)
    {
        if(soldiersCount < soldiers.Length)
        for (int i = 0; i < soldiersCount; i++)
        {
            if (soldiers[i] != null)
            {
                soldiers[i].gameObject.transform.position = startPos+new Vector2(0,Random.Range(0,5));
                soldiers[i].gameObject.SetActive(true);

                StartCoroutine(MoveSoldier(soldiers[i].transform, startPos, endPos, 0.5f + i * 0.05f));
            }
        }
    }

    private System.Collections.IEnumerator MoveSoldier(Transform soldier, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            soldier.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        soldier.position = endPos; 
        soldier.gameObject.SetActive(false);
    }
    private void Update()
    {
        aiTimer += Time.deltaTime;
        if (aiTimer >= aiMoveInterval)
        {
            aiTimer = 0f;
            aiPlayer.MakeMove();
        }
    }

}
