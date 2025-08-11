using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BottleCap : MonoBehaviour
{
    private MeshRenderer _body;
    private GameObject _bottleHead;
    private GameObject _bottle;
    private Transform _bottleHeadParent;

    private Vector3 _bottleHeadInitialPos;
    private Vector3 _bottleHeadInitialLocalPos;

    public void Initialize(GameObject bottle, GameObject bottleHead, GameObject bottleBody)
    {
        _bottleHead = bottleHead;
        _bottle = bottle;
        _body = bottleBody.GetComponent<MeshRenderer>();

        _bottleHeadInitialPos = _bottleHead.transform.position;
        _bottleHeadInitialLocalPos = _bottleHead.transform.localPosition;
        _bottleHeadParent = _bottleHead.transform.parent;
    }

    public void OpenSpecificPos(Vector3 destination)
    {
        _bottleHead.transform.parent = null;
        _bottleHead.transform.DOMove(destination, 1f);
    }

    public void OpenToSide()
    {
        _bottleHead.transform.parent = null;

        Vector3 destination = new Vector3(_bottle.transform.localPosition.x - (_body.bounds.extents.x - _body.bounds.center.x), 
                                            _body.bounds.center.y - _body.bounds.extents.y,
                                            _bottle.transform.position.z + _body.bounds.extents.z - 0.1f);

        _bottleHead.transform.DOMove(destination, 1f);
    }

    public void Close()
    {
        _bottleHead.transform.parent = _bottleHeadParent;
        _bottleHead.transform.DOMove(_bottleHeadInitialPos, 1);

        if (_bottleHead.transform.localPosition != _bottleHeadInitialLocalPos)
            StartCoroutine(BottleHeadTransformHardReset(1f, 0.75f));
    }

    private IEnumerator BottleHeadTransformHardReset(float delay, float moveTime)
    {
        yield return new WaitForSeconds(delay);
        Vector3 currentPos = _bottleHead.transform.localPosition;

        float time = 0;

        while (time < moveTime)
        {
            _bottleHead.transform.localPosition = Vector3.Lerp(currentPos, _bottleHeadInitialLocalPos, time / moveTime);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _bottleHead.transform.localPosition = _bottleHeadInitialLocalPos;
    }
}
