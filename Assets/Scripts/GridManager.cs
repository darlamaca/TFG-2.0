using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int MaxGridValue;
    public int MinGridValue;
    private List<Cell> listCell = new List<Cell>();


    [SerializeField] private GameObject prefabCell;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    public void CreateGrid(int x, int y)
    {

        gridLayoutGroup.constraintCount = x;

        var totalGridCells = x * y;

        for (int i = 0; i < totalGridCells; i++)
        {
            GameObject newCell = Instantiate(prefabCell);
            newCell.transform.SetParent(this.transform, false);
            newCell.GetComponent<Cell>().X = i/x;
            newCell.GetComponent<Cell>().Y = i%x;
            listCell.Add(newCell.GetComponent<Cell>());
        }
    }

    internal void ToggleButtons(bool isEdit)
    {
        var childCount = transform.childCount;
        for (int i = 0; i< childCount; i++)
        {
            transform.GetChild(i).GetComponent<Button>().interactable = isEdit;
        }
    }
}
