using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallScript : MonoBehaviour {

    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <- 10)
        {
            transform.position = startingPosition;
        }
    }
}
