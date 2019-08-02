using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    private Rigidbody _rb; // corpo rígido
    private Collider _collider; // colisor
    public bool isFromPlayer; // identificação se essa flecha foi atirada pelo jogador
    [SerializeField] private float _gravity; // gravidade manual
    private ArrowPool _arrowPool; // pool de flechas
    private void Awake() //pegando os componentes para controle, 
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
        _arrowPool = FindObjectOfType<ArrowPool>();
    }
    public void OnActive() // quando for ativada, rotacionar para direção da velocidade do seu corpo
    {
        transform.rotation = Quaternion.LookRotation(_rb.velocity);
    }
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_rb.velocity); //manter a rotação para que o movimento simule a realidade
        this._rb.velocity = new Vector3(this._rb.velocity.x, this._rb.velocity.y - _gravity, this._rb.velocity.z); // força da gravidade

        if (this.transform.position.y < -2) // caso atravesse o terreno em áreas sem colisão e caia, volte para a pool
        {
            _arrowPool.ReturnArrow(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) // se a flecha colidir em um dos inimigos e for jogador, o inimigo morre e a flecha retorna
    {                                           
        if (isFromPlayer) 
        {
            if (other.gameObject.CompareTag("WarriorEnemy") || other.gameObject.CompareTag("ArcherEnemy"))
            {
                EnemyAnimator enemy = other.gameObject.GetComponent<EnemyAnimator>();
                enemy.TriggerDeath();
                _arrowPool.ReturnArrow(this.gameObject);
            }
        }
    }
}
