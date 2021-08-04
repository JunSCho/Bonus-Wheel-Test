using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeClaim : MonoBehaviour
{
    private float moveDuration = 0.5f;
    private float finalScale = 2f;

    // Publicly accessible function to start the moving animation.
    public void ClaimPrizeAnimation()
    {
        StartCoroutine(MoveToCenter());
    }

    // Animation to move the image toward the center of the screen and scale it to appear larger.
    private IEnumerator MoveToCenter()
    {
        float timer = 0f;
        Vector3 initialPosition = transform.localPosition;
        float initialAngle = transform.rotation.eulerAngles.z;
        while (timer < moveDuration)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialPosition, Vector3.zero, timer/moveDuration);
            float nextAngle = Mathf.LerpAngle(initialAngle, 0, timer/moveDuration);
            transform.rotation = Quaternion.Euler(0, 0, nextAngle);
            transform.localScale = Vector3.Lerp(Vector3.one, finalScale * Vector3.one, timer/moveDuration);
        }
    }

    // Publicly accessible function to destroy the game object. Called by the Claim Prize button.
    public void DestroySelf()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
