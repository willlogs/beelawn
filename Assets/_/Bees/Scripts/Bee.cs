using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PT.Bee
{
    public class Bee : MonoBehaviour
    {
        public Vector3 position;

        public void SetParent(Bee p, BeeChain c)
        {
            _parent = p;
            _hasParent = true;

            _chain = c;
            _hasChain = true;
        }

        public void SetChain(BeeChain c)
        {
            _chain = c;
            _hasChain = true;
        }

        public void Follow(bool f)
        {
            _outtafollow = f;
            if (f)
            {
                _follow = _outtafollow;
                OnFollowChnaged();
            }
        }

        [SerializeField] private float _delay = 0.2f, _tolerance = 0.2f;
        [SerializeField] private Transform _beeBodyT;
        private Bee _parent;
        private BeeChain _chain;
        private bool _hasParent, _hasChain, _follow, _outtafollow;
        private Tweener _randomTweener;

        private void OnFollowChnaged()
        {
            if (!_follow)
            {
                DoRandomMovement();
            }
            else
            {
                KillRandomMovement();
            }
        }

        private void Update()
        {
            if (_follow)
            {
                Vector3 random = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;
                _beeBodyT.position = Vector3.Lerp(
                    _beeBodyT.position,
                    _beeBodyT.position + random,
                    Time.deltaTime
                );

                if (_hasParent)
                {
                    transform.position = Vector3.Lerp(
                        transform.position,
                        _parent.position,
                        Time.deltaTime / _delay
                    );
                }
                position = transform.position;

                if (
                    !_outtafollow &&
                    (
                        !_hasParent ||
                        (transform.position - _parent.position).magnitude <= _tolerance
                    )
                )
                {
                    _follow = _outtafollow;
                    OnFollowChnaged();
                }
            }
        }

        private void DoRandomMovement()
        {
            _beeBodyT.transform.DOLocalMove(Vector3.zero, 0.1f);

            if (!_follow && _hasChain)
            {
                Vector3 newPos = new Vector3(
                    Random.Range(-_chain.radius, _chain.radius),
                    Random.Range(-_chain.radius, _chain.radius),
                    Random.Range(-_chain.radius, _chain.radius)
                ) + _chain.center;

                transform.DOMove(newPos, Random.Range(0.1f, 0.8f)).OnComplete(DoRandomMovement);
            }
        }

        private void KillRandomMovement()
        {
            _randomTweener.Kill();
        }
    }
}