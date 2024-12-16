using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public GameObject Object;
    public Material[] materials = new Material[8];  // The material to switch to

    public void changeMaterial(int index){
        if(index >= materials.Length) return; //Checking the material is in the list

        Object.GetComponent<MeshRenderer>().material = materials[index];
    }
}