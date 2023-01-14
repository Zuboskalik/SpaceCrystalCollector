using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEngineController : MonoBehaviour
{
    public Sprite[] spriteList;
    [HideInInspector] public int spriteListIndex = 0;
    float engineTimer = 0;
    float engineTimerMultiplier = 160;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        engineTimer += engineTimerMultiplier * Time.deltaTime;

        if (engineTimer > 100)
        {
            engineTimer = engineTimer - 100;
            spriteListIndex = spriteListIndex > 0 ? 0 : 1;
            this.GetComponent<SpriteRenderer>().sprite = spriteList[spriteListIndex];
        }

    }
}
