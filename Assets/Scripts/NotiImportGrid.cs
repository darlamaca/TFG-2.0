using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotiImportGrid : MonoBehaviour
{
    [SerializeField] private Transform tContainer;

    void Start()
    {
        var loadedGridsCount = ExamplesJson.Escenaris.Length;

        for (int i = 0; i < loadedGridsCount && i < 15; i++)
        {
            var child = tContainer.GetChild(i);
            child.GetComponent<Button>().onClick.AddListener(delegate 
            {  
                GridManager.Instance.ImportGrid(ExamplesJson.Escenaris[child.GetSiblingIndex()]);
                gameObject.SetActive(false);
            });
            child.GetComponentInChildren<TextMeshProUGUI>().text = ExamplesJson.Noms[i];
            child.gameObject.SetActive(true);
        }        
    }
}
