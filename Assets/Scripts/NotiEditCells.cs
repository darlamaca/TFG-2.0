using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotiEditCells : MonoBehaviour
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TextMeshProUGUI tmpBody;

    private const string strEdit = "Editar mapa";
    private const string strFix = "Fixar mapa";
    private bool isEdit = true;

    private void Start() 
    {
        btnConfirm.onClick.AddListener(changeEditing);
        tmpBody.text = strFix;
    }

    private void changeEditing()
    {
        isEdit = !isEdit;
        gridManager.ToggleButtons(isEdit);
        tmpBody.text = (isEdit)? strFix : strEdit;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
    }
}
