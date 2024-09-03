using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundController : MonoBehaviour
{
    public static RoundController instance;

    [SerializeField]
    private int round = 0;

    [SerializeField]
    private List<EnemyController> enemies;

    public UnitController player;
    [SerializeField]
    private PrefabPositionPair playerPrefab;

    [SerializeField]
    private List<PrefabPositionPair> enemiesToSpawn;

    [SerializeField]
    private GameObject defeatCanvas;
    [SerializeField]
    private GameObject playerUICanvas;

    private int currentEnemy = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnEnemies();
        
        round++;
        StartCoroutine(DeckManager.instance.FirstStart());
    }

    private void SpawnPlayer()
    {
        player = Instantiate(playerPrefab.prefab).GetComponent<UnitController>();

        player.SetUnitStats(playerPrefab.position.x, playerPrefab.position.y);
    }

    private void SpawnEnemies()
    {
        foreach(var enemyPref in enemiesToSpawn)
        {
            EnemyController enemy = Instantiate(enemyPref.prefab).GetComponent<EnemyController>();

            enemy.SetUnitStats(enemyPref.position.x, enemyPref.position.y);

            enemies.Add(enemy);
        }
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(StartEnemyTurnCoroutine());
    }

    public IEnumerator StartEnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1);

        if (enemies.Count > 0)
            enemies[0].StartAction();
        else
        {
            yield return new WaitForSeconds(1);
            StartPlayerTurn();
        }
    }

    public IEnumerator NextEnemyAction()
    {
        currentEnemy++;
        if (currentEnemy <= enemies.Count - 1)
        {
            yield return new WaitForSeconds(1);
            enemies[currentEnemy].StartAction();
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartPlayerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        currentEnemy = 0;
        round++;
        StartCoroutine(DeckManager.instance.NewTurnCoroutine());
    }

    public void PlayerLost()
    {
        playerUICanvas.SetActive(false);
        defeatCanvas.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(0);
    }
}
