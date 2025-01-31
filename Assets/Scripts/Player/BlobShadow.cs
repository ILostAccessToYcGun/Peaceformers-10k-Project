using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    public GameObject shadow;
    public RaycastHit hit;
    public float offset;

    private void Update()
    {
        Ray downRay = new Ray(new Vector3(this.transform.position.x, this.transform.position.y - offset, this.transform.position.z), -Vector3.up);

        Vector3 hitPosition = hit.point;
        
        if (Physics.Raycast(downRay, out hit))
        {
            shadow.transform.position = hitPosition;
        }
    }
}
