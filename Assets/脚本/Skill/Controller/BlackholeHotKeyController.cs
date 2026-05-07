using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackholeHotKeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private BlackHoleController blackHole;
    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, BlackHoleController _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
       myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackHole;

        myHotKey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);
            

             myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
