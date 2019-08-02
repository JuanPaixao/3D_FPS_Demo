using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator; // pegando o animador e setando as animações nas funções abaixo de acordo com os parâmetros estabelecidos, para fazer o controle
                                // das animações
    private void Awake()
    {
        _animator = GetComponent<Animator>(); // pegando componente de animação
    }

    public void RunAnimation(bool run)  // animação de correr
    {
        _animator.SetBool("Run", run); 
    }
    public void IdleAnimation(bool idle) // animação idle
    {
        _animator.SetBool("Idle", idle);
    }
    public void WalkAnimation(bool walk) // animação de andar (não implementada)
    {
        _animator.SetBool("Walk", walk);
    }
    public void AttackAnimation(bool attack) // animação de atacar
    {
        _animator.SetBool("Attack", attack);
    }
    public void TriggerDeath() // animação de morte
    {
        _animator.SetTrigger("Death");
    }
}
