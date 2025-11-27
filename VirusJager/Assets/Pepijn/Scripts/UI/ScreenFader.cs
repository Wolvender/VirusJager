using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;

    public IEnumerator FadeToBlack(float duration)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0, 1, t / duration);
            fadeImage.color = c;
            yield return null;
        }
    }
}
