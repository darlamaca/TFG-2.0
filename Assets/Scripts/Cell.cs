using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    private enum CellType { Floor, Wall, Obstacle }
    
    [SerializeField] private Color[] colorsByType;
    
    [SerializeField] private Image imgBackground;
    [SerializeField] private Image imgRobot;
    [SerializeField] private Button btnChangeType;
    [SerializeField] private TextMeshProUGUI tmpType;
    [SerializeField] private TextMeshProUGUI tmpGCost;
    [SerializeField] private TextMeshProUGUI tmpHCost;
    [SerializeField] private TextMeshProUGUI tmpFCost;
    public GridManager Parent;

    public int X;
    public int Y;
    private int gCost;
    private int hCost;
    private int fCost;
    private bool isRobot;


    private CellType type = CellType.Floor;

    private void Start()
    {
        UpdateCell();
        btnChangeType.onClick.AddListener(onClickCell);
    }

    private void onClickCell() {
        var state = Parent.GetEditState();
        switch (state) {
            case NotiEditCells.State.Edit:{
                changeCellType();
                break;
            }
            case NotiEditCells.State.Rob:{
                setRobot();
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

    public void SetIsRobot(bool isRobot) {
        this.isRobot = isRobot;
    }
    
    private void setRobot() {
        Parent.NoRobot();
        isRobot = true;
        UpdateCell();
    }

    public void UpdateCell()
    {
        tmpType.text = type.ToString();
        imgBackground.color = colorsByType[(int)type];
        imgRobot.enabled = isRobot;
    }
}
