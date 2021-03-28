using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiSelectNewImport : MonoBehaviour
{
    [SerializeField] private Button btnCreate;
    [SerializeField] private Button btnImport;
    [SerializeField] private GameObject goCreate;
    [SerializeField] private GameObject goImport;

    void Start()
    {
        btnCreate.onClick.AddListener( delegate { goCreate.SetActive(true); gameObject.SetActive(false); } );        
        btnImport.onClick.AddListener( delegate { goImport.SetActive(true); gameObject.SetActive(false); } );        
    }
}
