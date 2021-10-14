using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Interactable : MonoBehaviour
{
    public TMP_Text text;

    public abstract string GetName();
    public abstract void Use(PlayerController player);

    void Update()
    {
        if (!DropManager.Instance.lookingAtDrop)
        {
            text.gameObject.SetActive(false);
        } else
        {
            text.gameObject.SetActive(true);
        }
    }

}
