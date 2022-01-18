using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        //this scene can only be triggered once
        private bool isTriggered = false;

        //trigger the scene when player gets into this gameobject
        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggered && other.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                isTriggered = true;
            }
        }
    }
}