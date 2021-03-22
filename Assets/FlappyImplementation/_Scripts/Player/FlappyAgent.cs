using GeneticNeuralNetwork;
using UnityEditor;
using UnityEngine;

namespace FlappyPlane
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [SelectionBase]
    public class FlappyAgent : Agent
    {
#if UNITY_EDITOR
        [SerializeField] private bool cheat;
#endif
        private Rigidbody2D rb;
        private Vector2 forceVector;

        public float force;
        public int numberOfLasers;
        public Transform[] lasers;
        public LayerMask objectsLayer;

        private const float velocityCap = .1f;
        public const float EyeDistanceCap = 20f;
        private const float agentSize = 1;
        private RaycastHit2D[] hits = new RaycastHit2D[1];
        private static ContactFilter2D filter;
        private static bool setupFilter = true;

        private void Awake()
        {
            GameController.OnGameStart += StartGame;
            Score.OnScoreChange += AdjustPosition;
            SetupAgent();
        }

        protected override void SetupAgent()
        {
            if (setupFilter)
                SetupFilter();
            forceVector = new Vector2(0, force);
            rb = GetComponent<Rigidbody2D>();
            rb.simulated = true;
        }

        private void SetupFilter()
        {
            filter = new ContactFilter2D
            {
                layerMask = objectsLayer,
                useLayerMask = true,
                maxDepth = transform.position.z + 1,
                minDepth = transform.position.z - 1,
                useDepth = true
            };
            setupFilter = false;
        }

        private void StartGame()
        {
            rb.simulated = true;
        }

        private void Jump()
        {
            rb.velocity = forceVector;
        }

        private void AdjustPosition(int score)
        {
            Vector3 temp = transform.position;
            temp.x = -.8f * ((float)score / GameController.ScoreToMaxSpeed).Capped(0, 1);
            transform.position = temp;
        }

        private void FixedUpdate()
        {
            if (Dead)
                return;
            output = NeuralNet.Query(GetInputs());
            if (output[0] > 0)
                Jump();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
#if UNITY_EDITOR
            if (cheat)
                return;
#endif
            if (!Dead)
                Die();
        }

        private void Die()
        {
            Dead = true;
            rb.simulated = false;
            Fitness = Score.CurrentScore;
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
        }

        private void OnDestroy()
        {
            GameController.OnGameStart -= StartGame;
            Score.OnScoreChange -= AdjustPosition;
        }

        protected override float[] GetInputs()
        {
            // Bias
            input[0] = 1;

            // Get velocity
            input[1] = rb.velocity.y / velocityCap;
            input[2] = GameController.Speed / GameController.MaxSpeed;

            // Get lasers
            for (int i = 0; i < lasers.Length; i++)
                input[3 + i] = DistanceFromEye(lasers[i]);

            return input;
        }

        private float DistanceFromEye(Transform laser)
        {
            if (Physics2D.Raycast(laser.position, laser.right, filter, hits, EyeDistanceCap) > 0)
                return hits[0].distance - agentSize / 2;
            return EyeDistanceCap;
        }

        private Vector3 GetHitPoint(Transform laser)
        {
            if (!Application.isPlaying)
                filter = new ContactFilter2D
                {
                    layerMask = objectsLayer,
                    useLayerMask = true,
                    maxDepth = transform.position.z + 1,
                    minDepth = transform.position.z - 1,
                    useDepth = true
                };
            if (Physics2D.Raycast(laser.position, laser.right, filter, hits, EyeDistanceCap) > 0)
                return hits[0].point;
            return laser.position;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Selection.activeGameObject.Equals(gameObject))
                return;

            // Lasers
            Gizmos.color = Color.yellow;
            for (int i = 0; i < lasers.Length; i++) {
                if (lasers[i] != null) {
                    Gizmos.DrawLine(lasers[i].position, GetHitPoint(lasers[i]));
                }
            }
        }
    }
}