using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance = null;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private int battery;
    public int batteryshown;
    public const int MAX_BATTERY = 20;
    private const int LOW_BATTERY = 4;
    private Cell currentCell;
    private List<Cell> cellPath = new List<Cell>();

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void Reset()
    {
        currentCell = cellPath[0];
        cellPath = new List<Cell>();
        SetRobotTo(currentCell.X, currentCell.Y);
    }

    public void SetRobotTo(int x, int y) {
        if (currentCell != null) currentCell.SetRobot(false);
        currentCell = GridManager.Instance.GetCell(x,y);
        currentCell.SetRobot(true);
        Debug.Log( "Robot Moved to Cell: " + currentCell.ToString() );
    }

    public void ResetBattery() {
        battery = MAX_BATTERY;
        batteryshown = battery +1 ;
    }

    public void Clean() {
        ResetBattery();
        Debug.Log("---- START CLEANING ----");
        var room = currentCell.GetRoomType();
        addCellsToPath(new List<Cell>() {currentCell});
        while(!GridManager.Instance.FinishedCleaning()) {
            Debug.Log("Start cleaning:" + room.ToString());
            // Començar sala
            CleanRoom(room);
            if((int) room < (GridManager.Instance.GetRoomCount() - 1)) {
                room = room + 1;
            }
        } 
        StartCoroutine("moveInPath");
        Debug.Log("---- TOT NET ----");
    }
 
    public void CleanRoom(Cell.RoomType roomtype) {
        
        Debug.Log( "---- START CLEANING ROOM ----" );
        while(!GridManager.Instance.RoomCleaned(roomtype)) {
            // Buscar camí
            if(battery <= LOW_BATTERY) {
                var nearestChargeCell = GridManager.Instance.NearestChargeCell(currentCell);
                var goChargeCell = searchPathA(currentCell, nearestChargeCell);
                goChargeCell.RemoveAt(0);
                addCellsToPath(goChargeCell);
                Debug.Log("Battery after charge: " + battery);                
                var comeBack = searchPathA(nearestChargeCell, currentCell);
                comeBack.RemoveAt(0);
                for(int i = comeBack.Count - 1; i>= 0; i--) {
                    if (comeBack[i].GetTimesPassed() < comeBack[i].GetDirtLevel() && GridManager.Instance.LowerCellsClean(comeBack[i].Y)) {
                        if(i<comeBack.Count - 1) {
                            comeBack.RemoveRange(i + 1, comeBack.Count - i - 1);
                        }
                        break;
                    }
                }
                addCellsToPath(comeBack);
                currentCell = comeBack[comeBack.Count - 1];
            }
            else {
                var nextCells = searchPath(roomtype);
                addCellsToPath(nextCells);
                currentCell = nextCells[nextCells.Count - 1];
            }
            
        }
        if((int) roomtype < (GridManager.Instance.GetRoomCount() - 1)) {
            var nextRoomStartCell = GridManager.Instance.StartCell(roomtype + 1);
            Debug.Log("nextRoomStartCell: " + nextRoomStartCell);
            var nextCellsToNextRoom = searchPathA(currentCell, nextRoomStartCell);
            nextCellsToNextRoom.RemoveAt(0);
            addCellsToPath(nextCellsToNextRoom);
            currentCell = nextCellsToNextRoom[nextCellsToNextRoom.Count - 1];
            Debug.Log("CurrentCell:" + currentCell.ToString());
            Debug.Log("Move to next room: " + (roomtype + 1));
        }
       
    }

    private List<Cell> searchPath(Cell.RoomType roomtype) {
        var direction = searchDirection(currentCell, roomtype);
        Debug.Log( "Direction : " + direction );
        var llista = new List<Cell>();
        if(currentCell.GetDirtLevel() > currentCell.GetTimesPassed()) {
            llista.Add(currentCell);
            Debug.Log("Em quedo netejant la mateixa cela");
            return llista;
        }
        var horizontalcell = GridManager.Instance.GetCell(currentCell.X + direction, currentCell.Y);
        if(horizontalcell != null && horizontalcell.GetCellType() == Cell.CellType.Floor && horizontalcell.GetRoomType() == roomtype && horizontalcell.GetDirtLevel() > horizontalcell.GetTimesPassed()) {
            llista.Add(horizontalcell);
            Debug.Log("Move Horizontal: " + horizontalcell.ToString());
            return llista;
        }

        var nextRowCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y, roomtype);
        if(nextRowCell != null) {
            llista = searchPathA(currentCell, nextRowCell);
            llista.RemoveAt(0);
            Debug.Log("Move Horizontal A*: " + nextRowCell.ToString());
            return llista;
        }

        var verticalcell = GridManager.Instance.GetCell(currentCell.X, currentCell.Y + 1);
        if(verticalcell != null && verticalcell.GetCellType() == Cell.CellType.Floor && verticalcell.GetRoomType() == roomtype && verticalcell.GetDirtLevel() > verticalcell.GetTimesPassed()) {
            llista.Add(verticalcell);
            return llista;
        }

        var nextColumnCell = GridManager.Instance.GetNextRowCell(currentCell.X, currentCell.Y + 1, roomtype);
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
        battery = battery - count;
        Debug.Log("Current battery: " + battery);
        for(int i = 0; i < count; i++){
            var cell = cells[i];
            if(cell.GetCellType() == Cell.CellType.Charge) battery = MAX_BATTERY;
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
        return xDistance + yDistance;
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
        }
        if (currentC.X + 1 < GridManager.Instance.GridWidth) {
            // Right
            neighbourList.Add(GridManager.Instance.GetCell(currentC.X + 1, currentC.Y));
        }
        // Down
        if (currentC.Y - 1 >= 0) neighbourList.Add(GridManager.Instance.GetCell(currentC.X, currentC.Y - 1));
        // Up
        if (currentC.Y + 1 < GridManager.Instance.GridHeight) neighbourList.Add(GridManager.Instance.GetCell(currentC.X, currentC.Y + 1));

        return neighbourList;
    }

    private int searchDirection(Cell celaInici, Cell.RoomType roomtype) {
        var rowCells = GridManager.Instance.GetRowCells(celaInici.Y);
        Debug.Log("Search direction. Row Count : " + rowCells.Count);
        var celesDreta = rowCells.FindAll(cell => cell.X > celaInici.X && cell.GetDirtLevel() > cell.GetTimesPassed() && cell.GetCellType() == Cell.CellType.Floor && cell.GetRoomType() == roomtype);
        Debug.Log("Search direction. Right Count : " + celesDreta.Count);
        var celesEsquerra = rowCells.FindAll(cell => cell.X < celaInici.X && cell.GetDirtLevel() > cell.GetTimesPassed() && cell.GetCellType() == Cell.CellType.Floor && cell.GetRoomType() == roomtype);
        Debug.Log("Search direction. Left Count : " + celesEsquerra.Count);
        
        if (celesEsquerra == null || celesEsquerra.Count == 0) return 1;
        if (celesDreta == null || celesDreta.Count == 0) return -1;
        if (celesEsquerra.Count >= celesDreta.Count) return 1;
        else return -1; 
    }
}
