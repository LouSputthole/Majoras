using System;
using UnityEngine;

namespace Majoras.City
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class CityExploration : MonoBehaviour
    {
        public Camera viewCamera;
        public Vector3 spawnPosition = new Vector3(-2.79f, 0, -6.27f);
        public float spawnYaw;
        public float spawnPitch;
        [Min(.1f)] public float walkSpeed = 3;
        [Min(.1f)] public float runSpeed = 4.8f;
        [Min(.2f)] public float bodyHeight = 1.35f;
        [Min(.05f)] public float bodyRadius = .2f;
        [Min(.1f)] public float eyeHeight = 1.1f;
        [Min(.1f)] public float mouseSensitivity = 2;
        public bool allowJump = true;
        [Min(0)] public float jumpHeight = .45f;
        public float gravity = -18;
        public float fallResetY = -12;
        public bool startPaused = true;
        public bool freezeTimeWhenPaused = true;
        public bool handleClickToResume;

        public event Action<bool> PauseChanged;
        public event Action Respawned;
        public bool IsPaused { get; private set; }
        public bool IsGrounded => controller != null && controller.enabled && controller.isGrounded;
        public bool TestInputActive => testInputActive && Time.unscaledTime <= testExpiresAt;
        public Vector3 Velocity { get; private set; }
        public float Yaw => yaw;
        public float Pitch => pitch;
        public float HorizontalDistanceTravelled { get; private set; }
        public int RespawnCount { get; private set; }
        public CollisionFlags LastCollisionFlags { get; private set; }

        CharacterController controller;
        float yaw, pitch, verticalSpeed, resumeTimeScale = 1;
        float testExpiresAt;
        Vector2 testMove;
        bool testRun, testInputActive, manualTest, ownsTimeScale, capturedCursor;
        CursorLockMode previousCursorLock;
        bool previousCursorVisible;
        int skipLookFrames;

        void Awake()
        {
            EnsureReferences();
            ConfigureBody();
            yaw = transform.eulerAngles.y;
            if (viewCamera != null) AttachCamera();
            SetLookAngles(yaw, spawnPitch);
        }
        void OnEnable()
        {
            if (!Application.isPlaying) return;
            previousCursorLock = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            capturedCursor = true;
        }
        void Start()
        {
            SetPaused(startPaused);
            Debug.Log("[CityExploration] Ready: first-person exploration, source collision, no Link avatar.");
        }
        void OnDisable()
        {
            RestoreTimeScale();
            if (capturedCursor)
            {
                Cursor.lockState = previousCursorLock; Cursor.visible = previousCursorVisible;
                capturedCursor = false;
            }
            ClearTestInput();
        }
        void OnApplicationFocus(bool focused)
        {
            if (!focused && Application.isPlaying && !Application.isBatchMode) SetPaused(true);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SetPaused(!IsPaused);
            if (IsPaused)
            {
                if (handleClickToResume && Input.GetMouseButtonDown(0)) SetPaused(false);
                return;
            }
            if (testInputActive && !TestInputActive) ClearTestInput();
            if (!TestInputActive && Cursor.lockState == CursorLockMode.Locked)
            {
                if (skipLookFrames > 0) skipLookFrames--;
                else SetLookAngles(yaw + Input.GetAxisRaw("Mouse X") * mouseSensitivity,
                    pitch - Input.GetAxisRaw("Mouse Y") * mouseSensitivity);
            }
            if (manualTest) return;
            Vector2 move;
            bool run, jump = false;
            if (TestInputActive) { move = testMove; run = testRun; }
            else
            {
                // Key reads avoid depending on project-specific Horizontal/Vertical axis names.
                move = new Vector2((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                    (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
                run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                jump = allowJump && Input.GetKeyDown(KeyCode.Space);
            }
            Move(move, run, jump, Mathf.Min(Time.deltaTime, .05f));
        }

        public void Configure(Camera camera, Vector3 feetPosition, float lookYaw = 0, float lookPitch = 0)
        {
            viewCamera = camera; spawnPosition = feetPosition; spawnYaw = lookYaw; spawnPitch = lookPitch;
            EnsureReferences(); ConfigureBody(); AttachCamera();
            SetPosition(feetPosition); SetLookAngles(lookYaw, lookPitch);
        }
        void EnsureReferences()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            if (viewCamera == null) viewCamera = GetComponentInChildren<Camera>(true);
        }
        void ConfigureBody()
        {
            if (controller == null) return;
            controller.height = Mathf.Max(.4f, bodyHeight);
            controller.radius = Mathf.Clamp(bodyRadius, .08f, controller.height * .45f);
            controller.center = Vector3.up * (controller.height * .5f);
            controller.skinWidth = .025f;
            controller.stepOffset = Mathf.Min(.22f, controller.height * .3f);
            controller.slopeLimit = 48;
            controller.minMoveDistance = 0;
        }
        void AttachCamera()
        {
            if (viewCamera == null) return;
            viewCamera.transform.SetParent(transform, false);
            viewCamera.transform.localPosition = new Vector3(0, eyeHeight, 0);
            viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            viewCamera.nearClipPlane = .04f;
        }
        public void SetLookAngles(float lookYaw, float lookPitch)
        {
            RequireFinite(lookYaw, "lookYaw"); RequireFinite(lookPitch, "lookPitch");
            yaw = Mathf.Repeat(lookYaw, 360); pitch = Mathf.Clamp(lookPitch, -80, 80);
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            if (viewCamera != null) viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
        public void SetPosition(Vector3 feetPosition)
        {
            RequireFinite(feetPosition.x, "position.x"); RequireFinite(feetPosition.y, "position.y"); RequireFinite(feetPosition.z, "position.z");
            EnsureReferences();
            bool wasEnabled = controller != null && controller.enabled;
            if (wasEnabled) controller.enabled = false;
            transform.position = feetPosition;
            if (wasEnabled) controller.enabled = true;
            verticalSpeed = 0; Velocity = Vector3.zero; LastCollisionFlags = CollisionFlags.None;
            Physics.SyncTransforms();
        }
        public void SetPaused(bool paused)
        {
            bool changed = IsPaused != paused;
            IsPaused = paused;
            if (paused && freezeTimeWhenPaused && !ownsTimeScale)
            {
                resumeTimeScale = Time.timeScale; Time.timeScale = 0; ownsTimeScale = true;
            }
            else if (!paused) RestoreTimeScale();
            if (Application.isPlaying)
            {
                Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = paused;
            }
            skipLookFrames = 2;
            if (changed)
            {
                PauseChanged?.Invoke(paused);
                Debug.Log("[CityExploration] " + (paused ? "Paused" : "Exploring"));
            }
        }
        void RestoreTimeScale()
        {
            if (!ownsTimeScale) return;
            Time.timeScale = resumeTimeScale; ownsTimeScale = false;
        }
        public void Respawn()
        {
            SetPosition(spawnPosition + Vector3.up * .04f); SetLookAngles(spawnYaw, spawnPitch);
            RespawnCount++; Respawned?.Invoke();
            Debug.Log("[CityExploration] Returned to south entrance.");
        }
        void Move(Vector2 input, bool run, bool jump, float dt)
        {
            if (dt <= 0 || controller == null || !controller.enabled) return;
            input = Vector2.ClampMagnitude(input, 1);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            if (jump && controller.isGrounded) verticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);
            verticalSpeed = Mathf.Max(-35, verticalSpeed + gravity * dt);
            Vector3 planar = (transform.right * input.x + transform.forward * input.y) * (run ? runSpeed : walkSpeed);
            var before = transform.position;
            LastCollisionFlags = controller.Move((planar + Vector3.up * verticalSpeed) * dt);
            if ((LastCollisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0) verticalSpeed = 0;
            if ((LastCollisionFlags & CollisionFlags.Below) != 0 && verticalSpeed < 0) verticalSpeed = -2;
            var delta = transform.position - before;
            Velocity = delta / dt;
            HorizontalDistanceTravelled += new Vector2(delta.x, delta.z).magnitude;
            if (transform.position.y < fallResetY || (transform.position - spawnPosition).sqrMagnitude > 62500)
                Respawn();
        }

        // Input overrides expire after 30 real seconds and never bypass pause/collision.
        public void SetTestInput(Vector2 move, bool run)
        {
            RequireFinite(move.x, "input.x"); RequireFinite(move.y, "input.y");
            testMove = Vector2.ClampMagnitude(move, 1); testRun = run;
            testInputActive = true; manualTest = false; testExpiresAt = Time.unscaledTime + 30;
        }
        // Switches the active override to explicit stepping, preventing Update from also moving.
        // Call SetTestInput first. Every step is bounded to 50 ms; invalid steps throw.
        public void SimulateTestStep(float seconds)
        {
            RequireFinite(seconds, "seconds");
            if (seconds <= 0 || seconds > .05f) throw new ArgumentOutOfRangeException(nameof(seconds), "Use a step in (0, .05].");
            if (!TestInputActive) throw new InvalidOperationException("Set a test input first (expires after 30 real seconds).");
            manualTest = true;
            if (!IsPaused) Move(testMove, testRun, false, seconds);
        }
        public void ClearTestInput()
        {
            testInputActive = false; manualTest = false; testMove = Vector2.zero; testRun = false;
        }
        static void RequireFinite(float value, string name)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(name);
        }
    }
}
