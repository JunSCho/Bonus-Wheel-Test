using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimButton : MonoBehaviour
{
    // Calls the destroy function on the cloned instance of the prize image.
    public void ClaimPrize()
    {
        if (transform.parent.childCount > 3)
            transform.parent.GetChild(3).GetComponent<PrizeClaim>().DestroySelf();
    }
}
