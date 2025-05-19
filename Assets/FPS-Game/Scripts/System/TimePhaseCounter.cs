using System.Collections;
using UnityEngine;

public class TimePhaseCounter : MonoBehaviour
{
    public float firstPhaseDuration = 5f;  // x giây
    public float secondPhaseDuration = 3f; // y giây
    public float thirdPhaseDuration = 2f;  // z giây

    private void Start()
    {
        StartCoroutine(PhaseCountdown());
    }

    private IEnumerator PhaseCountdown()
    {
        Debug.Log("Phase 1 started");
        yield return new WaitForSeconds(firstPhaseDuration);

        Debug.Log("Phase 2 started");
        yield return new WaitForSeconds(secondPhaseDuration);

        Debug.Log("Phase 3 started");
        yield return new WaitForSeconds(thirdPhaseDuration);

        Debug.Log("All phases finished");
    }
}