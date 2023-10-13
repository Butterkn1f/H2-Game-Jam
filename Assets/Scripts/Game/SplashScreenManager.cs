using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject Credits;
    [SerializeField] private RectTransform toastTransform;
    [SerializeField] private Image toastImage;

    private Vector3 defaultPos;

    private void Start()
    {
        defaultPos = toastTransform.anchoredPosition;
        toastTransform.anchoredPosition = new Vector3(defaultPos.x, -500, defaultPos.z);
        toastImage.color *= new Vector4(1, 1, 1, 0);

        var sequence = DOTween.Sequence();
        sequence.Append(toastTransform.DOAnchorPosY(defaultPos.y, 0.5f))
            .Join(toastImage.DOFade(1, 0.5f));
    }

    public void OpenCredits()
    {
        Credits.SetActive(true);

        var sequence = DOTween.Sequence();
        int i = 0;
        foreach (Transform child in Credits.transform)
        {
            int sign = (i % 2 == 0) ? -1 : 1;
            child.position = new Vector3(1080 * sign, child.position.y);
            i++;
        }

        foreach (Transform child in Credits.transform)
        {
            sequence.Join(child.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f));
        }

        sequence.Join(Credits.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
    }

    public void CloseCredits()
    {
        Credits.GetComponent<CanvasGroup>().DOFade(0, 0.5f)
            .OnComplete(() => Credits.SetActive(false));
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
