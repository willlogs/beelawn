using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PT.Bees
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

        [SerializeField] private float _delay = 0.2f, _tolerance = 0.2f, _randomFactor = 0.2f;
        [SerializeField] private Transform _beeBodyT;
        private Bee _parent;
        private BeeChain _chain;
        private bool _hasParent, _hasChain, _follow, _outtafollow;
        private Tweener _randomTweener;
        private Vector3 _randomDir;

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
                // --------------------- doing a random local move --------------------- //
                Vector3 random = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

                _randomDir = (1 - _randomFactor) * _randomDir + _randomFactor * random;

                _beeBodyT.position = Vector3.Lerp(
                    _beeBodyT.position,
                    _beeBodyT.position + _randomDir,
                    Time.deltaTime
                );
                // --------------------- doing a random local move --------------------- //

                // --------------------- follow the parent --------------------- //
                if (_hasParent)
                {
                    transform.forward = Vector3.Lerp(
                        transform.forward,
                        (_parent.position - transform.position).normalized,
                        Time.deltaTime * 10
                    );

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
                        (transform.position - _chain.center).magnitude <= _tolerance
                    )
                )
                {
                    _follow = _outtafollow;
                    OnFollowChnaged();
                }
                // --------------------- follow the parent --------------------- //
            }
        }

        private void DoRandomMovement()
        {
            if (_hasParent)
            {
                _beeBodyT.transform.DOLocalMove(Vector3.zero, 0.1f);

                if (!_follow && _hasChain)
                {
                    Vector3 newPos = new Vector3(
                        Random.Range(-_chain.radius, _chain.radius),
                        Random.Range(-_chain.radius, _chain.radius),
                        Random.Range(-_chain.radius, _chain.radius)
                    ) + _chain.center;

                    Vector3 direction = newPos - transform.position;
                    transform.forward = direction.normalized;
                    transform.DOMove(newPos, Random.Range(0.1f, 0.8f)).OnComplete(DoRandomMovement);
                }
            }
        }

        private void KillRandomMovement()
        {
            _randomTweener.Kill();
        }
    }
}