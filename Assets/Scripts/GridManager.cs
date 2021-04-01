using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


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

    public void ImportGrid(string gridJson)
    {
        var cellsData = CellData.FromJson(gridJson);

        GridHeight = cellsData[cellsData.Length - 1].X + 1;
        GridWidth = cellsData[cellsData.Length - 1].Y + 1;
        gridLayoutGroup.constraintCount = GridWidth;

        for (int i = 0; i < cellsData.Length ; i++)
        {
            GameObject newCell = Instantiate(prefabCell);
            newCell.transform.SetParent(this.transform, false);
            newCell.GetComponent<Cell>().Data = cellsData[i];
            newCell.GetComponent<Cell>().SetGCost(int.MaxValue);
            newCell.GetComponent<Cell>().CalculateFCost();
            newCell.GetComponent<Cell>().CameFromCell = null;
            listCell.Add(newCell.GetComponent<Cell>());
        }

        for (int i = 0; i < listCell.Count(); i++)
        {
            listCell[i].UpdateCell();
        }
    }

    public string GetListCellJson()
    {
        return SerializeCells.ToJson(listCell.Select(cell => cell.Data).ToArray());
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
        return listCell.Find(cell => cell.GetCellType() == Cell.CellType.Floor && cell.GetDirtLevel() > cell.GetTimesPassed()) == null;
    }

    public bool RoomCleaned(Cell.RoomType roomtype) {
        return listCell.Find(cell => cell.GetRoomType() == roomtype && cell.GetCellType() == Cell.CellType.Floor && cell.GetDirtLevel() > cell.GetTimesPassed()) == null;
    }

    public Cell GetNextRowCell(int x, int y, Cell.RoomType roomtype) {
        var allRowCell = listCell.FindAll(cell => cell.Y == y && cell.GetDirtLevel() > cell.GetTimesPassed() && cell.GetCellType() == Cell.CellType.Floor && cell.GetRoomType() == roomtype);
        
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

    public Cell NearestChargeCell(Cell currentCell) {
        var allChargeCells = listCell.FindAll(cell => cell.GetCellType() == Cell.CellType.Charge);
        Cell nearestChargeCell = allChargeCells[0];
        for (int i = 1; i < allChargeCells.Count; i++) {
            int xDistancei = Mathf.Abs(currentCell.X - allChargeCells[i].X);
            int yDistancei = Mathf.Abs(currentCell.Y - allChargeCells[i].Y);
            int distanciai = xDistancei + yDistancei;
            int xDistancen = Mathf.Abs(currentCell.X - nearestChargeCell.X);
            int yDistancen = Mathf.Abs(currentCell.Y - nearestChargeCell.Y);
            int distancian = xDistancen + yDistancen;
            if (distanciai < distancian) {
                nearestChargeCell = allChargeCells[i];
            }
        }
        return nearestChargeCell;
    }

    public Cell StartCell(Cell.RoomType roomtype) {
        var celesRoom = listCell.FindAll(cell => cell.GetRoomType() == roomtype && cell.GetCellType() == Cell.CellType.Floor);
        Cell startCell = celesRoom[0];
        for (int i = 1; i < celesRoom.Count; i++) {
            if(celesRoom[i].X <= startCell.X && celesRoom[i].Y <= startCell.Y) {
                startCell = celesRoom[i];
            }
        }
        return startCell;
    }

    public int GetRoomCount()
    {
        return listCell.Select(cell => cell.GetRoomType()).Distinct().Count();
    }
}
