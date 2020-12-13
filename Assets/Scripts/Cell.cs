using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    private enum CellType { Floor, Wall, Obstacle }
    
    [SerializeField] private int[] weightsByType;
    [SerializeField] private Color[] colorsByType;
    
    [SerializeField] private Image imgBackground;
    [SerializeField] private Button btnChangeType;
    [SerializeField] private Text tType;
    [SerializeField] private Text tWeight;

    private CellType type = CellType.Floor;

    private void Start()
    {
        updateCell();
        btnChangeType.onClick.AddListener(changeCellType);
    }

    private void changeCellType()
    {
        var typesCount = Enum.GetNames(typeof(CellType)).Length;
        int nextType = (int)type + 1;
        if (nextType == typesCount) nextType = 0;

        type = (CellType)Enum.GetValues(typeof(CellType)).GetValue(nextType);

        updateCell();
    }

    private void updateCell()
    {
        tType.text = type.ToString();
        tWeight.text = weightsByType[(int)type].ToString();
        imgBackground.color = colorsByType[(int)type];
    }
}
