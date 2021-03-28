using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiGridSize : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private InputField ifGridX;
    [SerializeField] private InputField ifGridY;
    [SerializeField] private Text tError;
    [SerializeField] private Button btnCreateGrid;

    private void Start()
    {
        btnCreateGrid.onClick.AddListener(onClickCreateGrid);
    }

    private void onClickCreateGrid()
    {
        int x, y;
        bool xIsNumber = Int32.TryParse(ifGridX.text, out x);
        bool yIsNumber = Int32.TryParse(ifGridY.text, out y);

        if (xIsNumber && yIsNumber && isValid(x) && isValid(y))
        {
            Hide();
            GridManager.Instance.CreateGrid(x, y);
        }
        else
        {
            tError.text = String.Format("ERROR: Values are invalid.\nX and Y must be numbers between [{0}, {1}]", GridManager.Instance.MinGridValue, GridManager.Instance.MaxGridValue);
        }
    }

    private bool isValid(int n)
    {
        return n >= GridManager.Instance.MinGridValue && n <= GridManager.Instance.MaxGridValue;
    }

    private void Hide()
    {
        canvas.enabled = false;
    }

    private void Show()
    {
        canvas.enabled = true;
    }
}
