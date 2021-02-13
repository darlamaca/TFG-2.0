using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance = null;
    public int MaxGridValue;
    public int MinGridValue;
    private List<Cell> listCell = new List<Cell>();
    public int GridWidth;
    public int GridHeight;


    [SerializeField] private GameObject prefabCell;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private NotiEditCells notiEditCells;

    private void Awake() {
        if (Instance == null) Instance = this;
    }
    
    public void CreateGrid(int x, int y)
    {
        GridHeight = y;
        GridWidth = x;
        gridLayoutGroup.constraintCount = x;

        var totalGridCells = x * y;

        for (int i = 0; i < totalGridCells; i++)
        {
            GameObject newCell = Instantiate(prefabCell);
            newCell.transform.SetParent(this.transform, false);
            newCell.GetComponent<Cell>().X = i/x;
            newCell.GetComponent<Cell>().Y = i%x;
            newCell.GetComponent<Cell>().SetGCost(int.MaxValue);
            newCell.GetComponent<Cell>().CalculateFCost();
            newCell.GetComponent<Cell>().CameFromCell = null;
            listCell.Add(newCell.GetComponent<Cell>());
        }
    }

    public void ToggleButtons(bool isEdit)
    {
        var childCount = transform.childCount;
        for (int i = 0; i< childCount; i++)
        {
            transform.GetChild(i).GetComponent<Button>().interactable = isEdit;
        }
    }

    public Cell GetCell(int x, int y) {
        return listCell.Find(cell => cell.X == x && cell.Y == y);
    }

    public NotiEditCells.State GetEditState() {
        return notiEditCells.GetState();
    }

    public bool FinishedCleaning() {
        return listCell.Find(cell => cell.GetCellType() == Cell.CellType.Floor && cell.GetTimesPassed() == 0) == null;
    }

    public Cell GetNextRowCell(int x, int y) {
        var allRowCell = listCell.FindAll(cell => cell.Y == y && cell.GetTimesPassed() == 0 && cell.IsWalkable());
        
        if(allRowCell != null && allRowCell.Count > 0) {
            var cellsCount = allRowCell.Count;
            if(Math.Abs(allRowCell[0].X - x) < Math.Abs(allRowCell[cellsCount - 1].X - x)) {
                return allRowCell[0];
            }
            else {
                return allRowCell[cellsCount - 1];
            }
        }
        return null;
    }

    public List<Cell> GetRowCells(int row) {
        return listCell.FindAll(cell => cell.Y == row);
    }

    public void ResetCosts() {
        var listCount = listCell.Count;
        for (int i = 0; i < listCount; i++) {
            listCell[i].SetGCost(int.MaxValue);
            listCell[i].CalculateFCost();
            listCell[i].CameFromCell = null;
        }
    }

    public void Reset()
    {
        var listCount = listCell.Count;
        for (int i = 0; i < listCount; i++) {
            listCell[i].SetGCost(int.MaxValue);
            listCell[i].CalculateFCost();
            listCell[i].CameFromCell = null;
            listCell[i].ResetTimesPassed();
            listCell[i].ResetGraphic();            
        }
    }
}
