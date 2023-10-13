using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _bell;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _pauseStuff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBellAnim()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 45), 0.5f));
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -45), 0.5f));
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), 0.5f));
    }
}
