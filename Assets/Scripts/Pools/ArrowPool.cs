using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private int _arrowsQuantity; // quantidade de itens na pool
    [SerializeField] private GameObject _arrow; // objeto a ser guardado
    private Stack<GameObject> _arrowPool; // pilha do objeto guardado
 
    private void Start() // pegando as referências e criando todos os objetos
    {
        _arrowPool = new Stack<GameObject>();
        CreateAllArrows();
    }

    private void CreateAllArrows() // criando e colocando os objetos criados na pilha
    {
        for (int i = 0; i < this._arrowsQuantity; i++)
        {
            GameObject arrow = GameObject.Instantiate(this._arrow, this.transform);
            arrow.SetActive(false);
            this._arrowPool.Push(arrow);
        }
    }

    public GameObject GetArrow() // pegando objeto da pilha
    {
        return this._arrowPool.Pop();
    }
    public void ReturnArrow(GameObject arrow) //retornando objeto para a pilha
    {
        arrow.SetActive(false);
        this._arrowPool.Push(arrow);
    }
    public bool HasArrows() //verificando se ainda há objetos na pilha
    {
        return this._arrowPool.Count > 0;
    }
}
