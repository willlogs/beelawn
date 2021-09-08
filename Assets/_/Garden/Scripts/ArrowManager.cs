using PT.Bees;
using PT.Garden;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private Arrow[] _pointers;
    [SerializeField] private FlowerManager[] _flowerFields;
    [SerializeField] private Transform _baseT;
    [SerializeField] private Bee _mainBee;

    private bool _sLevel = false;

    void Start()
    {
        _flowerFields = FindObjectsOfType<FlowerManager>().ToArray();
        _baseT = FindObjectOfType<HoneyStorage>().transform;
        _sLevel = _baseT != null;
        //_pointers = GetComponentsInChildren<Arrow>().ToArray();
    }

    private void Update()
    {
        if (_sLevel)
        {
            int idx = 0;
            foreach (FlowerManager field in _flowerFields)
            {
                Vector3 center = field.GetComponent<Collider>().bounds.center;
                Vector3 diff = center - _mainBee.transform.position;
                if (diff.magnitude > 10)
                {
                    _pointers[idx].gameObject.SetActive(true);
                    _pointers[idx].transform.position = new Vector3(
                        diff.normalized.x,
                        diff.normalized.z,
                        0
                    ) * Screen.width / 3 + new Vector3(
                        Screen.width / 2,
                        Screen.height / 2
                    );

                    diff.y = diff.z;
                    diff.z = 0;
                    _pointers[idx].transform.right = diff;
                }
                else
                {
                    _pointers[idx].gameObject.SetActive(false);
                }
                idx++;
            }

            Transform lastArrow = _pointers[_pointers.Length - 1].transform;
            Vector3 diff1 = _baseT.position - _mainBee.transform.position;
            if (diff1.magnitude > 10)
            {
                lastArrow.gameObject.SetActive(true);
                lastArrow.transform.position = new Vector3(
                        diff1.normalized.x,
                        diff1.normalized.z,
                        0
                    ) * Screen.width / 3 + new Vector3(
                        Screen.width / 2,
                        Screen.height / 2
                    );

                diff1.y = diff1.z;
                diff1.z = 0;
                lastArrow.transform.right = diff1;
            }
            else
            {
                lastArrow.gameObject.SetActive(false);
            }
        }
    }
}
