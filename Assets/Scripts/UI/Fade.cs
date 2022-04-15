using System.Threading.Tasks;
using UnityEngine;

public class Fade : MonoBehaviour, IGameObserver
{
    [SerializeField] float fadeSpeed;
    [SerializeField] private CanvasGroup fade;

    public bool isFadeOut => fade.alpha == 1;

    public void Awake()
    {
        FadeOut(false);
        GameManager.gm.Subscribe(this);
    }

    public void GameUpdated(GameInfoExtension gameInfo) => FadeOut(gameInfo.isFade);

    public async void FadeOut(bool isFadeOut)
    {
        fade.gameObject.SetActive(true);

        while (true) {
            if (isFadeOut) {
                fade.alpha = Mathf.MoveTowards(fade.alpha, 1, fadeSpeed * Time.deltaTime);
                if (fade.alpha == 1) break;
            }
            else {
                fade.alpha = Mathf.MoveTowards(fade.alpha, 0, fadeSpeed * Time.deltaTime);
                if (fade.alpha == 0) {
                    fade.gameObject.SetActive(false);
                    break;
                }
            }

            try { await Task.Delay(1, GameManager.tokenSource.Token); }
            catch (TaskCanceledException) { return; }
        }
    }
}
