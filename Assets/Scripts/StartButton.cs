using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Material redColor;
    [SerializeField] private Material greenColor;

    [SerializeField] private Renderer buttonRenderer;

    [SerializeField] private Dummy dummyPrefab;
    private Dummy dummy;

    public int dummyAmount;
    private int dummyCount;

    private bool isStartPractice;

    public bool GetIsStartPractice() { return isStartPractice; }

    [SerializeField] private Transform limitMin;
    [SerializeField] private Transform limitMax;

    private void Start()
    {
        dummy = Instantiate(dummyPrefab);
        dummy.gameObject.SetActive(false);

        buttonRenderer.material = redColor;

        isStartPractice = false;
        dummyCount = 0;
    }

    public void StartPractice()
    {
        isStartPractice = true;
        dummy.gameObject.SetActive(true);

        buttonRenderer.material = greenColor;

        SpawnDummyAtRandomPos();
    }

    private void SpawnDummyAtRandomPos()
    {
        float posX = Random.Range(limitMin.position.x, limitMax.position.x);
        float posZ = Random.Range(limitMax.position.z, limitMin.position.z);
        dummy.transform.position = new Vector3(posX, dummy.transform.position.y, posZ);
    }

    private void Update()
    {
        if (isStartPractice == true)
        {
            if (dummy.GetIsDestroy() == true)
            {
                dummyCount++;
                if (dummyCount >= dummyAmount)
                {
                    dummyCount = 0;
                    isStartPractice = false;
                    buttonRenderer.material = redColor;
                    dummy.ResetDummy();

                    return;
                }

                dummy.ResetDummy();
                SpawnDummyAtRandomPos();
                dummy.gameObject.SetActive(true);
            }
        }
    }
}