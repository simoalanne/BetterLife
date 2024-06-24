using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Audio;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for spinning the roulette wheel and the ball.
    /// It also checks the winning number and calls the RouletteBetHandler to check the bets.
    /// </summary> 
    public class RouletteSpinner : MonoBehaviour
    {
        [SerializeField] private Transform rouletteCenter;  // The center of the roulette wheel.
        [SerializeField] private float radius = 2.35f; // The radius from the center of the wheel to the ball.
        private float _speed; // The speed at which the ball is spinning.
        [SerializeField] private float minSpeed = 50f; // The minimum speed the ball can spin at before stopping and snapping to a number.
        [SerializeField] private float deceleration = 5f; // The rate at which the ball slows down.
        [SerializeField] private float pocketRadius = 1.45f; // The radius of the pockets from the center of the wheel.
        [SerializeField] private GameObject _wheel; // The roulette wheel object.
        [SerializeField] private GameObject _bettingTable; // The betting table object.
        [SerializeField] private float _ballMinimumInitialSpeed = 180f; // The minimum initial speed of the ball.
        [SerializeField] private float _ballMaximumInitialSpeed = 360f; // The maximum initial speed of the ball.
        [SerializeField] private float _wheelSpeed = 90f; // The speed at which the wheel spins.
        private SoundEffectPlayer _soundEffectPlayer; // Reference to the SoundEffectPlayer script.
        private int _spinDirection = 1; // The direction in which the wheel spins. 1 is clockwise, -1 is counterclockwise.
        private bool _roundUnderway; // Whether the wheel is spinning or not.
        readonly string[] _rouletteNumbers = new[] // The numbers on the European roulette wheel in order.
        {
        "0", "32", "15", "19", "4", "21", "2", "25", "17", "34", "6", "27", "13", "36", "11", "30", "8", "23", "10", "5", "24", "16", "33", "1", "20", "14", "31", "9", "22", "18", "29", "7", "28", "12", "35", "3", "26"
        };

        [SerializeField] private float currentAngle = 90f; // The current angle of the ball.
        private readonly Vector2[] _pockets = new Vector2[37]; // The positions of the pockets on the wheel.
        private RouletteBetHandler _rouletteBetHandler; // Reference to the RouletteBetHandler script.
        private float _originalRadius; // The original radius of the ball.

        void Awake()
        {
            _originalRadius = radius; // Set the original radius of the ball.
            _soundEffectPlayer = GetComponent<SoundEffectPlayer>(); // Get the SoundEffectPlayer component.
            _rouletteBetHandler = FindObjectOfType<RouletteBetHandler>();

            /* Calculate the position for each pocket on the wheel.
            *  The wheel sprite must be perfectly centered in the scene for this to work correctly. 
            *  for this project the sprite rotation is rotated 2.158 degrees to the right to make the 0 pocket be at the top of the wheel.
            */
            for (int i = 0; i < _pockets.Length; i++)
            {
                float angle = (90 - (i * 360.0f / _pockets.Length)) * Mathf.Deg2Rad; // Calculate the angle of the pocket

                float x = Mathf.Cos(angle) * pocketRadius; // Calculate the x position of the pocket
                float y = Mathf.Sin(angle) * pocketRadius; // Calculate the y position of the pocket
                _pockets[i] = new Vector2(x, y) + (Vector2)rouletteCenter.position; // Set the position of the pocket

                GameObject pocket = new(_rouletteNumbers[i]); // Create a new GameObject for the pocket
                pocket.transform.position = _pockets[i]; // Set the position of the pocket
                pocket.transform.SetParent(_wheel.transform); // Set the parent of the pocket to the wheel

                // If the pocket number is "0", set the ball's parent to this pocket
                if (_rouletteNumbers[i] == "0")
                {
                    transform.SetParent(pocket.transform);
                    transform.position = pocket.transform.position;
                }
            }
        }

        void Update()
        {
            RotateWheel(); // Rotate the wheel every frame
            if (transform.parent != null) // If the ball is on a pocket
            {
                //transform.position = transform.parent.position; // Set the position of the ball to the position of the pocket 
            }
            else // Else if the ball is spinning around the wheel
            {
                currentAngle -= _speed * _spinDirection * Time.deltaTime; // Calculate the new angle of the ball
                currentAngle %= 360; // Keep the angle between 0 and 360 to avoid overflow which won't realistically happen but just in case

                float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius; // Calculate the x position of the ball
                float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius; // Calculate the y position of the ball

                transform.position = new Vector3(x, y, 0) + rouletteCenter.position; // Set the position of the ball
            }
        }

        void RotateWheel()
        {
            if (_roundUnderway)
            {
                _wheel.transform.Rotate(Vector3.forward * _spinDirection, _wheelSpeed * Time.deltaTime);
            }
            else
            {
                _wheel.transform.Rotate(Vector3.forward * _spinDirection, _wheelSpeed / 2 * Time.deltaTime); // Spin slower on idle
            }
        }

        public void SpinTheWheel()
        {
            _spinDirection = Random.Range(0, 2) == 0 ? 1 : -1; // Randomize the direction of the wheel spin
            _bettingTable.GetComponentInParent<CanvasGroup>().blocksRaycasts = false; // Disable the betting table
            radius = _originalRadius; // Reset the radius of the ball
            currentAngle = Random.Range(0, 360); // Randomize the angle of the ball
            _speed = Random.Range(_ballMinimumInitialSpeed, _ballMaximumInitialSpeed); // Reset the speed of the ball
            transform.SetParent(null); // Remove the parent of the ball to make it spin around the wheel
            _roundUnderway = true; //
            StartCoroutine(DecelerateBall()); // Start the coroutine that decelerates the ball
        }

        /// <summary>
        /// Decelerates the ball and moves it towards the center of the wheel before stopping it on a pocket.
        /// </summary>
        IEnumerator DecelerateBall()
        {
            _soundEffectPlayer.PlaySoundEffect(0); // Play the sound of the ball spinning
            while (_speed > minSpeed)
            {
                _speed -= deceleration * Time.deltaTime; // Slow down the ball
                yield return null; // Wait for the next frame
            }

            _speed = 0; // Set the speed to 0 to make sure the ball stops spinning
            while (radius > pocketRadius - 0.05f) // While the ball is not in the center of the wheel
            {
                radius -= 5f * Time.deltaTime; // Move the ball towards the center of the wheel
                yield return null; // Wait for the next frame
            }

            int totalBounces = 3; // Total number of bounces
            float firstBounceAmount = radius / 3; // bounce to 1/3 of the radius
            float secondBounceAmount = firstBounceAmount / 2; // bounce to 1/2 of the first bounce
            float thirdBounceAmount = secondBounceAmount / 2; // bounce to 1/2 of the second bounce
            float initialRadius = radius; // The initial radius of the ball
            float bounceSpeed = 1.5f;

            for (int i = 0; i < totalBounces; i++) // Loop through the total number of bounces
            {
                while (radius < initialRadius + firstBounceAmount) // While the ball is bouncing back
                {
                    radius += bounceSpeed * Time.deltaTime; // Move the ball back
                    yield return null; // Wait for the next frame
                }

                while (radius > initialRadius) // While the ball is bouncing back
                {
                    radius -= 7.5f * Time.deltaTime; // Move the ball back
                    yield return null; // Wait for the next frame
                }

                if (i == 0) // If it's the first bounce
                {
                    firstBounceAmount = secondBounceAmount; // Set the first bounce amount to the second bounce amount
                }
                else if (i == 1) // If it's the second bounce
                {
                    firstBounceAmount = thirdBounceAmount; // Set the first bounce amount to the third bounce amount
                }
            }

            Transform nearestPocket = null; // The nearest pocket to the ball
            float minDistance = float.MaxValue; // The minimum distance to the nearest pocket
            foreach (Transform pocket in _wheel.transform) // Loop through all the pockets on the wheel
            {
                float distance = Vector2.Distance(transform.position, pocket.position); // Calculate the distance between the ball and the pocket
                if (distance < minDistance) // If the distance is less than the minimum distance
                {
                    minDistance = distance; // Set the minimum distance to the distance
                    nearestPocket = pocket; // Set the nearest pocket to the pocket
                }
            }

            if (nearestPocket != null) // If the nearest pocket is not null
            {
                transform.SetParent(nearestPocket); // Set the parent of the ball to the nearest pocket
                transform.position = transform.parent.position; // Set the position of the ball to the position of the pocket
                _rouletteBetHandler.CheckWin(int.Parse(nearestPocket.name)); // Check if the player has won
                _soundEffectPlayer.StopSoundEffect(); // Stop the sound of the ball spinning
                _roundUnderway = false; // Set the round underway to false
                _bettingTable.GetComponentInParent<CanvasGroup>().blocksRaycasts = true; // Enable the betting table
            }
            else
            {
                Debug.LogError("This wasn't supposed to happen. The ball didn't stop on a pocket.");
            }
        }
    }
}
