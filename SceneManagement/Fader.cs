using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        Coroutine currentAction = null;

        // public void FadeOutImmediate()
        // {
        //     canvasGroup.alpha=1;
        // }

        public Coroutine Fade(float target, float time)
        {
            //stop the current coroutine
            if (currentAction != null)
                StopCoroutine(currentAction);

            currentAction=StartCoroutine(fadeRoutine(target,time));
            return currentAction;
        }

        private IEnumerator fadeRoutine(float target,float time)
        {
            while (!Mathf.Approximately(GetComponent<CanvasGroup>().alpha,target))
            {
                GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(
                    GetComponent<CanvasGroup>().alpha,target,Time.deltaTime / time);
                yield return null;
            }
        }
    }
}