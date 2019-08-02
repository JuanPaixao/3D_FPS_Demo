using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    [SerializeField] private int _itemQuantity; // quantidade de itens na pool
    [SerializeField] private GameObject[] _itemPrefab; // objeto a ser guardado

    private Stack<GameObject> _itemPool; // pilha do objeto guardado
    [SerializeField] private int _itemType; // variável para dinamizar os objetos de forma aleatória dentro da pilha

    private void Start() // pegando as referências e criando todos os objetos
    {
        _itemPool = new Stack<GameObject>();
        CreateAllItems();
    }

    private void CreateAllItems() // criando e colocando os objetos criados na pilha
    {
        for (int i = 0; i < this._itemQuantity; i++)
        {
            int randomItem = Random.Range(0, _itemType);
            GameObject itemObject = GameObject.Instantiate(this._itemPrefab[randomItem], this.transform);
            Item itemScript = itemObject.GetComponent<Item>();
            itemObject.SetActive(false);
            itemScript.myPool = this;
            this._itemPool.Push(itemObject);
        }
    }

    public GameObject GetItem() // pegando os objetos da pilha
    {
        return this._itemPool.Pop();
    }
    public void ReturnItem(GameObject item) // devolvendo os objetos a pilha
    {
        item.SetActive(false);
        this._itemPool.Push(item);
    }
    public bool HasItems() // verificando se a pilha não está vazia
    {
        return this._itemPool.Count > 0;
    }
}
