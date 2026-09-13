using DuetCats.Gameplay;
using DuetCats.Session;
using UnityEngine;

namespace DuetCats.Controls
{
    [DefaultExecutionOrder(50)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(GameplayLayout))]
    public sealed class CatInput : MonoBehaviour
    {
        private const int NoPointer = int.MinValue;
        private const int MousePointer = -1;

        [SerializeField, Min(0.01f)] private float dragSensitivity = 1f;
        [SerializeField, Min(0.01f)] private float keyboardSpeed = 0.5f;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameplayLayout gameplayLayout;

        private int leftPointerId = NoPointer;
        private int rightPointerId = NoPointer;
        private float leftCatX;
        private float rightCatX;
        private float mousePreviousX;

        public float LeftCatX { get { return leftCatX; } }
        public float RightCatX { get { return rightCatX; } }

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GetComponent<GameSession>();
            }

            if (gameplayLayout == null)
            {
                gameplayLayout = GetComponent<GameplayLayout>();
            }
        }

        private void Start()
        {
            if (gameSession == null || gameplayLayout == null ||
                !gameplayLayout.Initialize(gameSession.SongContent))
            {
                Debug.LogError("CatInput needs a valid GameSession and GameplayLayout.", this);
                enabled = false;
                return;
            }

            leftCatX = gameplayLayout.GetDefaultCatX(CatSide.Left);
            rightCatX = gameplayLayout.GetDefaultCatX(CatSide.Right);
        }

        private void Update()
        {
            if (gameSession.Phase != GamePhase.Playing)
            {
                ReleaseAllPointers();
                return;
            }

            ProcessKeyboard();

            if (Input.touchCount > 0)
            {
                ProcessTouches();
            }
            else
            {
                ProcessMouse();
            }
        }

        private void ProcessKeyboard()
        {
            var distance = keyboardSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.A))
            {
                MoveCat(CatSide.Left, -distance);
            }

            if (Input.GetKey(KeyCode.D))
            {
                MoveCat(CatSide.Left, distance);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveCat(CatSide.Right, -distance);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveCat(CatSide.Right, distance);
            }
        }

        private void ProcessTouches()
        {
            for (var index = 0; index < Input.touchCount; index++)
            {
                var touch = Input.GetTouch(index);
                if (touch.phase == TouchPhase.Began)
                {
                    TryClaimPointer(touch.fingerId, touch.position.x);
                    continue;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    MoveClaimedPointer(touch.fingerId, touch.deltaPosition.x);
                    continue;
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    ReleasePointer(touch.fingerId);
                }
            }
        }

        private void ProcessMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePreviousX = Input.mousePosition.x;
                TryClaimPointer(MousePointer, mousePreviousX);
            }

            if (Input.GetMouseButton(0) && IsPointerClaimed(MousePointer))
            {
                var mouseX = Input.mousePosition.x;
                MoveClaimedPointer(MousePointer, mouseX - mousePreviousX);
                mousePreviousX = mouseX;
            }

            if (Input.GetMouseButtonUp(0))
            {
                ReleasePointer(MousePointer);
            }
        }

        private void TryClaimPointer(int pointerId, float screenX)
        {
            if (gameSession.Phase != GamePhase.Playing || IsPointerClaimed(pointerId))
            {
                return;
            }

            var side = screenX < Screen.width * 0.5f ? CatSide.Left : CatSide.Right;
            if (side == CatSide.Left)
            {
                if (leftPointerId == NoPointer)
                {
                    leftPointerId = pointerId;
                }

                return;
            }

            if (rightPointerId == NoPointer)
            {
                rightPointerId = pointerId;
            }
        }

        private void MoveClaimedPointer(int pointerId, float deltaPixels)
        {
            var deltaX = deltaPixels / Mathf.Max(1f, Screen.width) * dragSensitivity;
            if (pointerId == leftPointerId)
            {
                MoveCat(CatSide.Left, deltaX);
            }
            else if (pointerId == rightPointerId)
            {
                MoveCat(CatSide.Right, deltaX);
            }
        }

        private void MoveCat(CatSide side, float deltaX)
        {
            if (side == CatSide.Left)
            {
                leftCatX = gameplayLayout.ClampCatX(side, leftCatX + deltaX);
                return;
            }

            rightCatX = gameplayLayout.ClampCatX(side, rightCatX + deltaX);
        }

        private bool IsPointerClaimed(int pointerId)
        {
            return pointerId == leftPointerId || pointerId == rightPointerId;
        }

        private void ReleasePointer(int pointerId)
        {
            if (pointerId == leftPointerId)
            {
                leftPointerId = NoPointer;
            }

            if (pointerId == rightPointerId)
            {
                rightPointerId = NoPointer;
            }
        }

        private void ReleaseAllPointers()
        {
            leftPointerId = NoPointer;
            rightPointerId = NoPointer;
        }
    }
}
