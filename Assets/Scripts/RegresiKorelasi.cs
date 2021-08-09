using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RegresiKorelasi : MonoBehaviour
{
    private GameObject[] input_X, input_Y;
    private List<string> data_X = new List<string>();
    private List<string> data_Y = new List<string>();
    
    public void Submit()
    {
        input_X = GameObject.FindGameObjectsWithTag("X");
        input_Y = GameObject.FindGameObjectsWithTag("Y");

        CheckX();
    }

    private void CheckX()
    {
        var i = 0;
        foreach (var X in input_X)
        {
            i++;
            var inputX = X.GetComponent<TMP_InputField>();
            
            // cek kalau ada yg null
            if (inputX.text != "")
            {
                data_X.Add(inputX.text);
            }
            else
            {
                Debug.Log("error, nilai tidak boleh kosong");
                break;
            }
            
            // sampai akhir ga ada yg null
            if (i == input_X.Length)
            {
                CheckY();
            }
        }
    }

    private void CheckY()
    {
        var j = 0;
        foreach (var Y in input_Y)
        {
            j++;
            var inputY = Y.GetComponent<TMP_InputField>();
  
            if (inputY.text != "")
            {
                data_Y.Add(inputY.text);
            }
            else
            {
                Debug.Log("error, nilai tidak boleh kosong");
                break;
            }
    
            if (j == input_Y.Length)
            {
                Regresi();
            }
        }
    }

    void Regresi()
    {
        foreach (string x in data_X)
        {
            Debug.Log("x: "+x);
        }
        foreach (string y in data_Y)
        {
            Debug.Log("y: "+y);
        }
    }
}
