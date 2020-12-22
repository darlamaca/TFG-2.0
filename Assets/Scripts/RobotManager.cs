using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance = null;
    [SerializeField] private int battery;
    private Cell currentCell;
    private List<Cell> cellPath = new List<Cell>();
    private int direction = 1;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void SetRobotTo(int x, int y) {
        if (currentCell != null) currentCell.SetRobot(false);
        currentCell = GridManager.Instance.GetCell(x,y);
        currentCell.SetRobot(true);
        Debug.Log( "Robot Moved to Cell: " + currentCell.ToString() );
    }
 
    public void Clean() {
        addCellToPath(currentCell);
        Debug.Log( "---- START SEARCH PATH ----" );
        while(!GridManager.Instance.FinishedCleaning()) {
            // Buscar camí
            var nextCell = searchPath();
            addCellToPath(nextCell);
            currentCell = nextCell;
        } 
        StartCoroutine("moveInPath");
    }

    private Cell searchPath() {
        var horizontalcell = GridManager.Instance.GetCell(currentCell.X + direction, currentCell.Y);
        if(horizontalcell != null && horizontalcell.GetCellType() != Cell.CellType.Wall && horizontalcell.GetCellType() != Cell.CellType.Obstacle) {
            return horizontalcell;
        }

        var nextRowCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y);
        if(nextRowCell != null) {
            return nextRowCell;
        }

        direction *= -1;

        var verticalcell = GridManager.Instance.GetCell(currentCell.X, currentCell.Y + 1);
        if(verticalcell != null && verticalcell.GetCellType() != Cell.CellType.Wall && verticalcell.GetCellType() != Cell.CellType.Obstacle) {
            return verticalcell;
        }

        var nextColumnCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y + 1);
        if(nextColumnCell != null) {
            return nextColumnCell;
        }

        Debug.LogError("ERROR: Could not find next cell!!!");
        return null;
    }

    private IEnumerator moveInPath() {
        Debug.Log( "---- START MOVE PATH ----" );
        var count = cellPath.Count;
        for(int i = 0; i < count; i++) {
            SetRobotTo(cellPath[i].X, cellPath[i].Y);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private void addCellToPath(Cell cell) {
        GridManager.Instance.GetCell(cell.X, cell.Y).IncreaseTimesPassed();
        Debug.Log( "Adding Cell to Path: " + cell.ToString() );
        cellPath.Add(cell);
    }
}
