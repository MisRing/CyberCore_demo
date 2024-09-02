using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCastUI : MonoBehaviour
{
    public static CardCastUI instance;

    public RectTransform cardCastSlot;

    private void Awake()
    {
        instance = this;
    }
}
