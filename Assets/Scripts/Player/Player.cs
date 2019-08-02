using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _movHor, _movVer; // valores dos eixos horizontal e vertical
    [SerializeField] private float _walkSpeed; // velocidade de movimento
    [SerializeField] private float _jumpForceMultiplier; // força do pulo 
    [SerializeField] private float _circleSphereRay; // tamanho do círculo de detecção do chão para saber se o player está em solo
    private CharacterController _characterController; // componente para controle do player
    [SerializeField] private AnimationCurve jumpAnimationCurve; // curva de animação do pulo, representando o pulo do jogador em gráfico
    [SerializeField] private bool isJumping, isDead; // status se está ou não pulando e se está vivo ou não
    [SerializeField] private float _playerHP, _invencibilityTime, _actualInvencibilityTime; // vida do jogador, tempo de invenciblidade após tomar hit
                                                                                            // e o tempo real do contador do tempo de invencibilidade

    [SerializeField] private bool _chargingArrow; // indicador se a flecha está carregando
    [SerializeField] PlayerShoot _playerShoot; // script responsável pelo tiro em sí
    public bool arrowCharged; // indicador se a flecha já está carregada
    private UI_Manager _uiManager; // gerenciador de interface
    public int arrowQuantity; // quantidade de flecha
    public MeshRenderer arrowMeshRenderer; // renderização da flecha, para quando estiver sem a flecha não ser visível
    private float _initialMaxHP; // hp máximo inicial para ter o controle do valor máximo de vida
    public bool canPlay; // verifica se o countdown inicial já terminou para eu poder jogar
    private AudioSource _audioSource; // componente de sons para o player
    public AudioClip shootClip, takeDamageClip; // som da flecha sendo disparada e de dano tomado
    public Animator cameraAnimator; // responsável pelas animações da câmera
    private GameManager _gameManager; // gerenciador de jogo

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>(); // pegando componente que realiza o controle do player
        _uiManager = FindObjectOfType<UI_Manager>(); // buscando referencia do script 
        _audioSource = GetComponent<AudioSource>();
        _gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        _uiManager.UpdateArrowNumber(this.arrowQuantity);
        _initialMaxHP = _playerHP;
    }
    public void CastShoot() // animação e preparação do tiro
    {
        arrowMeshRenderer.enabled = false;
        _chargingArrow = false;
        _playerShoot.ShootingAnimation(true);
        _playerShoot.Shoot();
        _audioSource.PlayOneShot(shootClip, 1);
    }

    private void Update()
    {
        if (canPlay)
        {
            if (!isDead) // se não está morto, movimenta-se
            {
                Movement();

                if (arrowQuantity > 0)
                {
                    if (Input.GetMouseButtonUp(0)) // se botão solto e flecha carregada, inicia-se a preparação para atirar
                    {
                        arrowMeshRenderer.enabled = true;
                        if (arrowCharged)
                        {
                            CastShoot();
                        }
                    }
                }
                else if (arrowQuantity <= 0) // se o player não tem flechas, a flecha não é mais renderizada
                {
                    arrowMeshRenderer.enabled = false;
                }
            }
            _actualInvencibilityTime += Time.deltaTime; // somamos ao tempo de cooldown a cada frame
        }
    }

    private void Movement()
    {
        _movHor = Input.GetAxis("Horizontal"); // verifica inputs horizontal para o movimento
        _movVer = Input.GetAxis("Vertical"); // verifica o input vertical para o movimento

        Vector3 forwardMovement = transform.forward * _movVer * _walkSpeed; // verifica o movimento em torno do eixo Z
        Vector3 horizontalMovement = transform.right * _movHor * _walkSpeed; // verifica o movimento em torno do eixo X

        _characterController.SimpleMove(forwardMovement + horizontalMovement); // move em torno do eixo Z e X

        if (Input.GetButtonDown("Jump") && _characterController.isGrounded) // se pressionado o botão de pulo e estiver no chão
        {
            Jump(); // pular!
        }
    }
    private void Jump()
    {
        isJumping = true; // o status de pulo se torna verdadeiro
        StartCoroutine(JumpRoutine()); // inicia a routina responsável pelo pulo
    }
    private IEnumerator JumpRoutine()
    {
        float timeInTheAir = 0.0f; //inicia a função em relação ao gráfico de pulo em 0 (tempo)
        do
        {
            float jumpForce = jumpAnimationCurve.Evaluate(timeInTheAir); // baseando-se no timeInTheAir, é feito o pulo de acordo com o gráfico
            _characterController.Move(Vector3.up * _jumpForceMultiplier * Time.deltaTime); // move o player para cima multiplicando pelo modificar de força do pulo
            timeInTheAir += Time.deltaTime; // soma-se tempo ao tempo no ar, para que se continue seguindo o gráfico de pulo estabeleicido
            yield return null;
        }
        while (!_characterController.isGrounded && _characterController.collisionFlags != CollisionFlags.Above); // enquanto o jogador não estiver no chão, e não estiver com colisões em cima do player
        {
            isJumping = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_invencibilityTime < _actualInvencibilityTime) // se o tempo total já for menor que o tempo sem levar dano
        {
            if (other.gameObject.CompareTag("Sword")) // e for algo que dê dano no jogador entrando em colisão
            {
                TakeDamage(); // chamará a função que representa que o jogador tomou dano
            }
            if (other.gameObject.CompareTag("Arrow"))
            {
                ArrowBehaviour arrow = other.gameObject.GetComponentInParent<ArrowBehaviour>();
                if (!arrow.isFromPlayer)
                {
                    TakeDamage();
                }
            }
        }
    }
    public void TakeDamage()
    {
        _playerHP--;
        _audioSource.PlayOneShot(takeDamageClip, 1); // som de dano
        cameraAnimator.SetTrigger("Hit"); // animação de dano
        _uiManager.UpdateSlideHP(_playerHP); // atualizar barra de vida
        if (this._playerHP <= 0) // caso o jogador tenha menos que 0 ou 0 de vida, morrerá
        {
            _uiManager.UpdateSlideHP(_playerHP); // atualizar a vida do player na barra
            isDead = true;  //indicar que está morto e que não pode mais jogar
            canPlay = false;
            _gameManager.FinishGame(); // finalizar o jogo
        }
        _actualInvencibilityTime = 0;// caso não, somente será subtraída sua vida e o mesmo terá um tempo de invencibilidade
    }
    public void Heal() // função de quando o player pegar vida
    {
        if (_playerHP < _initialMaxHP)
        {
            _playerHP++;
            _uiManager.UpdateSlideHP(_playerHP);
        }
    }
    public void GetAmmo() // função de quando o palyer pegar munição
    {
        arrowQuantity++;
        _uiManager.UpdateArrowNumber(arrowQuantity);
    }
}
