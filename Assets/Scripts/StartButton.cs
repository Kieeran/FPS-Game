using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Material redColor;
    [SerializeField] private Material greenColor;

    [SerializeField] private Renderer buttonRenderer;

    [SerializeField] private Dummy dummy;

    private int dummyAmount;
    private int dummyCount;

    private bool isStartPractice;

    [SerializeField] private Transform limitX;
    [SerializeField] private Transform limitZ;

    private void Start()
    {
        dummy = Instantiate(dummy);
        dummy.gameObject.SetActive(false);

        buttonRenderer.material = redColor;

        isStartPractice = false;

        dummyAmount = 2;
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
        float posX = Random.Range(limitX.position.x, limitZ.position.x);
        float posZ = Random.Range(limitX.position.z, limitZ.position.z);
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
                }

                dummy.ResetDummy();
                SpawnDummyAtRandomPos();
                dummy.gameObject.SetActive(true);
            }
        }
    }
}