using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowScript : MonoBehaviour
{
    internal bool wasPickedUp = false;
    [SerializeField] private AudioSource cowSound;



    internal void PickedUp()
    {
        wasPickedUp = true;
        cowSound.Play();
    }
}
