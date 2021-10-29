using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_SubDivision : MonoBehaviour {
    [Header("Set Dynamically:")]
    public GameObject GO_MapSubDiv;
    public List<GameObject> SubDiv_TileList;
    private GameObject tempGO;


    public void SubDivListGen() { //2nd step in the initialization
        GO_MapSubDiv = this.gameObject;
        foreach (Transform child in transform) //finds every child object in the subdivision
        {
            tempGO = child.gameObject; //gains access to the instance
            if (tempGO.GetComponent<Tile_Properties>() != null)
            { //looks for Tile_Properties to confirm it is in fact a 'Tile'
                Tile_Properties tempProp = tempGO.GetComponent<Tile_Properties>();
                tempProp.TilePropertiesDefault(); //sets this tile's properties to their default (used in initialization)
                SubDiv_TileList.Add(child.gameObject); //adds the tile to the list
                
            }
            tempGO = null; //Empties the variable to prevent memory garbage
        }
    }
}
