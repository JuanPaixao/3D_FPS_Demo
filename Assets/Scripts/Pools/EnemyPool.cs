using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private int _enemyQuantity; // quantidade de itens na pool
    [SerializeField] private GameObject[] _enemyPrefab; // objeto a ser guardado

    private Stack<GameObject> _enemyPool; // pilha do objeto guardado
    [SerializeField] private int _enemyType; // variável para dinamizar os objetos de forma aleatória dentro da pilha

    private void Start() // pegando as referências e criando todos os objetos
    {
        _enemyPool = new Stack<GameObject>();
        CreateAllEnemies();
    }

    private void CreateAllEnemies() // criando e colocando os objetos criados na pilha
    {
        for (int i = 0; i < this._enemyQuantity; i++)
        {
            int randomEnemy = Random.Range(0, _enemyType);
            GameObject enemy = GameObject.Instantiate(this._enemyPrefab[randomEnemy], this.transform);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemy.SetActive(false);
            enemyScript.myPool = this;
            this._enemyPool.Push(enemy);
        }
    }

    public GameObject GetEnemy() // pegando os objetos da pilha
    {
        return this._enemyPool.Pop();
    }
    public void ReturnEnemy(GameObject enemy) // devolvendo os objetos a pilha
    {
        enemy.SetActive(false);
        this._enemyPool.Push(enemy);
    }
    public bool HasEnemies() // verificando se a pilha não está vazia
    {
        return this._enemyPool.Count > 0;
    }
}
