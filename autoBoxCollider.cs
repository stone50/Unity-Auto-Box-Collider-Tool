using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxCollider))]
public class autoBoxCollider : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Auto"))
        {
            generateBoxCollider(target as BoxCollider);
        }
    }

    public void generateBoxCollider(BoxCollider box)
    {
        MeshFilter[] meshFilters = box.gameObject.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogError("No Mesh Filters Found");
            return;
        }

        Quaternion rotation = box.transform.rotation;
        box.transform.rotation = Quaternion.Euler(0, 0, 0);

        float maxX = meshFilters[0].sharedMesh.bounds.center.x;
        float maxY = meshFilters[0].sharedMesh.bounds.center.y;
        float maxZ = meshFilters[0].sharedMesh.bounds.center.z;
        float minX = meshFilters[0].sharedMesh.bounds.center.x;
        float minY = meshFilters[0].sharedMesh.bounds.center.y;
        float minZ = meshFilters[0].sharedMesh.bounds.center.z;

        foreach (MeshFilter currentMeshFilter in meshFilters)
        {
            Vector3[] boundingCorners = findBoundingCorners(currentMeshFilter);
            foreach (Vector3 corner in boundingCorners)
            {

                maxX = Mathf.Max(maxX, corner.x);
                maxY = Mathf.Max(maxY, corner.y);
                maxZ = Mathf.Max(maxZ, corner.z);
                minX = Mathf.Min(minX, corner.x);
                minY = Mathf.Min(minY, corner.y);
                minZ = Mathf.Min(minZ, corner.z);
            }
        }
        box.transform.rotation = rotation;

        Undo.RecordObject(box, "Auto Generated Box Collider");
        box.center = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
        box.size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
    }

    public Vector3[] findBoundingCorners(MeshFilter mesh)
    {
        Transform trans = mesh.transform;
        Vector3 center = mesh.sharedMesh.bounds.center;
        Vector3 extents = mesh.sharedMesh.bounds.extents;
        Quaternion rotation = trans.rotation;
        return new Vector3[]
        {
            (rotation * (center + extents)) + trans.localPosition,
            (rotation * new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z)) + trans.localPosition,
            (rotation * new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z)) + trans.localPosition,
            (rotation * new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z)) + trans.localPosition,
            (rotation * new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)) + trans.localPosition,
            (rotation * new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z)) + trans.localPosition,
            (rotation * new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z)) + trans.localPosition,
            (rotation * (center - extents)) + trans.localPosition
        };
    }
}