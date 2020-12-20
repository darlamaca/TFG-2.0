using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotiEditCells : MonoBehaviour
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI tmpBody;

    private const string strEdit = "Fixar mapa";
    private const string strRob = "Fixar robot";
    private const string strFix = "Editar mapa";
    private State state = State.Edit;
    public enum State {Edit, Rob, Fix};

    private void Start() 
    {
        btnConfirm.onClick.AddListener(changeEditing);
        tmpBody.text = strEdit;
    }

    private void changeEditing()
    {
        var statesCount = Enum.GetNames(typeof(State)).Length;
        int nextState = (int)state + 1;
        if (nextState == statesCount) nextState = 0;

        state = (State)Enum.GetValues(typeof(State)).GetValue(nextState);

        updateState();
    }

    private void updateState() {
        GridManager.Instance.ToggleButtons(state == State.Edit || state == State.Rob);
        switch (state) {
            case State.Edit:{
                tmpBody.text = strEdit;
                break;
            }
            case State.Rob:{
                tmpBody.text = strRob;
                break;
            }
            case State.Fix:{
                tmpBody.text = strFix;
                break;
            }
        }
    }

    public State GetState() {
        return state;
    }
    
    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
    }
}
