using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class ItemManager : MonoBehaviourPunCallbacks
{
    public Transform itemPosition;

    public List<Item> items = new List<Item>();

    public Item selectedItem;
    public int selectedItemIndex = 0;
    public GameObject selectedItemGameObject;

    PhotonView PV;

    [SerializeField] PlayerController playerController;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        if (!PV.IsMine)
        {
            return;
        }
        SelectItem(0);
       
    }

    void Update()
    {
        if (!PV.IsMine || playerController.isPaused)
        {
            return;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // Scroll Up - Go to the next item in list (increasing)

            if (items.Count > selectedItemIndex + 1)
            {
                SelectItem(selectedItemIndex + 1);
            } else
            {
                SelectItem(0);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // Scroll Down - Go to the previous item in list (decreasing)

            if (selectedItemIndex - 1 >= 0)
            {
                SelectItem(selectedItemIndex - 1);
            }
            else
            {
                SelectItem(items.Count -1);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (items[selectedItemIndex] != null)
            {
                Item item = items[selectedItemIndex];
                item.Use();
                
            }
        }

    }

    public void SelectItem(int index)
    {
        // Set the current item's gameobject to deactivated
        items[selectedItemIndex].itemGameObject.SetActive(false);

        // Set the new selected item.
        items[index].itemGameObject.SetActive(true);
        selectedItem = items[index];
        selectedItemIndex = index;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", selectedItemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public Item GetItem(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name.Equals(itemName))
            {
                return items[i];
            }
        }
        throw new System.Exception("Could not find item with the name: " + itemName);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            SelectItem((int) changedProps["itemIndex"]);
        }
    }

}
