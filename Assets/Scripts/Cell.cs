using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    public enum CellType { Floor, Wall, Obstacle, Charge }
    
    [SerializeField] private Color[] colorsByType;
    
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgRobot;
    [SerializeField] private TextMeshProUGUI tmpPosition;
    [SerializeField] private TextMeshProUGUI tmpTimesPassed;
    [SerializeField] private Button btnChangeType;

    public int X;
    public int Y;
    private int gCost;
    private int hCost;
    private int fCost;
    private bool isRobot;
    private int timesPassed;
    private int timesPassedShown = 0;
    public Cell CameFromCell;


    private CellType type = CellType.Floor;

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
            case NotiEditCells.State.Rob:{
                RobotManager.Instance.SetRobotTo(X,Y);
                break;
            }
        }        
    }

    private void changeCellType()
    {
        var typesCount = Enum.GetNames(typeof(CellType)).Length;
        int nextType = (int)type + 1;
        if (nextType == typesCount) nextType = 0;

        type = (CellType)Enum.GetValues(typeof(CellType)).GetValue(nextType);

        UpdateCell();
    }

    public void SetRobot(bool isRobot) {
        this.isRobot = isRobot;
        if (isRobot) timesPassedShown++;
        UpdateCell();
    }
    
    public void UpdateCell()
    {
        imgBackground.color = colorsByType[(int)type];
        imgRobot.enabled = isRobot;
        tmpPosition.text = ToString();
        tmpTimesPassed.text = timesPassedShown.ToString();
    }

    public void IncreaseTimesPassed() {
        timesPassed ++;
    }

    public int GetTimesPassed() {
        return timesPassed;
    }

    public CellType GetCellType() {
        return type;
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
        return type == CellType.Floor || type == CellType.Charge;
    }

    public void ResetTimesPassed()
    {
        timesPassed = 0;
        timesPassedShown = 0;
    }

    public void ResetGraphic()
    {
        isRobot = false;
        UpdateCell();
    }
}
