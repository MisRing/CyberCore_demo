using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public bool isActivated = false;

    [SerializeField]
    private TMP_Text name_Text;
    [SerializeField]
    private TMP_Text cost_Text;
    [SerializeField]
    private TMP_Text effects_Text;
    [SerializeField]
    private TMP_Text type_Text;

    private Animator animator;

    public bool updateStatus = false;

    public void Awake()
    {
        animator = GetComponent<Animator>();

        //CloseCard(null);
    }

    public void SetStats(string name, string cost, string effect, string type)
    {
        name_Text.text = name;
        cost_Text.text = cost;
        effects_Text.text = effect;
        type_Text.text = type;
    }

    [ContextMenu("OpenCard")]
    public void OpenCard(PointerEventData pointer, bool forced = false)
    {
        if (!updateStatus && !forced)
            return;

        isActivated = true;
        animator.SetTrigger("Open");
    }

    [ContextMenu("Close")]
    public void CloseCard(PointerEventData pointer, bool forced = false)
    {
        if (!updateStatus && !forced)
            return;

        isActivated = false;
        animator.SetTrigger("Close");
    }
}
