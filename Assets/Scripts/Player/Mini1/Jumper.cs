using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public Vector3 endPoint;
    public float jumpHeight = 5.0f;
    public float jumpDuration = 1f;

    private Vector3 startPoint;
    private float a, b, c;
    private bool isJumping = false;
    private bool hasCollided = false;

    public bool IsJumping{get {return isJumping;}}

    [SerializeField] private Sound landSound;
    [SerializeField] private Sound slideSound;
    
    public void SetJumpParameters(Vector3 newEndPoint, float newJumpHeight, float newJumpDuration)
    {
        endPoint = newEndPoint;
        jumpHeight = Mathf.Max(newJumpHeight, 0.1f); // Ensure jumpHeight is always positive
        jumpDuration = Mathf.Max(newJumpDuration, 0.1f); // Ensure jumpDuration is always positive
    }

    public void Jump()
    {
        if(!isJumping)
        {
            StartCoroutine(PerformJump());
        }
    }

    private IEnumerator PerformJump()
    {
        startPoint = transform.position;
        CalculateParabolaParameters();
        isJumping = true;
        hasCollided = false;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration && !hasCollided)
        {
            float t = elapsedTime / jumpDuration;

            Vector3 distance = endPoint - startPoint;
            float jumpDistance = new Vector2(distance.x, distance.z).magnitude;
            float x = t * jumpDistance;
            float y = a * x * x + b * x + c;

            Vector3 newPosition = Vector3.Lerp(startPoint, endPoint, t);
            newPosition.y = startPoint.y + y;

            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hasCollided)
        {
            transform.position = endPoint;
        }

        isJumping = false;
    }

    private void CalculateParabolaParameters()
    {
        Vector3 distance = endPoint - startPoint;
        float jumpDistance = new Vector2(distance.x, distance.z).magnitude;

        // Calculate parabola parameters
        float x1 = 0, y1 = 0;
        float x2 = jumpDistance / 2f, y2 = jumpHeight;
        float x3 = jumpDistance, y3 = 0;

        float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        a = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        b = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
        c = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float dropDuration = 0.5f; // Duration of the Lerp
        // float minimumFallPosition = 1.05f; //Same as initial player start position
        // float safeRecoveryStartPosition = Random.Range(minimumFallPosition, 1.65f);
        float waterPosition = 0.4f;
        float safeRecoveryStartPosition = waterPosition;
        if (isJumping && !collision.gameObject.CompareTag("Mini1MedalInteractable") && !collision.gameObject.CompareTag("Mini1SafePlatform"))
        {
            hasCollided = true;
            isJumping = false;
            if(transform.position.y > safeRecoveryStartPosition)
            {
                StartCoroutine(LerpToNewPosition(safeRecoveryStartPosition, dropDuration));
            }
            // You can add additional logic here, such as playing a sound or spawning particles
            // Debug.Log("Jumper: Jump interrupted due to collision with " + collision.gameObject.name);
        }
        
        else if(collision.gameObject.CompareTag("Mini1SafePlatform") && collision.gameObject.transform.position.y >= 0f)
        {
            SoundManager.Instance.PlaySFX(landSound, gameObject.transform, 1f, false);
        }
    }

    private IEnumerator LerpToNewPosition(float targetY, float duration)
    {
        float startDropBuffer = 0.5f;
        yield return new WaitForSeconds(startDropBuffer);
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);
        SoundManager.Instance.PlaySFX(slideSound, gameObject.transform, 1f, false);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure final position is set
    }
}
