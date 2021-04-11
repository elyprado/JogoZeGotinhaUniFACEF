using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenShooter_000004 : MonoBehaviour
{
    public bool useMouseFiring = true;
    public int force = 500;
    public float massMultiplier = 1f;
    public int timer = 10;

    public enum Selectibles { Sphere, Capsule, Cube, Cylinder, Custom };
    public Selectibles selectedObject = Selectibles.Sphere;

    public GameObject customObject;
    public GameObject projectile;

    private void Update()
    {
        if (useMouseFiring)
        {
            //  Check if the user is clicking
            if (Input.GetMouseButtonDown(0))
            {
                //	Check if the mouse is not on an UI object 
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Shoot();
                }
            }
        }
    }

    public void Shoot()
    {
        if ((selectedObject == Selectibles.Custom && customObject != null) || (selectedObject != Selectibles.Custom))
        {
            GameObject bullet = CreateProjectile();

            //	Position the bullet at the camera's transform
            bullet.transform.position = Camera.main.transform.position;

            Rigidbody rigidbody = new Rigidbody();
            if (bullet.GetComponent<Rigidbody>() == null)
            {
                rigidbody = bullet.AddComponent<Rigidbody>();
                rigidbody.mass = massMultiplier;
                rigidbody.angularDrag = 0.01f;
                rigidbody.drag = 0.01f;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                bullet.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            }
            else
            {
                rigidbody = bullet.GetComponent<Rigidbody>();
            }

            //	Add force to the bullet in the direction to where the user clicks
            Vector3 direction = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f);
            bullet.transform.LookAt(Camera.main.ScreenToWorldPoint(direction));
            rigidbody.AddRelativeForce(Vector3.forward * rigidbody.mass * force);

            //	Set a timer which will destroy the object
            Destroy(bullet, timer);
        }
    }

    private GameObject CreateProjectile()
    {
        //	Function used to assign the right geometry or primitive
        GameObject returnObject;
        switch (selectedObject)
        {
            case Selectibles.Sphere:
                returnObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                return returnObject;

            case Selectibles.Capsule:
                returnObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                return returnObject;

            case Selectibles.Cube:
                returnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                return returnObject;

            case Selectibles.Cylinder:
                returnObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                return returnObject;

            case Selectibles.Custom:
                returnObject = Instantiate(customObject) as GameObject;
                return returnObject;

            default:
                return null;
        }
    }
}

[CustomEditor(typeof(ScreenShooter_000004))]
public class ScreenShooter_000004CE : Editor
{
    public override void OnInspectorGUI()
    {
        //	Store the ScreenShooter class
        ScreenShooter_000004 sSClass = target as ScreenShooter_000004;

        sSClass.useMouseFiring = EditorGUILayout.Toggle("Use mouse firing", sSClass.useMouseFiring);

        //  Make the selected object type visible
        sSClass.selectedObject = (ScreenShooter_000004.Selectibles)EditorGUILayout.EnumPopup("Object type", sSClass.selectedObject);
        if (sSClass.selectedObject == ScreenShooter_000004.Selectibles.Custom)
        {
            sSClass.customObject = (GameObject)EditorGUILayout.ObjectField(sSClass.customObject, typeof(GameObject), false);
        }

        //  Make the 'force' value visible
        sSClass.force = EditorGUILayout.IntField("Force", sSClass.force);
        sSClass.massMultiplier = EditorGUILayout.FloatField("Mass multiplier", sSClass.massMultiplier);
        sSClass.timer = EditorGUILayout.IntField("Life (secs)", sSClass.timer);
    }
}