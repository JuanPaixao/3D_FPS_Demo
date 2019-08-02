using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemPool myPool; // pool de itens
    public string itemType; // tipo do item

    private void OnTriggerEnter(Collider other) // se entrar em colisão com o player (e só entrará com o player, 
                                                // pois o mesmo está com colisão desabilitada pela física de outros objetos) realiza a função de cura ou recarga de flechas
                                                // de acordo com o tipo do item que foi obtido, logo após isso o item volta a pool de itens
                                                
    {
        if (this.itemType == "HP_Pill")  // verifica-se qual item está sendo tocado pelo player, e cura ou da flecha ao player
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.Heal();
            myPool.ReturnItem(this.gameObject);
        }
        else if (this.itemType == "Arrow")
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.GetAmmo();
            myPool.ReturnItem(this.gameObject);
        }
    }
}
