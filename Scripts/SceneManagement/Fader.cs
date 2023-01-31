using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private Coroutine currentCoroutine = null;
        public void FadeOutImmediately()
        {
            GetComponent<CanvasGroup>().alpha = 1;
        }
        public Coroutine FadeOut(float time)
        {
            return Fade(time, 1);
        }
        public Coroutine FadeIn(float time)
        {
            return Fade(time, 0);
        }
        private Coroutine Fade(float time,float target)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeRoutine(time,target));
            return currentCoroutine;
        }
        private IEnumerator FadeRoutine(float time,float target)
        {
            while (!Mathf.Approximately(GetComponent<CanvasGroup>().alpha,target))
            {
                GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(GetComponent<CanvasGroup>().alpha, target,Time.deltaTime / time) ;
                yield return null;
            }
        }

        public IEnumerator ColorFury()
        {
            while (true)
            {
                Color randomColor = new Color(Random.Range(0.15f, 1), Random.Range(0.15f, 1), Random.Range(0.15f, 1));
                GetComponent<Image>().color = randomColor;
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}