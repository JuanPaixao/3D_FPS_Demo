using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private float _spawnTime; // tempo entre spawn de inimigos
    [SerializeField] private Transform[] _spawnPosition; // posições em que ele será criado
    [SerializeField] private EnemyPool _enemyPool; // pool de inimigos
    [SerializeField] private int _numberOfSpawnPoints; // posição total de pontos de spawn de inimigos
    [SerializeField] private List<int> positionsSpawned; // salva em uma lista todas as posições já ocupadas pelo inimigo, para evitar que dois ou mais nasçam no mesmo local
    [SerializeField] private string enemyName; // nome do inimigo
    private UI_Manager _uiManager; // gerenciador de UI
    private float _timeToFirstSpawn; // tempo para o primeiro inimigo ser criado (será baseado no tempo de início do jogo)
    [SerializeField] private float _timeToFirstSpawnCompensation; // tempo adicional após o jogo começar que os inimigos começarão a ser spawnados
    private void Awake()
    {
        _uiManager = FindObjectOfType<UI_Manager>(); // referência do gerenciador de UI
    }
    private void Start() // pegando as referências, criando a lista e chamando as funções de spawn
    {
        _timeToFirstSpawn = _uiManager.GetCountdownNumber();
        positionsSpawned = new List<int>();
        InvokeRepeating("CreateEnemy", _timeToFirstSpawn + _timeToFirstSpawnCompensation, this._spawnTime);
        StartCoroutine(ChangeSpawnTime());
    }

    private void CreateEnemy() // criando o inimigo, verifica-se se há inimigo e aleatoriamente escolhe a posição em que ele será criado
    {
        if (_enemyPool.HasEnemies())
        {
            int randomPos = Random.Range(0, _numberOfSpawnPoints);
            if (!positionsSpawned.Contains(randomPos)) // verifica-se então se há ou não inimigo naquela posição, caso não, cria-se um inimigo na posição definida
            {                                           // e passa os parâmetros ao inimigo de onde ele veio e qual foi seu ponto de spawn
                GameObject enemy = _enemyPool.GetEnemy();
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemy.transform.position = _spawnPosition[randomPos].position;
                enemy.SetActive(true);
                positionsSpawned.Add(randomPos);
                enemyScript.myPositionID = randomPos;
                enemyScript.myGenerator = this;
                enemyScript.isDead = false;
            }
        }
    }
    private IEnumerator ChangeSpawnTime() // aleatoriamente muda o tempo entre spawns de inimigos
    {
        while (this._enemyPool.enabled)
        {
            yield return new WaitForSeconds(10);
            float randomTime = Random.Range(0f, 5f);
            _spawnTime = randomTime;
            int randomPos = Random.Range(0, _numberOfSpawnPoints);
        }
    }
    public void DeleteMyPositionOnList(int pos) // quando o inimigo é derrotado, ele libera sua posição de spawn inicial para que outro inimigo possa ser criado nela
    {
        if (positionsSpawned.Contains(pos))
        {
            positionsSpawned.Remove(pos);
        }
    }
}
