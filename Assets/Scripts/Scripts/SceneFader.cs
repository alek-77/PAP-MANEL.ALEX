// SceneFader.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    // Singleton simples para outros scripts conseguirem chamar o fade.
    public static SceneFader Instance;
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    void Awake() => Instance = this;

    public IEnumerator FadeOut() // Escurece
    {
        float t = 0;
        while (t < fadeDuration)
        {
            // Aumenta gradualmente o alfa ate a imagem ficar preta.
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }
    }

    public IEnumerator FadeIn() // Clareia
    {
        float t = fadeDuration;
        while (t > 0)
        {
            // Diminui gradualmente o alfa para revelar a cena.
            t -= Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }
    }

    public void LoadScene(string sceneName)
    {
        // Inicia a rotina que faz fade out antes de trocar de cena.
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        // A nova cena e carregada apenas quando o ecra ja esta escuro.
        SceneManager.LoadScene(sceneName);
    }
}
