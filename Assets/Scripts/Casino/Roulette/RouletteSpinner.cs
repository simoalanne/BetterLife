using UnityEngine;
using System.Collections;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for spinning the roulette wheel and the ball.
    /// It also checks the winning number and calls the RouletteBetHandler to check the bets.
    /// </summary> 
    public class RouletteSpinner : MonoBehaviour
    {
        [SerializeField] private Transform rouletteCenter;  // The center of the roulette wheel.
        [SerializeField] private float radius = 2.33f; // The radius from the center of the wheel to the ball.
        private float _speed; // The speed at which the ball is spinning.
        [SerializeField] private float minSpeed = 50f; // The minimum speed the ball can spin at before stopping and snapping to a number.
        [SerializeField] private float deceleration = 5f; // The rate at which the ball slows down.
        [SerializeField] private float slotRadius = 1.85f; // The radius of the "slots" on the wheel where the ball can stop.
        [SerializeField] private GameObject _wheel; // The roulette wheel object.
        [SerializeField] private GameObject _bettingTable; // The betting table object.
        [SerializeField] private float _targetRotation = -90f; // The target rotation of the betting table.
        [SerializeField] private float _bettingTableRotationSpeed = 60f; // The speed at which the betting table rotates.
        private float _initialCameraSize; // The initial size of the camera.
        readonly string[] _rouletteNumbers = new[] // The numbers on the European roulette wheel in order.
        {
        "0", "32", "15", "19", "4", "21", "2", "25", "17", "34", "6", "27", "13", "36", "11", "30", "8", "23", "10", "5", "24", "16", "33", "1", "20", "14", "31", "9", "22", "18", "29", "7", "28", "12", "35", "3", "26"
        };

        private float currentAngle = 90f; // The current angle of the ball.
        private readonly Vector2[] _rouletteNumberSlots = new Vector2[37]; // The positions of the slots on the wheel.
        private bool _isSpinning = false; // Flag to check if the wheel is spinning.
        private RouletteBetHandler _betHandler; // Reference to the RouletteBetHandler script.

        void Awake()
        {
            _initialCameraSize = Camera.main.orthographicSize; // Get the initial size of the camera.
            _betHandler = FindObjectOfType<RouletteBetHandler>();
            _speed = Random.Range(270, 360); // Random speed for the ball to spin at. Needs to tested later on to see if the ball can actually stop on all numbers.
            for (int i = 0; i < _rouletteNumberSlots.Length; i++) // calculate the position of the slots on the wheel
            {
                float angle = (90 - (i * 360.0f / _rouletteNumberSlots.Length)) * Mathf.Deg2Rad; // Calculate the angle of the slot

                float x = Mathf.Cos(angle) * slotRadius; // Calculate the x position of the slot
                float y = Mathf.Sin(angle) * slotRadius; // Calculate the y position of the slot
                _rouletteNumberSlots[i] = new Vector2(x, y) + (Vector2)rouletteCenter.position; // Set the position of the slot

                GameObject slot = new(_rouletteNumbers[i]); // Create a new GameObject for the slot
                slot.transform.position = _rouletteNumberSlots[i]; // Set the position of the slot
                slot.transform.SetParent(_wheel.transform); // Set the parent of the slot to the wheel

                // If the slot number is "0", set the ball's parent to this slot
                if (_rouletteNumbers[i] == "0")
                {
                    transform.SetParent(slot.transform);
                }
            }
        }

        void Update()
        {
            RotateWheel(); // Rotate the wheel every frame
            if (transform.parent != null) // If the ball is on a slot
            {
                transform.position = transform.parent.position; // Set the position of the ball to the position of the slot 
            }
            else // Else if the ball is spinning around the wheel
            {
                currentAngle -= _speed * Time.deltaTime; // Calculate the new angle of the ball
                currentAngle %= 360; // Keep the angle between 0 and 360 to avoid overflow which won't realistically happen but just in case

                float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius; // Calculate the x position of the ball
                float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius; // Calculate the y position of the ball

                transform.position = new Vector3(x, y, 0) + rouletteCenter.position; // Set the position of the ball
            }
        }

        IEnumerator DecelerateBall()
        {
            /* Contains some magic numbers that need to be adjusted later on
            to make the ball stop on the numbers more accurately. */
            while (radius > slotRadius + 0.18f && _speed > minSpeed)
            {
                _speed -= deceleration * Time.deltaTime; // Slow down the ball
                if (radius < slotRadius + 0.18f) yield return null; // If the ball is close to the slot, break out of the loop
                radius -= 0.045f * Time.deltaTime; // Move the ball towards the center of the wheel
                yield return null; // Wait for the next frame
            }
            radius = slotRadius; // Set the radius to the slot radius to make sure the ball stops on the slot
            _speed = 0; // Set the speed to 0 to make sure the ball stops spinning

            Transform nearestSlot = null; // The nearest slot to the ball
            float minDistance = float.MaxValue; // The minimum distance to the nearest slot
            foreach (Transform slot in _wheel.transform) // Loop through all the slots on the wheel
            {
                float distance = Vector2.Distance(transform.position, slot.position); // Calculate the distance between the ball and the slot
                if (distance < minDistance) // If the distance is less than the minimum distance
                {
                    minDistance = distance; // Set the minimum distance to the distance
                    nearestSlot = slot; // Set the nearest slot to the slot
                }
            }

            if (nearestSlot != null) // If the nearest slot is not null
            {
                transform.SetParent(nearestSlot); // Set the parent of the ball to the nearest slot
                _betHandler.CheckWin(int.Parse(nearestSlot.name)); // Check if the player has won
                StopCoroutine(DecelerateBall()); // Stop the coroutine that decelerates the ball
                _isSpinning = false; // Set the spinning flag to false
            }
        }

        IEnumerator SpinTheWheel()
        {
            _bettingTable.GetComponentInParent<CanvasGroup>().blocksRaycasts = false; // Disable the betting table
            Quaternion targetRotation = Quaternion.Euler(_targetRotation, 0, 0); // The target rotation of the betting table
            RectTransform rectTransform = _bettingTable.GetComponent<RectTransform>();

            while (Quaternion.Angle(rectTransform.rotation, targetRotation) > 1f)
            {
                rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRotation, _bettingTableRotationSpeed * Time.deltaTime);
                yield return null;
            }

            rectTransform.rotation = targetRotation;

            Debug.Log("Betting table rotated!"); // Log a message to the console
            Camera mainCamera = Camera.main; // Get the main camera
            float initialSize = mainCamera.orthographicSize; // Get the initial size of the camera
            float targetSize = initialSize / 2.5f; // The target size of the camera. zoom in 2.5 times
            float zoomSpeed = 5f; // The speed at which the camera zooms in
            while (mainCamera.orthographicSize > targetSize) // While the camera is not zoomed in completely
            {
                mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetSize, zoomSpeed * Time.deltaTime); // Zoom in the camera
                yield return null; // Wait for the next frame
            }

            Debug.Log("Spinning the wheel..."); // Log a message to the console

            radius = 2.33f; // Reset the radius of the ball
            _speed = Random.Range(270, 360); // Reset the speed of the ball
            currentAngle = 90f; // Reset the angle of the ball
            transform.SetParent(null); // Remove the parent of the ball to make it spin around the wheel
            StartCoroutine(DecelerateBall()); // Start the coroutine that decelerates the ball
        }

        void RotateWheel()
        {
            _wheel.transform.Rotate(Vector3.forward, 90 * Time.deltaTime); // Rotate the wheel 90 degrees every second. should be adjustable value.
        }

        /// <summary>
        /// Starts spinning the roulette wheel and the ball. 
        /// Should be called when the player clicks the spin button or after the betting time is over.
        /// </summary>
        public void StartSpin()
        {
            if (!_isSpinning) // If the wheel is not spinning
            {
                StartCoroutine(SpinTheWheel()); // Start the coroutine that resets the ball
                _isSpinning = true; // Set the spinning flag to true
            }
        }

        public void EnableBettingTable()
        {
            StartCoroutine(ReverseSpinTheWheel());
        }

        IEnumerator ReverseSpinTheWheel()
        {
            Camera mainCamera = Camera.main; // Get the main camera
            float targetSize = _initialCameraSize; // The target size of the camera
            float zoomSpeed = 5f; // The speed at which the camera zooms out
            while (mainCamera.orthographicSize < targetSize) // While the camera is not zoomed out completely
            {
                mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetSize, zoomSpeed * Time.deltaTime); // Zoom out the camera
                yield return null; // Wait for the next frame
            }

            Debug.Log("Reversing the wheel..."); // Log a message to the console

            Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // The target rotation of the betting table
            RectTransform rectTransform = _bettingTable.GetComponent<RectTransform>();

            while (Quaternion.Angle(rectTransform.rotation, targetRotation) > 1f)
            {
                rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, targetRotation, _bettingTableRotationSpeed * Time.deltaTime);
                yield return null;
            }

            rectTransform.rotation = targetRotation;
            _bettingTable.GetComponentInParent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
