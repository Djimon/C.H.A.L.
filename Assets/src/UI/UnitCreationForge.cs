using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreationForge : MonoBehaviour
{
    //TODO: After Creation is submittet, make a screenshot using and save it to Unit.unit3DModelScreenShot
    public UnitScreenshotCreator screenshotCreater;

    // Start is called before the first frame update
    void Start()
    {
        screenshotCreater = GetComponent<UnitScreenshotCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
