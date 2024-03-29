﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NotiRobotControl : MonoBehaviour
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnRefresh;
    [SerializeField] private NotiEditCells notiEditCells;

    private void Start() {
        notiEditCells.OnUpdateState += (state) => {onUpdateState(state);};
        btnPlay.onClick.AddListener(onClickPlay);
        btnPause.onClick.AddListener(onClickPause);
        btnRefresh.onClick.AddListener(onClickRefresh);
    }

    private void onClickPlay() {
        GridManager.Instance.Reset();
        Debug.Log(GridManager.Instance.GetListCellJson());
        RobotManager.Instance.Clean();
    }

    private void onClickPause() {
        Debug.Log("Pause");
    }

    private void onClickRefresh() {
        GridManager.Instance.Reset();
        RobotManager.Instance.Reset();
    }

    private void onUpdateState(NotiEditCells.State state) {
        GetComponent<Canvas>().enabled = state == NotiEditCells.State.Fix;
    }
}