using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public static RobotManager Instance = null;
    [SerializeField] private int battery;
    private Cell currentCell;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void SetRobotTo(int x, int y) {
        if (currentCell != null) currentCell.SetRobot(false);
        currentCell = GridManager.Instance.GetCell(x,y);
        currentCell.SetRobot(true);
    }
}
