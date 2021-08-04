using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Prize
{
    public string prizeName;
    public GameObject prizePrefab;
    public float dropWeight;
}

public class WheelSpin : MonoBehaviour
{
    public List<Prize> prizeList;
    public float placementRadius = 350;

    public int minRotations = 1;
    public int maxRotations = 2;
    public float spinDuration = 5;

    private float dropWeightSum = 0;
    private bool spinning = false;

    public GameObject playButton;
    public GameObject claimButton;

    // Start is called before the first frame update
    void Start()
    {
        InitializePrizeList();
        TestPrizeRNG(1000);
    }

    // Generates images of the prizes in a circle on the wheel depending on the prize list. Also calculates the sum of drop weights.
    private void InitializePrizeList()
    {
        for (int i = 0; i < prizeList.Count; i++)
        {
            float angle = -(2 * i + 1) * Mathf.PI / prizeList.Count + Mathf.PI / 2;
            Vector3 newPosition = new Vector3(transform.position.x + placementRadius * Mathf.Cos(angle), transform.position.y + placementRadius * Mathf.Sin(angle), 0);
            Quaternion newRotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI - 90);
            Instantiate(prizeList[i].prizePrefab, newPosition, newRotation, transform);
            dropWeightSum += prizeList[i].dropWeight;
        }
    }

    // Randomly selects a prize from the wheel using the given drop weights.
    private int SelectPrize()
    {
        float selection = Random.Range(0f, dropWeightSum);
        for (int i = 0; i < prizeList.Count; i++)
        {
            if (selection <= prizeList[i].dropWeight)
                return i;
            selection -= prizeList[i].dropWeight;
        }
        return prizeList.Count - 1;
    }

    // Emulates a set number of spins, then outputs the results to console.
    private void TestPrizeRNG(int numSpins)
    {
        int[] pickedPrizeCounts = new int[prizeList.Count];
        for (int i = 0; i < numSpins; i++)
        {
            pickedPrizeCounts[SelectPrize()]++;
        }
        Debug.Log("Out of " + numSpins + " spins, the following prizes were picked this many times:");
        for (int i = 0; i < prizeList.Count; i++)
        {
            Debug.Log(prizeList[i].prizeName + ": " + pickedPrizeCounts[i]);
        }
    }

    // Returns a random angle (in degrees) for the wheel to travel that will land on the input sector.
    private float CalculateTargetAngle(int sector)
    {
        int sectorInverse = prizeList.Count - sector - 1;
        float rotations = Random.Range(minRotations, maxRotations);
        rotations += Random.Range(sectorInverse+0.1f, sectorInverse+0.9f) / prizeList.Count;
        return -360f * rotations;
    }

    // Public accessible function that can be called to spin the wheel if it is not already spinning.
    public void SpinWheel()
    {
        if (!spinning)
        {
            int prizeNumber = SelectPrize();
            float targetAngle = CalculateTargetAngle(prizeNumber);
            Debug.Log(prizeList[prizeNumber].prizeName);
            StartCoroutine(PlayWheelAnimation(targetAngle, prizeNumber));
        }
    }

    // Starts the animation sequence for the prize wheel.
    private IEnumerator PlayWheelAnimation(float targetAngle, int prizeNumber)
    {
        spinning = true;
        playButton.SetActive(false);
        float initialAngle = transform.rotation.eulerAngles.z;
        yield return StartCoroutine(WheelSpinAnimation(initialAngle, initialAngle+10, 0.2f));
        initialAngle = transform.rotation.eulerAngles.z;
        yield return StartCoroutine(WheelSpinAnimation(initialAngle, targetAngle, spinDuration));
        spinning = false;
        InstantiateSelectedPrize(prizeNumber);
    }

    // Spins the wheel to the a target angle within a set duration.
    private IEnumerator WheelSpinAnimation(float initialAngle, float targetAngle, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            float nextAngle = Mathf.Lerp(initialAngle, targetAngle, Mathf.Sqrt(timer/duration));
            transform.rotation = Quaternion.Euler(0, 0, nextAngle);
        }
    }

    // Creates a new instance of the prize icon to be visible when the wheel is hidden.
    private void InstantiateSelectedPrize(int prizeNumber)
    {
        GameObject selectedPrize = Instantiate(prizeList[prizeNumber].prizePrefab, transform.GetChild(prizeNumber).position, transform.GetChild(prizeNumber).rotation, transform.parent.parent);
        selectedPrize.GetComponent<PrizeClaim>().ClaimPrizeAnimation();
        claimButton.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
