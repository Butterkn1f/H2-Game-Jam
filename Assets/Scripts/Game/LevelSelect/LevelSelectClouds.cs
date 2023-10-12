using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelSelectClouds : MonoBehaviour
{
    [SerializeField] private List<LevelSelectNode> _nodes;

    private LevelSelectNode _nextNode;

    // Start is called before the first frame update
    void Start()
    {
        SetSpawnLocation();
    }

    private void SetSpawnLocation()
    {
        LevelSelectNode currNode = null;

        int unlockedIndex = LevelManager.Instance.GetLastUnlockedLevelIndex();
        foreach (var node in _nodes)
        {
            if (node._levelIndex == unlockedIndex + 1)
                _nextNode = node;

            if (node._levelIndex == unlockedIndex)
                currNode = node;
        }

        if (!currNode || currNode._levelIndex == 0)
        {
            // For first node, with no previous, just set clouds to the position of the next node
            transform.position = new Vector3(transform.position.x, _nextNode.transform.position.y, transform.position.z);
        }
        else
        {
            // For the other nodes, set clouds to the current node, then animate to the next node
            transform.position = new Vector3(transform.position.x, currNode.transform.position.y, transform.position.z);
            AnimateCloudReveal();
        }
    }

    private void AnimateCloudReveal()
    {
        float destPos;
        if (_nextNode)
        {
            destPos = _nextNode.transform.position.y;
        }
        else
        {
            // Last level, no node
            Vector3[] worldCorners = new Vector3[4];
            transform.parent.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            destPos = worldCorners[1].y;
        }

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f)
            .Append(transform.DOMoveY(destPos, 1f));
    }
}
