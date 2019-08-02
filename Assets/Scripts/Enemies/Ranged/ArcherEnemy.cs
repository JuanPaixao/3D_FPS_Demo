using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : MonoBehaviour
{
    private Player _player; // pega referência do player
    [SerializeField] private Transform _arrowInstatiatePosition; //posição em que a flecha será criada
    [SerializeField] private float _arrowShootSpeed, _attackDistance; //velocidade do tiro e distância da visão
    private Rigidbody _rb; // referencia do rigidbody
    private EnemyAnimator _animator; // script responsável pelas animações
    private ArrowPool _arrowPool; // pool de flechas
    private Enemy _enemy; // referencia do script geral de inimigos
    private GameManager _gameManager;
    private int _myScore;
    private void Awake() // pegando as referências
    {
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<EnemyAnimator>();
        _arrowPool = FindObjectOfType<ArrowPool>();
        _enemy = GetComponent<Enemy>();
        _gameManager = FindObjectOfType<GameManager>();
        _enemy.isDead = false;
        _myScore = _enemy.ReturnScore();
    }
    private void Update()
    {
        DistanceCheck();
        transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y - 1f, _player.transform.position.z)); // olhando pro player, com uma pequena 
                                                                                                                                      // modificação na altura fazendo com que eu olhe um pouco abaixo da "cabeça"
                                                                                                                                      // do jogador.
    }
    private void DistanceCheck()
    {
        float distance = Vector3.Distance(this.transform.position, _player.transform.position); // verificação de distância
        if (distance <= _attackDistance) // se minha distância for menor que meu range de ataque, eu ataco
        {
            _animator.IdleAnimation(false);
            _animator.AttackAnimation(true);
        }
        else
        {
            _animator.IdleAnimation(true);
            _animator.AttackAnimation(false); // caso contrário, não ataco
        }
    }
    public void Shoot()
    {
        if (_arrowPool.HasArrows()) // verifica se tem flechas para poder instanciar
        {                           // é retirada a flecha da pool e avisa-se que não é criada pelo player, com isso ela é de fato "criada" pelo inimigo arqueiro
            GameObject arrowFromPool = this._arrowPool.GetArrow();
            ArrowBehaviour arrowScript = arrowFromPool.GetComponent<ArrowBehaviour>();
            arrowScript.isFromPlayer = false;
            arrowFromPool.SetActive(true);
            arrowFromPool.transform.position = _arrowInstatiatePosition.position + this.transform.forward;
            Rigidbody arrowRb = arrowFromPool.GetComponent<Rigidbody>(); // é acessado o corpo rígido da flecha e modificamos sua velocidade
            arrowRb.velocity = this._arrowShootSpeed * this.transform.forward; // de acordo com a força do tiro pré estabelecida
        }
    }
    public void TakeDamage() // tomar dano, verificar se ainda tenho vida, se sim, nada accontece, caso contrário, morte
    {
        _gameManager.SetScore(_myScore);
        _enemy.isDead = true;
        _animator.TriggerDeath();
    }
}
