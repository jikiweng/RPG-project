using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;

        private void Start()
        {
            //use delegate to add enable & disable control to the current method
            player = GameObject.FindWithTag("Player");
            //after the gameobject start, disable player control
            GetComponent<PlayableDirector>().played += DisableControl;
            //after the gameobject stop, enable player control
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        //cancel current action & disable player control
        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }
        //enable player control
        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}