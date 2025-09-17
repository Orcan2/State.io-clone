using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<DotSoldier> soldiers = new List<DotSoldier>();
    public DotSoldier soldierPrefab;
    public static List<State> allStates;
    [Header("States in Scene")]
    public State stateA;
    public State stateB;

    [Header("Players")]
    private PlayerHuman humanPlayer;
    public PlayerAI AIPlayer { get; private set; }

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
        allStates = Object.FindObjectsByType<State>(FindObjectsSortMode.None).ToList();
    }

    void Start()
    {
        humanPlayer = PlayerHuman.Instance;
        AIPlayer = new PlayerAI(1, "Enemy AI");

        stateA.SetOwner(humanPlayer, State.StateSituation.Player);
        stateB.SetOwner(AIPlayer, State.StateSituation.Enemy);

        // baþlangýç havuzu
        for (int i = 0; i < 30; i++)
        {
            CreateSoldier();
        }
    }

    private DotSoldier CreateSoldier()
    {
        DotSoldier newSoldier = Instantiate(soldierPrefab, transform);
        newSoldier.gameObject.SetActive(false);
        soldiers.Add(newSoldier);
        return newSoldier;
    }

    private DotSoldier GetAvailableSoldier()
    {
        foreach (var soldier in soldiers)
        {
            if (!soldier.gameObject.activeInHierarchy)
                return soldier;
        }
        return CreateSoldier();
    }

    public void Attack(int soldiersCount, Vector2 startPos, Vector2 endPos, State owner)
    {
        for (int i = 0; i < soldiersCount; i++)
        {
            DotSoldier soldier = GetAvailableSoldier();
            soldier.transform.position = startPos;
            soldier.gameObject.SetActive(true);

            // id yerine owner enum kullanýyoruz
            soldier.owner = owner.Owner.IsAI ? DotSoldier.SoldierOwner.AI : DotSoldier.SoldierOwner.Player;

            // spawn state atamasý
            soldier.spawnState = owner;

            StartCoroutine(MoveSoldier(
                soldier.transform,
                startPos,
                endPos,
                0.5f + i * 0.05f
            ));
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
        // hedefe vardýðýnda çarpýþma tetiklenecek
    }

    private void Update()
    {
        aiTimer += Time.deltaTime;
        if (aiTimer >= aiMoveInterval)
        {
            aiTimer = 0f;
            AIPlayer.MakeMove();
        }
    }
}
