using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance = null;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    [SerializeField] private int battery;
    private Cell currentCell;
    private List<Cell> cellPath = new List<Cell>();
    private int direction = 1;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void Reset()
    {
        currentCell = cellPath[0];
        direction = 1;
        cellPath = new List<Cell>();
        SetRobotTo(currentCell.X, currentCell.Y);
    }

    public void SetRobotTo(int x, int y) {
        if (currentCell != null) currentCell.SetRobot(false);
        currentCell = GridManager.Instance.GetCell(x,y);
        currentCell.SetRobot(true);
        Debug.Log( "Robot Moved to Cell: " + currentCell.ToString() );
    }
 
    public void Clean() {
        addCellsToPath(new List<Cell>() {currentCell});
        Debug.Log( "---- START SEARCH PATH ----" );
        while(!GridManager.Instance.FinishedCleaning()) {
            // Buscar camí
            var nextCells = searchPath();
            addCellsToPath(nextCells);
            currentCell = nextCells[nextCells.Count - 1];
        } 
        StartCoroutine("moveInPath");
    }

    private List<Cell> searchPath() {
        var llista = new List<Cell>();
        var horizontalcell = GridManager.Instance.GetCell(currentCell.X + direction, currentCell.Y);
        if(horizontalcell != null && horizontalcell.GetCellType() != Cell.CellType.Wall && horizontalcell.GetCellType() != Cell.CellType.Obstacle) {
            llista.Add(horizontalcell);
            return llista;
        }

        var nextRowCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y);
        if(nextRowCell != null) {
            llista = searchPathA(currentCell, nextRowCell);
            llista.RemoveAt(0);
            return llista;
        }

        direction *= -1;

        var verticalcell = GridManager.Instance.GetCell(currentCell.X, currentCell.Y + 1);
        if(verticalcell != null && verticalcell.GetCellType() != Cell.CellType.Wall && verticalcell.GetCellType() != Cell.CellType.Obstacle) {
            llista.Add(verticalcell);
            return llista;
        }

        var nextColumnCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y + 1);
        if(nextColumnCell != null) {
            llista = searchPathA(currentCell, nextColumnCell);
            llista.RemoveAt(0);
            return llista;
        }

        Debug.LogError("ERROR: Could not find next cell!!!");
        return llista;
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

    private void addCellsToPath(List<Cell> cells) {
        var count = cells.Count;
        for(int i = 0; i < count; i++){
            var cell = cells[i];
            GridManager.Instance.GetCell(cell.X, cell.Y).IncreaseTimesPassed();
            Debug.Log( "Adding Cell to Path: " + cell.ToString() );
            cellPath.Add(cell);
        }
    }

    private List<Cell> searchPathA(Cell startCell, Cell lastCell) {

        var openList = new List<Cell> {startCell};
        var closedList = new List<Cell>();

        GridManager.Instance.ResetCosts();

        GridManager.Instance.GetCell(startCell.X, startCell.Y).SetGCost(0);
        GridManager.Instance.GetCell(startCell.X, startCell.Y).SetHCost(calculateDistance(startCell, lastCell));
        GridManager.Instance.GetCell(startCell.X, startCell.Y).CalculateFCost();

        while (openList.Count > 0) {
            Cell currentC = getLowestFCostCell(openList);
            if (currentC.X == lastCell.X && currentC.Y == lastCell.Y)  {
                //Reached final node
                return CalculatePath(lastCell);
            }

            openList.Remove(currentC);
            closedList.Add(currentC);

            foreach (Cell neighbourCell in GetNeighbourList(currentC))
            {
                if (closedList.Contains(neighbourCell))
                {
                    Debug.Log( "THIS CELL ALREADY EXISTS IN CLOSED LIST : " + neighbourCell.ToString() );
                    continue;
                }
                if (!neighbourCell.IsWalkable()) {
                    closedList.Add(neighbourCell);
                    continue;
                }

                int tentativeGCost = currentC.GetGCost() + calculateDistance(currentC, neighbourCell);
                if (tentativeGCost < neighbourCell.GetGCost()) {
                    neighbourCell.CameFromCell = currentC;
                    neighbourCell.SetGCost(tentativeGCost);
                    neighbourCell.SetHCost(calculateDistance(neighbourCell, lastCell));
                    neighbourCell.CalculateFCost();

                    if (!openList.Contains(neighbourCell)) {
                        Debug.Log( "ADD TO OPEN LIST : " + neighbourCell.ToString() );
                        openList.Add(neighbourCell);
                    }
                }                
            }
        }

        // Out of nodes on the openList
        Debug.LogError("ERROR: Could not reach " + lastCell.ToString() + " from " + startCell.ToString());
        return null;
    }

    private int calculateDistance(Cell a, Cell b) {
        int xDistance = Mathf.Abs(a.X - b.X);
        int yDistance = Mathf.Abs(a.Y - b.Y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Cell getLowestFCostCell(List<Cell> cellsList) {
        Cell lowestFCostCell = cellsList[0];
        for ( int i = 1; i < cellsList.Count; i++) {
            if (cellsList[i].GetFCost() < lowestFCostCell.GetFCost()) {
                lowestFCostCell = cellsList[i];
            }
        }
        return lowestFCostCell;
    }

    private List<Cell> CalculatePath(Cell endCell) {
        List<Cell> path = new List<Cell>();
        path.Add(endCell);
        Cell currentCell = endCell;
        while (currentCell.CameFromCell != null) {
            path.Add(currentCell.CameFromCell);
            currentCell = currentCell.CameFromCell;
        }
        path.Reverse();
        return path;
    }

    private List<Cell> GetNeighbourList(Cell currentC) {
        List<Cell> neighbourList = new List<Cell>();

        if (currentC.X - 1 >= 0) {
            // Left
            neighbourList.Add(GridManager.Instance.GetCell(currentC.X - 1, currentC.Y));
            // Left Down
            if (currentC.Y - 1 >= 0) neighbourList.Add(GridManager.Instance.GetCell(currentC.X - 1, currentC.Y - 1));
            // Left Up
            if (currentC.Y + 1 < GridManager.Instance.GridHeight) neighbourList.Add(GridManager.Instance.GetCell(currentC.X - 1, currentC.Y + 1));
        }
        if (currentC.X + 1 < GridManager.Instance.GridWidth) {
            // Right
            neighbourList.Add(GridManager.Instance.GetCell(currentC.X + 1, currentC.Y));
            // Right Down
            if (currentC.Y - 1 >= 0) neighbourList.Add(GridManager.Instance.GetCell(currentC.X + 1, currentC.Y - 1));
            // Right Up
            if (currentC.Y + 1 < GridManager.Instance.GridHeight) neighbourList.Add(GridManager.Instance.GetCell(currentC.X + 1, currentC.Y + 1));
        }
        // Down
        if (currentC.Y - 1 >= 0) neighbourList.Add(GridManager.Instance.GetCell(currentC.X, currentC.Y - 1));
        // Up
        if (currentC.Y + 1 < GridManager.Instance.GridHeight) neighbourList.Add(GridManager.Instance.GetCell(currentC.X, currentC.Y + 1));

        return neighbourList;
    }
}
