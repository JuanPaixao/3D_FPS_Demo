using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyPool myPool; // pool de inimigos
    [SerializeField] private ItemPool _itemPool; // pool de itens
    public EnemyGenerator myGenerator; // identificando qual meu gerador
    public int myPositionID; // identificando qual posição eu fui criado das opções pré estabelecidas
    public bool isDead; // verificando se estou morto
    public GameObject[] drops; // objetos que eu posso dropar caso eu morra
    public GameManager _gameManager; // gerenciador
    [SerializeField] private int _myScore; // quantos pontos dar ao jogador ao morrer
    public AudioClip stepAudioLeft, stepAudioRight, deathAudio, reviveAudio, attackAudio; // sons que o inimigo emitirá
    private AudioSource _audioSource; // fonte de audio

    private void Awake() // pegando componentes
    {
        _itemPool = FindObjectOfType<ItemPool>();
        _gameManager = FindObjectOfType<GameManager>();
        _audioSource = GetComponent<AudioSource>();

    }
    public void ReturnToPool() // retornando ao pool e deletando minha posição da lista de posições já utilizadas
    {
        _gameManager.SetScore(_myScore);
        SpawnItem();
        myGenerator.DeleteMyPositionOnList(myPositionID);
        myPool.ReturnEnemy(this.gameObject);
    }
    private void SpawnItem() // escolhendo qual item vou dropar ao morrer 
    {
        if (_itemPool.HasItems())
        {
            int randomPos = Random.Range(0, drops.Length);

            GameObject item = _itemPool.GetItem();
            item.transform.position = this.transform.position;
            item.SetActive(true);
        }
    }
    public int ReturnScore() // retorna o número de pontos que o inimigo dará em sua morte 
    {
        return _myScore; 
    }
    public void StepAudio(string leg) // reproduz o audio de passos das pernas direita e esquerda
    {
        if (leg == "Right")
        {
            _audioSource.PlayOneShot(stepAudioRight, 1);
        }
        else
        {
            _audioSource.PlayOneShot(stepAudioLeft, 1);
        }
    }
    public void DeathAudio() // áudio tocado durante a morte do inimigo
    {
        _audioSource.PlayOneShot(deathAudio, 1);
    }
    public void ReviveAudio() // áudio tocado durante o spawn do inimigo
    {
        _audioSource.PlayOneShot(reviveAudio, 1);
    }
    public void AttackAudio() // audio tocado durante o ataque do inimigo
    {
        _audioSource.PlayOneShot(attackAudio, 1);
    }
}
