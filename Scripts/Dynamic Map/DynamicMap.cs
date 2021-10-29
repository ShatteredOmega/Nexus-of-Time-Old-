using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [General Note:]
/// The order of operations for the game's initialization goes something like this. DynMap -> Turn System -> PlayableCharacter -> PC_Archetype -> everything else
/// 
/// if we have a map of gridX length, and gridY height, then 0,0 is our furthest -X and -Y;
/// </summary>

public class DynamicMap : MonoBehaviour
{
    public List<Vector3Int> TileCoord;
    [Header("Set Manually:")]
    public GameObject TILEPREFAB;
    public int mapToGen = 0; // 0 = Small, 1 = Medium, 2= Big

    [Header("Set Dynamically:")]
    public TurnSystem t;
    public GameObject GO_DynMap;
    public GameObject[,] XY_TileArray;
    public Tile_Properties[,] XY_TilePropArray;


    public List<Tile_Properties> Bases;
    public List<Tile_Properties> Basewalls;
    //public List<List<Tile_Properties>> compoundNeighbors;

    public int gridX;
    public int gridY;
    public int gridArea;

    void Start()
    {
        GO_DynMap = this.gameObject;
        t = FindObjectOfType<TurnSystem>(); //looks for an object with a "Map" component and stores it
        GO_DynMap.transform.position = Vector3.zero;
        switch (mapToGen)
        {
            case 0:
                {
                    GenerateMap_Small();
                    break;
                }
            case 1:
                {
                    GenerateMap_Medium();
                    break;
                }
            case 2:
                {
                    GenerateMap_Big();
                    break;
                }
        }

    }

    void spawnTiles(GameObject[,] something)
    {

        XY_TileArray = new GameObject[gridX, gridY]; //defining a new XY_TileArray[,] based on the grid's coordinates
        XY_TilePropArray = new Tile_Properties[gridX, gridY];
        for (int i = 0; i < gridY; i++)
        { //note that 'i' starts at 0
            XY_TileArray[0, i] = (Instantiate(TILEPREFAB, (new Vector3(0f, i, 0f)), TILEPREFAB.transform.rotation, this.transform)); //instantiate X tile for every 'i'
            XY_TileArray[0, i].name = "Tile: X: 0 , Y: " + i; //set the name of the newly instantiated object to match its position
            XY_TilePropArray[0, i] = XY_TileArray[0, i].GetComponent<Tile_Properties>();
            XY_TilePropArray[0, i].TileIndex_Y = i;
            XY_TilePropArray[0, i].TileIndex_X = 0;

            for (int j = 1; j < gridX; j++)
            { //note that 'j' starts at 1, this is to prevent two instances of the first line of tiles
                if (j % 2 == 0)
                {
                    XY_TileArray[j, i] = (Instantiate(TILEPREFAB, (new Vector3((0.85f * j), i, 0)), TILEPREFAB.transform.rotation, this.transform)); //instantiate Z tile for every 'j'
                    XY_TileArray[j, i].name = "Tile: X: " + j + " , Y: " + i; //set the name of the newly instantiated object to match its position
                }
                else
                {
                    XY_TileArray[j, i] = (Instantiate(TILEPREFAB, (new Vector3((0.85f * j), (i + 0.5f), 0)), TILEPREFAB.transform.rotation, this.transform)); //instantiate Z tile for every 'j'
                    XY_TileArray[j, i].name = "Tile: X: " + j + " , Y: " + i; //set the name of the newly instantiated object to match its position
                }
                XY_TilePropArray[j, i] = XY_TileArray[j, i].GetComponent<Tile_Properties>();
                XY_TilePropArray[j, i].TileIndex_X = j;
                XY_TilePropArray[j, i].TileIndex_Y = i;

            }

        }
    }

    void GenerateMap_Medium()
    {
        gridX = 15;
        gridY = 15;
        gridArea = gridX * gridY; //declaring that the grid's area (i.e. it's total size)
        XY_TileArray = new GameObject[gridX, gridY]; //defining a new xzArray[,] based on the grid's coordinates
        spawnTiles(XY_TileArray);
        SetTileDefaults();
        SetHighland();
        SetBases_Medium();
        SetMines_Medium();
        t.FindPlayers();
    }

    void GenerateMap_Small()
    {
        gridX = 11;
        gridY = 11;

        gridArea = gridX * gridY; //declaring that the grid's area (i.e. it's total size)
        XY_TileArray = new GameObject[gridX, gridY]; //defining a new xzArray[,] based on the grid's coordinate
        spawnTiles(XY_TileArray);
        SetTileDefaults();
        SetHighland();
        SetBases_Small();
        SetMines_Small();
        SetQuarries_Small();
        SetSanctuaries_Small();
        SetMidland_Small();
        SetLowland_Small();
        HideInactive();
        t.FindPlayers();
    }

    void GenerateMap_Big()
    {
        gridX = 19;
        gridY = 19;
        gridArea = gridX * gridY; //declaring that the grid's area (i.e. it's total size)
        XY_TileArray = new GameObject[gridX, gridY]; //defining a new xzArray[,] based on the grid's coordinates
        spawnTiles(XY_TileArray);
        SetTileDefaults();
        SetHighland();
    }

    void SetTileDefaults()
    {

        for (int i = 0; i < gridY; i++)
        {
            for (int j = 0; j < gridX; j++)
            {
                XY_TilePropArray[j, i].tag = "Inactive";
                XY_TilePropArray[j, i].TilePropertiesDefault();
                XY_TilePropArray[j, i].tType = tileType.Inactive;
                XY_TilePropArray[j, i].tileDefaultColor = XY_TilePropArray[j, i].Srend.color;

                XY_TilePropArray[j, i].X = j;
                XY_TilePropArray[j, i].Y = i - (j / 2);
                XY_TilePropArray[j, i].Z = 0 - (j + (i - j / 2));
                XY_TilePropArray[j, i].individualTileCoords = new Vector3Int(XY_TilePropArray[j, i].X, XY_TilePropArray[j, i].Y, XY_TilePropArray[j, i].Z);
                XY_TilePropArray[j, i].rangeTo = int.MaxValue;
                XY_TilePropArray[j, i].distanceTo = int.MaxValue;
                SetNeighbors(j, i);
            }

        }
    }

    void HideInactive()
    {
        for (int i = 0; i < gridY; i++)
        {
            for (int j = 0; j < gridX; j++)
            {
                XY_TilePropArray[j, i].individualTileCoords = new Vector3Int(XY_TilePropArray[j, i].X, XY_TilePropArray[j, i].Y, XY_TilePropArray[j, i].Z); //Sets the Vector3ints in each individual tile to the new values
                TileCoord.Add(XY_TilePropArray[j, i].individualTileCoords);
                XY_TilePropArray[j, i].HideInactiveTiles();
            }
        }
    }

    void SetHighland()
    {
        GameObject tempGO;
        Tile_Properties tileProps;
        //Manual Settings
        tempGO = XY_TileArray[((gridX - 1) / 2), ((gridY - 1) / 2)];
        // = XYZ_TileArray[0, 0, 0];

        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Highland";
        tileProps.Srend.color = new Color(1, 0, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        tileProps.tType = tileType.Highland;

        Camera c;
        c = FindObjectOfType<Camera>();
        c.transform.position = new Vector3(tempGO.transform.position.x, tempGO.transform.position.y, c.transform.position.z);
    }

    void SetBases_Medium() //[SHUFFLE] -> means that this needs to be changed when we get to turn and base location shuffling  
    {
        GameObject tempGO;
        Tile_Properties tileProps;
        //Manual Settings

        //P1 Base
        tempGO = XY_TileArray[1, gridY - 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p1;//[SUFFLE]

        //P1 BaseWall
        tempGO = XY_TileArray[2, gridY - 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p1;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //P2 Base
        tempGO = XY_TileArray[1, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p2;//[SUFFLE]


        //P2 BaseWall
        tempGO = XY_TileArray[2, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p2;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //P3 Base
        tempGO = XY_TileArray[gridX - 2, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p3;//[SUFFLE]

        //P3 BaseWall
        tempGO = XY_TileArray[gridX - 3, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p3;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //P4 Base
        tempGO = XY_TileArray[gridX - 2, gridY - 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p4;//[SUFFLE]

        //P4 BaseWall
        tempGO = XY_TileArray[gridX - 3, gridY - 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p4;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //P5 Base
        tempGO = XY_TileArray[(gridX / 2), gridY - 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p5;//[SUFFLE]

        //P5 BaseWall
        tempGO = XY_TileArray[(gridX / 2), gridY - 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p5;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //Player 6 Base
        tempGO = XY_TileArray[(gridX / 2), 1];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Base";
        tileProps.Srend.color = new Color(0, 0, 1);
        tileProps.tileDefaultColor = tileProps.Srend.color;
        Bases.Add(tileProps);
        tileProps.tType = tileType.Base;
        tileProps.ownedBy = OwnedBy.p6;//[SUFFLE]

        //P6 BaseWall
        tempGO = XY_TileArray[(gridX / 2), 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "BaseWall";
        tileProps.tType = tileType.BaseWall;
        tileProps.ownedBy = OwnedBy.p6;//[SUFFLE]
        tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

    }

    void SetBases_Small()
    {
        {
            GameObject tempGO;
            Tile_Properties tileProps;
            //Manual Settings

            //P1 Base
            tempGO = XY_TileArray[1, 7];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p1;//[SUFFLE] 

            //P1 BaseWall
            tempGO = XY_TileArray[2, 7];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p1;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

            //P2 Base
            tempGO = XY_TileArray[1, 3];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p2;//[SUFFLE]


            //P2 BaseWall
            tempGO = XY_TileArray[2, 4];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p2;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

            //P3 Base
            tempGO = XY_TileArray[9, 7];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p3;//[SUFFLE]

            //P3 BaseWall
            tempGO = XY_TileArray[8, 7];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p3;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

            //P4 Base
            tempGO = XY_TileArray[9, 3];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p4;//[SUFFLE]

            //P4 BaseWall
            tempGO = XY_TileArray[8, 4];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p4;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

            //P5 Base
            tempGO = XY_TileArray[5, 9];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p5;//[SUFFLE]

            //P5 BaseWall
            tempGO = XY_TileArray[5, 8];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p5;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

            //Player 6 Base
            tempGO = XY_TileArray[5, 1];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "Base";
            tileProps.Srend.color = new Color(0, 0, 1);
            tileProps.tileDefaultColor = tileProps.Srend.color;
            Bases.Add(tileProps);
            tileProps.tType = tileType.Base;
            tileProps.ownedBy = OwnedBy.p6;//[SUFFLE]

            //P6 BaseWall
            tempGO = XY_TileArray[5, 2];
            tileProps = tempGO.GetComponent<Tile_Properties>();
            tempGO.tag = "BaseWall";
            Basewalls.Add(tileProps);
            tileProps.tType = tileType.BaseWall;
            tileProps.ownedBy = OwnedBy.p6;//[SUFFLE]
            tileProps.Srend.color = new Color(0, 0.75f, 0.75f);
            tileProps.tileDefaultColor = tileProps.Srend.color;

        }
    } //[SHUFFLE] -> means that this needs to be changed when we get to turn and base location shuffling

    void SetNeighbors(int X, int Y)
    { //this is what defines the order of which neighbor is what number
        Find_North(X, Y); //0
        Find_South(X, Y); //1
        Find_NE(X, Y); //2
        Find_SW(X, Y); //3
        Find_NW(X, Y); //4
        Find_SE(X, Y); //5
    }

    void Find_North(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since North will ALWAYS be 0 in this list, then I can hardcode it for reference ;)
        if (index_Y < gridY - 1) //North Neighbor Finder
        {

            XY_TilePropArray[index_X, index_Y].N_Neighbor = XY_TileArray[index_X, (index_Y + 1)];
            XY_TilePropArray[index_X, index_Y].individualNeighborsList[0] = XY_TilePropArray[index_X, (index_Y + 1)]; //Always if available
        }
        else
        {
            XY_TilePropArray[index_X, index_Y].N_Neighbor = null;
        }
    }

    void Find_South(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since South will ALWAYS be 1 in this list, then I can hardcode it for reference ;)
        if (index_Y > 0) //South Neighbor Finder
        {
            XY_TilePropArray[index_X, index_Y].S_Neighbor = XY_TileArray[index_X, (index_Y - 1)];
            XY_TilePropArray[index_X, index_Y].individualNeighborsList[1] = XY_TilePropArray[index_X, (index_Y - 1)];
        }
        else
        {
            XY_TilePropArray[index_X, index_Y].S_Neighbor = null;
        }
    }

    void Find_NE(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since NorthEast will ALWAYS be 2 in this list, then I can hardcode it for reference ;)
        if (index_X < gridX - 1) //NE neighbor finder
        {
            if (index_X % 2 == 0) //if evens
            {
                if (index_Y < gridY - 1)
                {
                    XY_TilePropArray[index_X, index_Y].NE_Neighbor = XY_TileArray[(index_X + 1), index_Y];
                    XY_TilePropArray[index_X, index_Y].individualNeighborsList[2] = XY_TilePropArray[(index_X + 1), index_Y];
                }
            }
            else if (index_Y < gridY - 1) //if odds
            {
                XY_TilePropArray[index_X, index_Y].NE_Neighbor = XY_TileArray[(index_X + 1), (index_Y + 1)];
                XY_TilePropArray[index_X, index_Y].individualNeighborsList[2] = XY_TilePropArray[(index_X + 1), (index_Y + 1)];
            }
        }
    }

    void Find_SW(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since SouthWest will ALWAYS be 3 in this list, then I can hardcode it for reference ;)
        if (index_X > 0) //SW neighbor finder
        {
            if (index_X % 2 == 0) //if evens
            {
                if (index_Y > 0)
                {
                    XY_TilePropArray[index_X, index_Y].SW_Neighbor = XY_TileArray[(index_X - 1), (index_Y - 1)];
                    XY_TilePropArray[index_X, index_Y].individualNeighborsList[3] = XY_TilePropArray[(index_X - 1), (index_Y - 1)];
                }
            }
            else //if odds
            {
                XY_TilePropArray[index_X, index_Y].SW_Neighbor = XY_TileArray[(index_X - 1), index_Y];
                XY_TilePropArray[index_X, index_Y].individualNeighborsList[3] = XY_TilePropArray[(index_X - 1), index_Y];
            }
        }
    }

    void Find_NW(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since NorthWest will ALWAYS be 4 in this list, then I can hardcode it for reference ;)
        if (index_X > 0) //NW neighbor finder
        {
            if (index_X % 2 == 0) //if evens
            {
                XY_TilePropArray[index_X, index_Y].NW_Neighbor = XY_TileArray[(index_X - 1), index_Y];
                XY_TilePropArray[index_X, index_Y].individualNeighborsList[4] = XY_TilePropArray[(index_X - 1), index_Y];
            }
            else if (index_Y < gridY - 1)//if odds
            {
                XY_TilePropArray[index_X, index_Y].NW_Neighbor = XY_TileArray[(index_X - 1), (index_Y + 1)];
                XY_TilePropArray[index_X, index_Y].individualNeighborsList[4] = XY_TilePropArray[(index_X - 1), (index_Y + 1)];
            }
        }
    }

    void Find_SE(int index_X, int index_Y)
    {
        XY_TilePropArray[index_X, index_Y].individualNeighborsList.Add(null); // since SouthEast will ALWAYS be 5 in this list, then I can hardcode it for reference ;)
        if (index_X < gridX - 1) //NW neighbor finder
        {
            if (index_X % 2 == 0) //if evens
            {
                if (index_Y > 0)
                {
                    XY_TilePropArray[index_X, index_Y].SE_Neighbor = XY_TileArray[(index_X + 1), (index_Y - 1)];
                    XY_TilePropArray[index_X, index_Y].individualNeighborsList[5] = XY_TilePropArray[(index_X + 1), (index_Y - 1)];
                }

            }
            else //if odds
            {
                XY_TilePropArray[index_X, index_Y].SE_Neighbor = XY_TileArray[(index_X + 1), index_Y];
                XY_TilePropArray[index_X, index_Y].individualNeighborsList[5] = XY_TilePropArray[(index_X + 1), index_Y];
            }
        }
    }

    void SetMines_Medium()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //West Mine Lower
        tempGO = XY_TileArray[0, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Mine Upper
        tempGO = XY_TileArray[0, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Mine Lower
        tempGO = XY_TileArray[gridX - 1, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Mine Upper
        tempGO = XY_TileArray[gridX - 1, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine W
        tempGO = XY_TileArray[3, 12];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine E
        tempGO = XY_TileArray[4, 13];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Mine W
        tempGO = XY_TileArray[10, 13];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine E
        tempGO = XY_TileArray[11, 12];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Mine W
        tempGO = XY_TileArray[3, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Mine E
        tempGO = XY_TileArray[4, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Mine W
        tempGO = XY_TileArray[10, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Mine E
        tempGO = XY_TileArray[11, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }

    void SetMines_Small()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //West Mine Lower
        tempGO = XY_TileArray[0, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Mine Upper
        tempGO = XY_TileArray[0, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Mine Lower
        tempGO = XY_TileArray[10, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Mine Upper
        tempGO = XY_TileArray[10, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine W
        tempGO = XY_TileArray[2, 9];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine E
        tempGO = XY_TileArray[3, 9];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Mine W
        tempGO = XY_TileArray[7, 9];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Mine E
        tempGO = XY_TileArray[8, 9];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Mine W
        tempGO = XY_TileArray[2, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Mine E
        tempGO = XY_TileArray[3, 1];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Mine W
        tempGO = XY_TileArray[7, 1];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Mine E
        tempGO = XY_TileArray[8, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Mine";
        tileProps.tType = tileType.Mine;
        tileProps.Srend.color = new Color(1f, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }

    void SetQuarries_Small()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //North Quarry
        tempGO = XY_TileArray[5, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //South Quarry
        tempGO = XY_TileArray[5, 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Quarry
        tempGO = XY_TileArray[7, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Quarry
        tempGO = XY_TileArray[3, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Quarry
        tempGO = XY_TileArray[7, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Quarry
        tempGO = XY_TileArray[3, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Quarry";
        tileProps.tType = tileType.Quarry;
        tileProps.Srend.color = new Color(1f, 0.5f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }

    void SetSanctuaries_Small()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //NW Sanctuary
        tempGO = XY_TileArray[4, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Sanctuary
        tempGO = XY_TileArray[6, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Sanctuary
        tempGO = XY_TileArray[3, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Sanctuary
        tempGO = XY_TileArray[7, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Sanctuary
        tempGO = XY_TileArray[4, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Sanctuary
        tempGO = XY_TileArray[6, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Sanctuary";
        tileProps.tType = tileType.Sanctuary;
        tileProps.Srend.color = new Color(0, 1f, 0);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }

    void SetMidland_Small()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //North Midland
        tempGO = XY_TileArray[5, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //South Midland
        tempGO = XY_TileArray[5, 4];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Midland
        tempGO = XY_TileArray[6, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Midland
        tempGO = XY_TileArray[4, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Midland
        tempGO = XY_TileArray[6, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Midland
        tempGO = XY_TileArray[4, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Midland";
        tileProps.tType = tileType.Midland;
        tileProps.Srend.color = new Color(0.5f, 0, 0.5f);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }

    void SetLowland_Small()
    {
        GameObject tempGO;
        Tile_Properties tileProps;

        //NE Outer Lowland
        tempGO = XY_TileArray[3, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Outer Lowland
        tempGO = XY_TileArray[7, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Outer Lowland
        tempGO = XY_TileArray[1, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Outer Lowland
        tempGO = XY_TileArray[9, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Outer Lowland
        tempGO = XY_TileArray[3, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Outer Lowland
        tempGO = XY_TileArray[7, 2];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Left Lowland
        tempGO = XY_TileArray[3, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NE Right Lowland
        tempGO = XY_TileArray[4, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Left Lowland
        tempGO = XY_TileArray[6, 8];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //NW Right Lowland
        tempGO = XY_TileArray[7, 7];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Top Lowland
        tempGO = XY_TileArray[2, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        //  tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //East Bottom Lowland
        tempGO = XY_TileArray[2, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Top Lowland
        tempGO = XY_TileArray[8, 6];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //West Bottom Lowland
        tempGO = XY_TileArray[8, 5];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        // tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Left Lowland
        tempGO = XY_TileArray[3, 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SW Right Lowland
        tempGO = XY_TileArray[4, 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Left Lowland
        tempGO = XY_TileArray[6, 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;

        //SE Right Lowland
        tempGO = XY_TileArray[7, 3];
        tileProps = tempGO.GetComponent<Tile_Properties>();
        tempGO.tag = "Lowland";
        tileProps.tType = tileType.Lowland;
        tileProps.Srend.color = new Color(1f, 1f, 1f);
        tileProps.tileDefaultColor = tileProps.Srend.color;
    }
}

// Leo Levain's Formula
// currentTileListIndex = (GetCursorZ() * g.gridX) + GetCursorX(); 
