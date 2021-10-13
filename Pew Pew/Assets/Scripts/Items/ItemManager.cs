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

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //  Go to the last item in list (decreasing) - Going to the left

            if (items.Count > selectedItemIndex + 1)
            {
                SelectItem(selectedItemIndex + 1);
            } else
            {
                SelectItem(0);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // Go to the next item in list (increasing) - Going to the right

            if (selectedItemIndex - 1 >= 0)
            {
                SelectItem(selectedItemIndex - 1);
            }
            else
            {
                SelectItem(items.Count -1);
            }
        }

        if (getNumeralKeycode() != -1 && getNumeralKeycode() != 0)
        {
            SelectItem(getNumeralKeycode() - 1);
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
        if (index >= items.Count)
        {
            return;
        }


        // Set the current item's gameobject to deactivated
        if (items[selectedItemIndex].itemGameObject.GetComponent<Animator>() != null)
        {
            Animator anim = items[selectedItemIndex].itemGameObject.GetComponent<Animator>();
            anim.CrossFade("PunchIdle", 0f);
            anim.Update(0f);
            anim.Update(0f);
        }
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

    private int getNumeralKeycode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            return 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            return 8;
        } else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            return 9; 
        }
        else

            return -1;
    }

}
