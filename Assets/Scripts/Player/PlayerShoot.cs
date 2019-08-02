using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private Animator _bowAnimator;
    // [SerializeField] private GameObject _arrow; // flecha que será atirada
    [SerializeField] private Transform _arrowInstatiatePosition; //posição em que a flecha será criada
    [SerializeField] private float _shootForce; // força de tiro da flecha.
    [SerializeField] private Player _player; // referencia script do player
    private ArrowPool _arrowPool; // referencia ao pool de flechas
    private UI_Manager _uiManager; // referencia ao gerenciador de interface

    private void Awake() // pegando as referências para o funcionamento do animador do arco do player, pool de flechas e interface
    {
        _bowAnimator = GetComponent<Animator>();
        _arrowPool = FindObjectOfType<ArrowPool>();
        _uiManager = FindObjectOfType<UI_Manager>();
    }
    public void Shoot()
    {
        if (_arrowPool.HasArrows()) // se há flechas, a flecha é ativada e é criada na posição frontal do jogador, e informa-se que a flecha foi criada pelo jogador
        {
            GameObject arrowFromPool = this._arrowPool.GetArrow();
            ArrowBehaviour arrowBehaviour = arrowFromPool.GetComponent<ArrowBehaviour>();
            arrowBehaviour.OnActive();
            arrowFromPool.SetActive(true);
            arrowFromPool.transform.position = _arrowInstatiatePosition.position + this.transform.forward;
            arrowBehaviour.isFromPlayer = true;
            Rigidbody arrowRb = arrowFromPool.GetComponent<Rigidbody>(); // é acessado o corpo rígido da flecha e modificamos sua velocidade
            arrowRb.velocity = _shootForce * Camera.main.transform.forward; // de acordo com a força do tiro pré estabelecida
            _player.arrowQuantity--;// reduzir quantidade de flechas do player
            _uiManager.UpdateArrowNumber(this._player.arrowQuantity); // atualiza na interface o número de flechas
        }
    }
    public void FinishedShoot() // avisa que o tiro já finalizou
    {
        _bowAnimator.SetBool("Shooting", false);
        _player.arrowCharged = false;
        _player.arrowMeshRenderer.enabled = true;
    }
    public void ShootingAnimation(bool shooting) // tiro em execução, já não está mais carregando
    {
        _bowAnimator.SetBool("Shooting", shooting);
        _player.arrowCharged = false;
    }
    public void ChargedAnimationComplete() // notificando que a animação de carregamento de tiro está completa
    {
        _player.arrowCharged = true;
    }
}
