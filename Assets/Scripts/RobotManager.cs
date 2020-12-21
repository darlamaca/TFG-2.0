using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance = null;
    [SerializeField] private int battery;
    private Cell currentCell;
    private int direction = 1;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void SetRobotTo(int x, int y) {
        if (currentCell != null) currentCell.SetRobot(false);
        currentCell = GridManager.Instance.GetCell(x,y);
        currentCell.SetRobot(true);
        Debug.Log("Current cell is: " + currentCell.X + ", " + currentCell.Y);
    }
 
    public void Clean() {
        if(!GridManager.Instance.FinishedCleaning()) {
            Debug.Log("Cleaning has not finished!");
            // Buscar camí
            var nextCell = searchPath();
            // Moure't
            IEnumerator coroutine = moveToCell(nextCell);
            StartCoroutine(coroutine);  
        } else Debug.Log("Cleaning has finished! Hurray!!!");
    }

    private Cell searchPath() {
        Debug.Log("Lets search next cell");
        var horizontalcell = GridManager.Instance.GetCell(currentCell.X + direction, currentCell.Y);
        if (horizontalcell != null) Debug.Log(String.Format( "Next neighbour horizontal cell: [{0}, {1}] > {2}", horizontalcell.X, horizontalcell.Y, horizontalcell.GetCellType() ) );
        else Debug.Log("Next neighbour horizontal cell is null");
        if(horizontalcell != null && horizontalcell.GetCellType() != Cell.CellType.Wall && horizontalcell.GetCellType() != Cell.CellType.Obstacle) {
            return horizontalcell;
        }

        // TODO: Fix this
        var nextRowCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y);
        if (nextRowCell != null) Debug.Log(String.Format( "Next non-neighbour horizontal cell: [{0}, {1}] > {2}", nextRowCell.X, nextRowCell.Y, nextRowCell.GetCellType() ) );
        else Debug.Log("Next non-neighbour horizontal cell is null");
        if(nextRowCell != null) {
            return nextRowCell;
        }

        direction *= -1;

        var verticalcell = GridManager.Instance.GetCell(currentCell.X, currentCell.Y + 1);
        if (verticalcell != null) Debug.Log(String.Format( "Next neighbour vertical cell: [{0}, {1}] > {2}", verticalcell.X, verticalcell.Y, verticalcell.GetCellType() ) );
        else Debug.Log("Next neighbour vertical cell is null");
        if(verticalcell != null && verticalcell.GetCellType() != Cell.CellType.Wall && verticalcell.GetCellType() != Cell.CellType.Obstacle) {
            return verticalcell;
        }

        var nextColumnCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y + 1);
        if (nextColumnCell != null) Debug.Log(String.Format( "Next non-neighbour vertical cell: [{0}, {1}] > {2}", nextColumnCell.X, nextColumnCell.Y, nextColumnCell.GetCellType() ) );
        else Debug.Log("Next non-neighbour vertical cell is null");
        if(nextColumnCell != null) {
            return nextColumnCell;
        }

        Debug.LogError("ERROR: Could not find next cell!!!");
        return null;
    }

    private IEnumerator moveToCell(Cell nextcell) {
        if(nextcell != null) {
            SetRobotTo(nextcell.X, nextcell.Y);
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }
}
