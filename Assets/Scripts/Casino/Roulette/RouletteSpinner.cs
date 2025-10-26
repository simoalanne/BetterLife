using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Types;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Casino.Roulette
{
    [DefaultExecutionOrder(-1)]
    public class RouletteSpinner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform wheel;
        [SerializeField] private Transform ball;

        [Header("Radii")]
        [SerializeField] private float trackRadius = 4.4f;
        [SerializeField] private float pocketRadius = 1.45f;

        [Header("Customization")]
        [SerializeField] private RandomInRange initialBallSpeed = new(300, 600);
        [SerializeField] private RandomInRange ballStopSpeed = new(50, 100);
        [SerializeField, Tooltip("How much of the speed does the ball lose in percentage when it hits a stopper?")]
        private RandomInRange stopperSpeedLoss = new(0.25f, 0.75f);
        [SerializeField] private float ballSpeedDecay = 30f;
        [SerializeField] private float wheelSpeed = 90f;
        [SerializeField] private float wheelSpeedIdle = 45f;
        [SerializeField] private int bounceCount = 3;
        [SerializeField] private float ballFallSpeed = 4f;
        [SerializeField] private float bounceDecay = 0.5f;
        [SerializeField, Tooltip("Controls random speed disturbances during bounces")]
        private RandomInRange speedDisturbance = new(25f, 75f);

        [Header("Events")]
        [SerializeField] private UnityEvent onBallLaunched;
        [SerializeField] private UnityEvent onBallHitStopper;
        [SerializeField] private UnityEvent onBallLanded;
        public event Action<int> OnRoundComplete;

        private float _currentBallSpeed;
        private float _ballAngle;
        private float _ballRadius;
        private int _wheelSpinDirection = 1;
        private bool _roundUnderway;
        private Dictionary<int, Transform> _pockets;
        private int BallSpinDirection => _wheelSpinDirection * -1;
 
        private void Awake()
        {
            AddPockets();
            ball.SetParent(_pockets[RouletteConstants.RandomRouletteNumber]);
            ball.localPosition = Vector3.zero;
            Services.Register(this);
        }

        private void AddPockets()
        {
            var numbers = RouletteConstants.NumbersInWheel;
            var angleStep = 360f / numbers.Count;

            _pockets = numbers
                .Select((number, index) => (number, angle: (index * angleStep - 90f) * -1))
                .ToDictionary(pair => pair.number, pair =>
                {
                    var pocketObj = new GameObject(pair.number.ToString());
                    var angleRad = pair.angle * Mathf.Deg2Rad;
                    var pos = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * pocketRadius;
                    var worldPos = wheel.position + new Vector3(pos.x, pos.y, 0f);
                    pocketObj.transform.position = worldPos;
                    pocketObj.transform.SetParent(wheel.transform);
                    return pocketObj.transform;
                });
        }


        private void Update()
        {
            RotateWheel();

            if (ball.parent is not null) return;
            var angleStep = BallSpinDirection * _currentBallSpeed * Time.deltaTime;
            _ballAngle += angleStep;
            var angleRad = _ballAngle * Mathf.Deg2Rad;
            var newPos = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * _ballRadius;
            ball.position = wheel.position + newPos;
        }

        private void RotateWheel()
        {
            wheel.Rotate(Vector3.forward *
                         (_wheelSpinDirection * (_roundUnderway ? wheelSpeed : wheelSpeedIdle) * Time.deltaTime));
        }

        public void StartRound()
        {
            if (_roundUnderway) return;
            _roundUnderway = true;
            ball.SetParent(null);
            _currentBallSpeed = initialBallSpeed;
            _ballRadius = trackRadius;
            _ballAngle = Random.Range(0f, 360f);
            _wheelSpinDirection = Random.value < 0.5f ? -1 : 1;
            onBallLaunched?.Invoke();
            StartCoroutine(DecelerateBall());
        }

        private IEnumerator DecelerateBall()
        {
            ballStopSpeed.Reset();
            // Step 1: decelerate the ball
            while (_currentBallSpeed > ballStopSpeed.CachedValue)
            {
                _currentBallSpeed -= ballSpeedDecay * Time.deltaTime;
                yield return null;
            }

            // Step 2: wait until ball "hits" a stopper (stoppers are every 45 degrees)
            while (Mathf.RoundToInt(_ballAngle) % 45 is not 0)
                yield return null;

            onBallHitStopper?.Invoke();
            _currentBallSpeed *= 1 - stopperSpeedLoss;

            // Step 3: initial fall to pocket
            var initialRadius = _ballRadius;
            var trackPocketDistance = initialRadius - pocketRadius;
            var fallTime = trackPocketDistance / ballFallSpeed;
            yield return FunctionLibrary.DoOverTime(fallTime, t =>
                _ballRadius = Mathf.Lerp(initialRadius, pocketRadius, t));

            // Step 4: bounces and speed disturbances
            for (var i = 0; i < bounceCount; i++)
            {
                var bounceHeight = trackPocketDistance * Mathf.Pow(1 - bounceDecay, i + 1);
                var bounceTime = bounceHeight / ballFallSpeed;
                
                var initialDisturbance = speedDisturbance * (bounceHeight / trackPocketDistance);
                var perturbDirection = Random.value < 0.5f ? 1 : -1;

                var startSpeed = _currentBallSpeed;
                yield return FunctionLibrary.DoOverTime(bounceTime, t =>
                {
                    var ping = Mathf.PingPong(t * 2, 1);
                    _ballRadius = Mathf.Lerp(pocketRadius, pocketRadius + bounceHeight, Mathf.SmoothStep(0, 1, ping));

                    // Add a small random disturbance to ball speed during bounce. this makes it look more natural maybe?
                    var disturbance = Mathf.Lerp(initialDisturbance * perturbDirection, 0f, t);
                    _currentBallSpeed = Mathf.Max(startSpeed - ballSpeedDecay * Time.deltaTime, 0f) + disturbance;
                });
            }

            // Step 5: snap to nearest pocket
            var closestPocket = _pockets.Values.Aggregate((closest, next) =>
                Vector3.Distance(ball.position, closest.position) <
                Vector3.Distance(ball.position, next.position)
                    ? closest
                    : next);
            ball.SetParent(closestPocket);
            ball.localPosition = Vector3.zero;
            onBallLanded?.Invoke();

            _roundUnderway = false;
            var winningNumber = _pockets.First(pair => pair.Value == closestPocket).Key;
            OnRoundComplete?.Invoke(winningNumber);
        }
    }
}
