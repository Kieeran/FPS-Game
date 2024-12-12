using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour

{

    public GameObject InfoText;

    void Update()

    {

        if (Input.anyKey)

        {

            InfoText.gameObject.SetActive(true);

        }

        else

        {

            InfoText.gameObject.SetActive(false);

        }

    }

}