using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rotationSpeed = 10f;
    private Vector2 lookInput;

    public void RotateGunTowardsMouse()
    {
        if (playerCamera == null || playerRoot == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPoint = hit.point;

            Vector3 direction = (targetPoint - playerRoot.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
