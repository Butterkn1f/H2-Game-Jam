using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CircleCollider2D))]
public class LevelSelectNode : MonoBehaviour
{
    [SerializeField] private Color _pressedColor;
    public int _levelIndex;
    [SerializeField] private LevelSelectNodeInfo _nodeInfo;

    private Image _image;
    private LevelData _level;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _level = LevelManager.Instance.GetLevelAtIndex(_levelIndex);
        _nodeInfo.Initialize(_level);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            ToggleShowInfo(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Truck"))
        {
            ToggleShowInfo(false);
        }
    }

    private void ToggleShowInfo(bool show)
    {
        _image.color = show ? _pressedColor : Color.white;

        if (show)
        {
            _nodeInfo.AnimatePopup();
        }
        else
        {
            _nodeInfo.sequence.Kill();
            _nodeInfo.gameObject.SetActive(false);
        }
    }
}
