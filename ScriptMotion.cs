using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMotion : MonoBehaviour
{
        public enum PathfTypes 
        {
            Square,
            Triangle,
            Hexagon,
            Star
        }
        
        [SerializeField] private PathfTypes _pathf;
        [SerializeField, Range (0,10.0f)] private float _speed;
        [SerializeField, Range (0,10.0f)] private float _stepSize = 5;

        private NewControls _controls;
        private Coroutine _motionRoutine;
        private Vector3 _startPosition = Vector3.zero;
        private List<GameObject> _pointShapes = new List<GameObject>();

        private void Awake() => _controls = new NewControls();

        private void OnEnable()
        {
            _controls.Enable();
            _controls.ShapeMotion.StartMotion.started += callbackContext => StartMotion(_pathf);
            _controls.ShapeMotion.StopMotion.started += callbackContext => StopMotion();
        }

        private void StartMotion(PathfTypes pathf)
        {
            switch (pathf)
            {
                case PathfTypes.Square:
                    _motionRoutine = StartCoroutine(ShapeMotion(90));
                    break;

                case PathfTypes.Triangle:
                    _motionRoutine = StartCoroutine(ShapeMotion(120));
                    break;

                case PathfTypes.Hexagon:
                    _motionRoutine = StartCoroutine(ShapeMotion(60));
                    break;

                case PathfTypes.Star:
                    _motionRoutine = StartCoroutine(StarMotion());
                    break;
            }
        }

        private IEnumerator MotionForward()
        {
            Vector3 targetPosition = transform.position + transform.forward * _stepSize;

            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
                yield return null;
            }

            yield break;
        }

        private IEnumerator Rotation(float angle)
        {
            transform.Rotate(Vector3.up, angle);
            yield break;
        }

        private IEnumerator ShapeMotion(float rotationAngle)
        {
            do
            {
                yield return MotionForward();
                AddPointShape();
                yield return Rotation(rotationAngle);
            }
            while (transform.position != _startPosition);
        }

        private IEnumerator StarMotion()
        {
            do
            {
                yield return MotionForward();
                AddPointShape();
                yield return Rotation(-72);
                yield return MotionForward();
                AddPointShape();
                yield return Rotation(144);
            }
            while (transform.position != _startPosition);
        }

        private void StopMotion()
        {
            StopCoroutine(_motionRoutine);
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
            RemoveAllPointShapes();
        }

        private void AddPointShape()
        {
            float shapeSize = 0.3f;
            GameObject createdShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
            createdShape.transform.localScale = Vector3.one * shapeSize;
            createdShape.transform.position = transform.position;
            _pointShapes.Add(createdShape);
        }

        private void RemoveAllPointShapes()
        {
            _pointShapes.ForEach(shape => Destroy(shape));
            _pointShapes.Clear();
        }

        private void OnDisable() => _controls.Disable();
    }
