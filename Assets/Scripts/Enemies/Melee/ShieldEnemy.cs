using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : MonoBehaviour
{
    private Player _player; // pega referência do player
    [SerializeField] private float _speed, _gravity, _playerDetectionDistance, _attackDistance; // setar valores de velocidade, gravidade, distancia p/ detecçao do player,
                                                                                                // distancia p/ atacar corpo a corpo
    public bool canFollowPlayer; // indicador de se pode seguir  player
    private Rigidbody _rb; // referencia do rigidbody
    private EnemyAnimator _animator; // script responsável pelas animações
    [SerializeField] private float attackCooldown, actualAttackCooldown; // cooldown de ataque, cooldown de ataque real no momento
    private Enemy _enemy; // referencia do script geral de inimigos
    private GameManager _gameManager; // referencia gerenciador de jogo
    private int _myScore; // indicador de quantos pontos eu dou ao jogador quando morro

    private void Awake() // pegando as referências
    {
        _player = FindObjectOfType<Player>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<EnemyAnimator>();
        _gameManager = FindObjectOfType<GameManager>();
        this._enemy = GetComponent<Enemy>();
        this._enemy.isDead = false;
        this._myScore = _enemy.ReturnScore();
    }
    private void Update()
    {
        if (!this._enemy.isDead) // se não estiver morto, verificar distância do jogador naquele momento e adicionar tempo ao cooldown atual, 
                                 //assim possibilitando o ataque quando a distância estiver de corpo a corpo
        {
            DistanceCheck();
            actualAttackCooldown += Time.deltaTime;
        }
        this._rb.velocity = new Vector3(this._rb.velocity.x, this._rb.velocity.y - _gravity, this._rb.velocity.z); // força da gravidade
    }

    private void DistanceCheck()
    {
        float distance = Vector3.Distance(this.transform.position, _player.transform.position); // verificação de distância
        if (actualAttackCooldown > attackCooldown) // se não estou em cooldown, ativo meu corpo e posso me mover, seguir o jogador, ou ataca-lo
        {
            if (distance <= _attackDistance)
            {
                _rb.isKinematic = false;
            }

            if (distance <= _playerDetectionDistance && distance > _attackDistance) // se estou dentro do range de detecção porém fora do de ataque, sigo o jogador
            {
                FollowPlayer(true);
                _animator.RunAnimation(true);
                _animator.IdleAnimation(false);
                _animator.AttackAnimation(false);

                if (canFollowPlayer) // seguindo o jogador
                {
                    _rb.isKinematic = false;
                    transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y - 1f, _player.transform.position.z));
                    transform.position += transform.forward * _speed * Time.deltaTime;
                }
            }
            else if (distance > _playerDetectionDistance) // se não, volto pro idle
            {
                _rb.isKinematic = true;
                FollowPlayer(false);
                _animator.RunAnimation(false);
                _animator.IdleAnimation(true);
                _animator.AttackAnimation(false);
            }
            else if (distance <= _attackDistance) // porém, se estiver perto do range de ataque, paro de seguir e ataco de fato
            {
                FollowPlayer(false);
                _animator.RunAnimation(false);
                _animator.IdleAnimation(false);
                _animator.AttackAnimation(true);
            }
        }
        else // volto pro idle pelo cooldown do ataque, e depois sigo o jogador novamente ou ataco dependendo da condição
        {
            _animator.RunAnimation(false);
            _animator.IdleAnimation(true);
            _animator.AttackAnimation(false);
        }
    }

    public void TakeDamage() // tomar dano, verificar se ainda tenho vida, se sim, nada accontece, caso contrário, morte
    {
        _gameManager.SetScore(_myScore);
        this._enemy.isDead = true;
        _rb.velocity = Vector3.zero;
    }
    public void FollowPlayer(bool follow) // função para definir se posso ou não seguir o jogador
    {
        this.canFollowPlayer = follow;
    }
    public void AttackFinished() // função ativada pela animação de ataque finalizado, para virar para o jogador e entrar em cooldown
    {
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y - 1f, _player.transform.position.z)); // olhando pro player, com uma pequena 
                                                                                                                                      // modificação na altura fazendo com que eu olhe um pouco abaixo da "cabeça"
                                                                                                                                      // do jogador.
        transform.position += transform.forward * Time.deltaTime;
        actualAttackCooldown = 0;
    }
}
