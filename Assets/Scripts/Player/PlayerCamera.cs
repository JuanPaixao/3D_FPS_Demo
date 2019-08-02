using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity; // sensibildiade do mouse
    [SerializeField] private float _yRot; // rotação em torno de Y
    [SerializeField] private float _xRot;// rotação em torno de X
    private Player _player; // script do jogador

    void Awake()
    {
        _yRot = 0; // valor inicial em 0 para assegurar que o limitador funcione corretamente
        _player = GetComponent<Player>(); // componente do script do jogador
    }

    void Update()
    {
        if (_player.canPlay) // verifica se o jogo já passou da contagem inicial e está liberado para jogar
        {
            CameraRotation(); // rotação da câmera
        }
    }

    private void CameraRotation()
    {
        float _yMouse = Input.GetAxis("Mouse Y"); // pegando valor do eixo Y e X do mouse
        float _mouseX = Input.GetAxis("Mouse X");

        _yRot -= _yMouse * _mouseSensitivity; // multiplicando Y pela sensibilidade
        _yRot = Mathf.Clamp(_yRot, -75.0f, 75.0f); // e limitando a rotação do mouse para que a câmera não gire 360 graus
        _xRot += _mouseX * _mouseSensitivity;  // multiplicando X pela sensibilidade
        Vector3 myRotation = transform.localEulerAngles; // pegando meus angulos eulerianos locais
        myRotation = new Vector3(_yRot, _xRot, this.transform.eulerAngles.z); // setando minha rotação de acordo com os cálculos acima
        transform.localEulerAngles = myRotation; // transformando minha rotação em meus angulos eulerianos local (em relação ao meu próprio corpo)
    }
}
