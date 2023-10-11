using System.Collections;
using System.Collections.Generic;
using Common.DesignPatterns;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : Singleton<ChatUI>
{
    [SerializeField] private GameObject _background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetUpChat()
    {
        _background.SetActive(true);
    }

    public void OutroResult()
    {
        // Add the whole chat remove animation
        Sequence outroSeq = DOTween.Sequence();

        outroSeq.Append(_background.GetComponent<Image>().DOFade(0, 1.0f));
        outroSeq.AppendCallback(() => _background.SetActive(false));

    }
}
