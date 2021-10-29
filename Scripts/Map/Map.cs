using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    [Header("Set Dynamically:")]
    public GameObject GO_Map;
    public TurnSystem t;
    public List<GameObject> tileList;
    public List<GameObject> baseTileList;
    
    public List<GameObject> midlandTileList;
    public List<GameObject> mineTileList;
    GameObject tempGO;
    // Use this for initialization
    void Start()
    {
        GO_Map = this.gameObject;
        t = FindObjectOfType<TurnSystem>(); //looks for an object with a "Map" component and stores it
        GenList(); //First step in the initialization
    }
    void GenList() {
        foreach (Transform child in transform)
        {
            tempGO = child.gameObject;
            if (tempGO.GetComponent<Map_SubDivision>() != null) //if it finds a subdivision
            {
                Map_SubDivision mSubDiv = tempGO.GetComponent<Map_SubDivision>(); //Gets access to Map_Subdivision for that particular instance
                mSubDiv.SubDivListGen();//generate the lsit in the subdivision

                for (int i = 0; i < mSubDiv.SubDiv_TileList.Count; i++){ //for loop that copies the list insisde the subdivision
                    tileList.Add(mSubDiv.SubDiv_TileList[i]); //puts the information inside 'TileList'
                }
                mSubDiv = null; //Empties the variable to prevent memory garbage
            }
            else
            { //finds the highland and excludes the subdivisions
                if (tempGO.tag == "Highland")
                {
                    Tile_Properties tempT_Prop = child.GetComponent<Tile_Properties>();
                    tempT_Prop.TilePropertiesDefault(); //sets this tile's properties to their default (used in initialization)
                    tileList.Add(child.gameObject); //Adds the highland
                }
            }
            tempGO = null; //Empties the variable to prevent memory garbage
        }
        FindBases(); //3rd step in the initialization
    }
    void FindBases() {
        for (int j = 0; j < tileList.Count; j++)
        {
            if (tileList[j].tag == "Base")
            {
                baseTileList.Add(tileList[j].gameObject); //Adds the highland
            }
        }
        t.FindPlayers(); //4th step in the initialization
    }

   
	// Update is called once per frame
	void Update () {
		
	}
}
