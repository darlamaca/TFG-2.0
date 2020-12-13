using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiEditCells : MonoBehaviour
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GridManager gridManager;

    private void Start() 
    {
        btnConfirm.onClick.AddListener(onEndEditing);    
    }

    private void onEndEditing()
    {
        Hide();
        gridManager.DisableCellButtons();
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
