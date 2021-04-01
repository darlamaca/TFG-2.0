using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class Cell : MonoBehaviour
{    
    public enum CellType { Floor, Wall, Obstacle, Charge }
    public enum RoomType { A, B, C }

    [SerializeField] private Color[] colorsByType;
    [SerializeField] private Color[] colorsByDirt;

    
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgRobot;
    [SerializeField] private TextMeshProUGUI tmpPosition;
    [SerializeField] private TextMeshProUGUI tmpRoomType;
    [SerializeField] private TextMeshProUGUI tmpTimesPassed;
    [SerializeField] private TextMeshProUGUI tmpBattery;
    [SerializeField] private Button btnChangeType;

    public CellData Data = new CellData();
    public int X { get { return Data.X; } set { Data.X = value; } }
    public int Y { get { return Data.Y; } set { Data.Y = value; } }
    private int gCost;
    private int hCost;
    private int fCost;
    private int timesPassed;
    private int timesPassedShown = 0;
    public Cell CameFromCell;

    private void Start()
    {
        UpdateCell();
        btnChangeType.onClick.AddListener(onClickCell);
    }

    private void onClickCell() {
        var state = GridManager.Instance.GetEditState();
        switch (state) {
            case NotiEditCells.State.Edit:{
                changeCellType();
                break;
            }
            case NotiEditCells.State.Room:{
                changeRoomType();
                break;
            }
            case NotiEditCells.State.Dirt:{
                changeDirtLevel();
                break;
            }
            case NotiEditCells.State.Rob:{
                RobotManager.Instance.ResetBattery();
                RobotManager.Instance.SetRobotTo(X,Y);
                break;
            }
        }        
    }

    private void changeCellType()
    {
        var typesCount = Enum.GetNames(typeof(CellType)).Length;
        int nextType = (int)Data.type + 1;
        if (nextType == typesCount) nextType = 0;

        Data.type = (CellType)Enum.GetValues(typeof(CellType)).GetValue(nextType);

        UpdateCell();
    }

    private void changeRoomType()
    {
        var roomsCount = Enum.GetNames(typeof(RoomType)).Length;
        int nextRoom = (int)Data.room + 1;
        if (nextRoom == roomsCount) nextRoom = 0;

        Data.room = (RoomType)Enum.GetValues(typeof(RoomType)).GetValue(nextRoom);

        UpdateCell();
    }

    private void changeDirtLevel() {

        int nextDirtLevel = Data.dirtLevel + 1;
        if (nextDirtLevel == 3) nextDirtLevel = 0;

        Data.dirtLevel = nextDirtLevel;
        UpdateCell();
    }

    public void SetRobot(bool isRobot) {
        Data.isRobot = isRobot;
        if (isRobot) {
            timesPassedShown++;
            if(Data.type == CellType.Charge) RobotManager.Instance.batteryshown = RobotManager.MAX_BATTERY;
            else RobotManager.Instance.batteryshown --;
        }
        UpdateCell();
    }
    
    public void UpdateCell()
    {
        imgRobot.enabled = Data.isRobot;
        tmpBattery.enabled = Data.isRobot;
        tmpBattery.text = RobotManager.Instance.batteryshown.ToString();
        tmpPosition.text = this.ToString();
        tmpRoomType.text = Data.room.ToString();
        tmpTimesPassed.text = timesPassedShown.ToString();
        if(Data.type == CellType.Floor) imgBackground.color = colorsByDirt[Math.Max(Data.dirtLevel - timesPassedShown, 0)];
        else imgBackground.color = colorsByType[(int)Data.type];
    }

    public void IncreaseTimesPassed() {
        timesPassed ++;
    }

    public int GetTimesPassed() {
        return timesPassed;
    }

    public int GetDirtLevel() {
        return Data.dirtLevel;
    }

    public CellType GetCellType() {
        return Data.type;
    }

    public RoomType GetRoomType() {
        return Data.room;
    }

    public override string ToString()
    {
        return "[" + X + ", " + Y + "]";
    }

    public void SetGCost(int g) {
        gCost = g;
    }

    public int GetGCost() {
        return gCost;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetHCost(int h) {
        hCost = h;
    }

    public int GetFCost() {
        return fCost;
    }

    public bool IsWalkable() {
        return Data.type == CellType.Floor || Data.type == CellType.Charge;
    }

    public void ResetTimesPassed()
    {
        timesPassed = 0;
        timesPassedShown = 0;
    }

    public void ResetGraphic()
    {
        Data.isRobot = false;
        UpdateCell();
    }
}
