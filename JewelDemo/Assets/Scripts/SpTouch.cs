using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 可操作脚本
public class SpTouch : MonoBehaviour {

    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        GameManager.Instance.enterItem(gameObject);
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        GameManager.Instance.relaseItem();
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        GameManager.Instance.pressItem(gameObject);
    }


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
