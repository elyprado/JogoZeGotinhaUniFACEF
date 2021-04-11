using UnityEngine;

public class ResetObjects_00004 : MonoBehaviour
{
    public GameObject[] objects = new GameObject[0];
    private Vector3[] positionData;
    private Quaternion[] rotationData;


    private void Awake()
    {
        CreateResetData();
    }

    private void CreateResetData()
    {
        positionData = new Vector3[objects.Length];
        rotationData = new Quaternion[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            positionData[i] = objects[i].transform.position;
            rotationData[i] = objects[i].transform.rotation;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].transform.position = positionData[i];
            objects[i].transform.rotation = rotationData[i];
        }
    }
}