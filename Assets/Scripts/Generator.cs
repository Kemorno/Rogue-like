using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

    public GameObject oRoom;//origin room
    public GameObject[] ARooms;
    public GameObject[] BRooms;
    public GameObject[] CRooms;
    public GameObject[] DRooms;

    public GameObject[] eRooms;

    bool gCreated = false;

    public GameObject Jointpf;
    public GameObject empty;

    GameObject gARooms;
    GameObject gBRooms;
    GameObject gCRooms;
    GameObject gDRooms;

     int stage;

    [Range(1,32)]//(mininum rooms, maximum rooms)
    public int maxRooms = 1;

    [Range(1, 16)]
    public int maxExtraRooms = 1;

    public float chanceFront;
    public float chanceRight;

    public float roomSize;

    public List<Vector3> CreatedORoom;
    public List<Vector3> CreatedARooms;
    public List<Vector3> CreatedBRooms;
    public List<Vector3> CreatedCRooms;
    public List<Vector3> CreatedDRooms;

    public List<Vector3> CreatedExtraARooms;
    public List<Vector3> CreatedExtraBRooms;
    public List<Vector3> CreatedExtraCRooms;
    public List<Vector3> CreatedExtraDRooms;

    [Range(0,2)]
    public float waitTime;

    public string Seed;//seed, 8 alphanumeric caracters formated like "XXXX XXXX"

    const string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    void Start () {

        if(Seed.Length != 8)
        {
            Seed = "";
        }
        else
        {

            string Seedf1 = Seed.Substring(0, 4);

            string Seedf2 = Seed.Substring(4, 4);

            string Seedf = Seedf1 + " " + Seedf2;

            Seedf = Seedf.ToUpper();

            Debug.Log(Seedf);

        }

        if(Seed == "")
        {

            for (int i = 1; i <= 8; i++)
            {

                Seed += glyphs[Random.Range(0, glyphs.Length)];

            }

            string Seedf1 = Seed.Substring(0, 4);

            string Seedf2 = Seed.Substring(4, 4);

            string Seedf = Seedf1 + " " + Seedf2;

            Debug.Log(Seedf);
        }

        StartCoroutine("Movement");

	}
	
	void Update () {
        

	}

    IEnumerator Movement()
    {

        Debug.Log("# of rooms " + maxRooms * 4);
        Debug.Log("# of Extra rooms " + maxExtraRooms * 4);

        bool rolledSide = false;

        for (int i = 0; i < (maxRooms * 4) + 1; i++)
        {
            

            int roomType= -1;

            if (i == 0)
            {
                transform.position = new Vector2(0, 0);

            }
            
            else if (0 < i && i <= (maxRooms * 4))
            {
               
                if (i == maxRooms+1 || i == maxRooms * 2 +1 || i == maxRooms * 3+1)
                {


                    transform.position = new Vector2(0,0);
                }


                if (i <= maxRooms)
                {

                    roomType = 0;

                    if (i > 3)
                    {

                        int mainDir = Random.Range(0, 100);
                        if(rolledSide)
                        {
                            mainDir = 1;
                        }

                        if (mainDir <= chanceFront)
                        {

                            transform.Translate(0, roomSize, 0);

                            rolledSide = false;
                        }

                        else if (mainDir <= chanceRight)
                        {

                            transform.Translate(-roomSize, 0, 0);

                            rolledSide = true;
                        }

                        else
                        {

                            transform.Translate(roomSize, 0, 0);

                            rolledSide = true;
                        }
                    }//other rooms
                    else
                    {
                        transform.Translate(0, roomSize, 0);
                    }//1st room

                }//first row

                else if (i <= maxRooms*2)
                {

                    roomType = 1;

                    if (i > maxRooms+3)
                    {

                        int mainDir = Random.Range(0, 100);

                        if (rolledSide)
                        {
                            mainDir = 1;
                        }

                        if (mainDir <= chanceFront)
                        {
                            

                            transform.Translate(roomSize, 0, 0);

                            rolledSide = false;
                        }

                        else if (mainDir <= chanceRight)
                        {

                            transform.Translate(0, -roomSize, 0);

                            rolledSide = true;
                        }

                        else
                        {

                            transform.Translate(0, roomSize, 0);

                            rolledSide = true;
                        }
                    }//other rooms
                    else
                    {
                        

                        transform.Translate(roomSize, 0, 0);
                    }//1st room
                }//second row

                else if (i <= maxRooms*3)
                {
                    roomType = 2;

                    if (i > maxRooms*2+3)
                    {

                        int mainDir = Random.Range(0, 100);

                        if (rolledSide)
                        {
                            mainDir = 1;
                        }
                        if (mainDir <= chanceFront)
                        {
                            

                            transform.Translate(0, -roomSize, 0);

                            rolledSide = false;
                        }

                        else if (mainDir <= chanceRight)
                        {

                            transform.Translate(roomSize, 0, 0);

                            rolledSide = true;
                        }

                        else
                        {

                            transform.Translate(-roomSize, 0, 0);

                            rolledSide = true;
                        }
                    }//other rooms
                    else
                    {
                        

                        transform.Translate(0, -roomSize, 0);
                    }//1st room

                }//third row

                else if (i <= maxRooms*4)
                {

                    roomType = 3;

                    if (i > maxRooms*3+3)
                    {

                        int mainDir = Random.Range(0, 100);

                        if (rolledSide)
                        {
                            mainDir = 1;
                        }
                        if (mainDir <= chanceFront)
                        {
                            

                            transform.Translate(-roomSize, 0, 0);

                            rolledSide = false;
                        }

                        else if (mainDir <= chanceRight)
                        {

                            transform.Translate(0, roomSize, 0);

                            rolledSide = true;
                        }

                        else
                        {

                            transform.Translate(0, -roomSize, 0);

                            rolledSide = true;
                        }
                    }//other rooms
                    else
                    {
                        

                        transform.Translate(-roomSize, 0, 0);
                    }//1st room

                }//fourth row
            }

            if (CreatedARooms.Contains(transform.position) || CreatedBRooms.Contains(transform.position) || CreatedCRooms.Contains(transform.position) || CreatedDRooms.Contains(transform.position) || CreatedORoom.Contains(transform.position))
            {
                i--;
            }//check for overlap
            else
            {
                Creation(roomType, i);
            }//creation

            yield return new WaitForSeconds(waitTime);
        }
        

        for (int i = 1; i <= (maxExtraRooms * 4); i++)
        {
            int roomType = -1;

            if (i <= maxRooms * 0.5f)
            {
                roomType = 0;
                
                int eRoomp = Random.Range(3, maxRooms-1);

                Vector3 oPoint = CreatedARooms[eRoomp];


                int rDir = Random.Range(0, 3);

                transform.position = oPoint;

                switch (rDir)
                {

                    case 0:
                        transform.Translate(roomSize, 0, 0);

                        if (CreatedARooms.Contains(transform.position) || CreatedExtraARooms.Contains(transform.position))
                        {
                            i--;
                        }
                        else
                        {
                            eCreation(i, roomType);
                        }
                        break;
                    case 1:
                        transform.Translate(-roomSize, 0, 0);

                        if (CreatedARooms.Contains(transform.position) || CreatedExtraARooms.Contains(transform.position))
                        {
                            i--;
                        }
                        else
                        {
                            eCreation(i, roomType);
                        }
                        break;
                    case 2:
                        transform.Translate(0, roomSize, 0);

                        if (CreatedARooms.Contains(transform.position) || CreatedExtraARooms.Contains(transform.position))
                        {
                            i--;
                        }
                        else
                        {
                            eCreation(i, roomType);
                        }
                        break;
                    case 3:
                        transform.Translate(0, -roomSize, 0);

                        if (CreatedARooms.Contains(transform.position) || CreatedExtraARooms.Contains(transform.position))
                        {
                            i--;
                        }
                        else
                        {
                            eCreation(i, roomType);
                        }
                        break;

                }
            }

            else if (i <= maxRooms * 1f)
            {
                roomType = 1;

                int eRoomp = Random.Range(3, maxRooms - 1);

                Vector3 oPoint = CreatedBRooms[eRoomp];


                int rDir = Random.Range(0, 3);

                transform.position = oPoint;

                switch (rDir)
                {

                    case 0:
                        transform.Translate(roomSize, 0, 0);

                        if (CreatedBRooms.Contains(transform.position) || CreatedExtraBRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 1:
                        transform.Translate(-roomSize, 0, 0);

                        if (CreatedBRooms.Contains(transform.position) || CreatedExtraBRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 2:
                        transform.Translate(0, roomSize, 0);

                        if (CreatedBRooms.Contains(transform.position) || CreatedExtraBRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 3:
                        transform.Translate(0, -roomSize, 0);

                        if (CreatedBRooms.Contains(transform.position) || CreatedExtraBRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }

                }
            }

            else if (i <= maxRooms * 1.5f)
            {
                roomType = 2;

                int eRoomp = Random.Range(3, maxRooms - 1);

                Vector3 oPoint = CreatedCRooms[eRoomp];


                int rDir = Random.Range(0, 3);

                transform.position = oPoint;

                switch (rDir)
                {

                    case 0:
                        transform.Translate(roomSize, 0, 0);

                        if (CreatedCRooms.Contains(transform.position) || CreatedExtraCRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 1:
                        transform.Translate(-roomSize, 0, 0);

                        if (CreatedCRooms.Contains(transform.position) || CreatedExtraCRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 2:
                        transform.Translate(0, roomSize, 0);

                        if (CreatedCRooms.Contains(transform.position) || CreatedExtraCRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 3:
                        transform.Translate(0, -roomSize, 0);

                        if (CreatedCRooms.Contains(transform.position) || CreatedExtraCRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }

                }
            }

            else if(i <= maxRooms * 2)
            {
                roomType = 3;

                int eRoomp = Random.Range(3, maxRooms - 1);

                Vector3 oPoint = CreatedDRooms[eRoomp];


                int rDir = Random.Range(0, 3);

                transform.position = oPoint;

                switch (rDir)
                {

                    case 0:
                        transform.Translate(roomSize, 0, 0);

                        if (CreatedDRooms.Contains(transform.position) || CreatedExtraDRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 1:
                        transform.Translate(-roomSize, 0, 0);

                        if (CreatedDRooms.Contains(transform.position) || CreatedExtraDRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 2:
                        transform.Translate(0, roomSize, 0);

                        if (CreatedDRooms.Contains(transform.position) || CreatedExtraDRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }
                    case 3:
                        transform.Translate(0, -roomSize, 0);

                        if (CreatedDRooms.Contains(transform.position) || CreatedExtraDRooms.Contains(transform.position))
                        {
                            i--;
                            break;
                        }
                        else
                        {
                            eCreation(i, roomType);
                            break;
                        }

                }

            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    void Creation(int roomType, int i)
    {


    GameObject iJoint;
    GameObject cRoom;
        if (!gCreated)
        {
            gARooms = Instantiate(empty);
            gARooms.name = "A Rooms";
            gBRooms = Instantiate(empty);
            gBRooms.name = "B Rooms";
            gCRooms = Instantiate(empty);
            gCRooms.name = "C Rooms";
            gDRooms = Instantiate(empty);
            gDRooms.name = "D Rooms";

            gCreated = true;
        }

        if (roomType == 0)
        {
            int RroomType;

            RroomType = Random.Range(0, ARooms.Length);

            cRoom = Instantiate(ARooms[RroomType], transform.position, transform.rotation);
            CreatedARooms.Add(cRoom.transform.position);
            
            if(i > 1)
            {
                float movX = 0;
                float movY = 0;

                if(CreatedARooms[i - 1].x < CreatedARooms[i - 2].x)
                {
                    movX = -roomSize * 0.5f;
                }
                if (CreatedARooms[i - 1].x > CreatedARooms[i - 2].x)
                {
                    movX = roomSize * 0.5f;

                }

                if (CreatedARooms[i - 1].y < CreatedARooms[i - 2].y)
                {
                    movY = -roomSize * 0.5f;
                }
                if (CreatedARooms[i - 1].y > CreatedARooms[i - 2].y)
                {
                    movY = roomSize * 0.5f;

                }

                iJoint = Instantiate(Jointpf, new Vector3(CreatedARooms[i - 2].x + movX, CreatedARooms[i - 2].y + movY, 0), transform.rotation);
                iJoint.transform.parent = cRoom.transform;
            }

            cRoom.name = "A Room #" +i;
            cRoom.transform.parent = gARooms.transform;
        }

        else if (roomType == 1)
        {
            int RroomType;

            RroomType = Random.Range(0, BRooms.Length);

            cRoom = Instantiate(BRooms[RroomType], transform.position, transform.rotation);
            CreatedBRooms.Add(cRoom.transform.position);

            if (i > maxRooms+1)
            {
                float movX = 0;
                float movY = 0;

                if (CreatedBRooms[i - (1 + maxRooms)].x < CreatedBRooms[i - (2 + maxRooms)].x)
                {
                    movX = -roomSize * 0.5f;
                }
                if (CreatedBRooms[i - (1 + maxRooms)].x > CreatedBRooms[i - (2 + maxRooms)].x)
                {
                    movX = roomSize * 0.5f;
                }

                if (CreatedBRooms[i - (1 + maxRooms)].y < CreatedBRooms[i - (2 + maxRooms)].y)
                {
                    movY = -roomSize * 0.5f;
                }
                if (CreatedBRooms[i - (1 + maxRooms)].y > CreatedBRooms[i - (2 + maxRooms)].y)
                {
                    movY = roomSize * 0.5f;
                }

                iJoint = Instantiate(Jointpf, new Vector3(CreatedBRooms[i - (2 + maxRooms)].x + movX, CreatedBRooms[i - (2 + maxRooms)].y + movY, 0), transform.rotation);
                iJoint.transform.parent = cRoom.transform;
            }
            cRoom.name = "B Room #" + i;
            cRoom.transform.parent = gBRooms.transform;
        }

        else if (roomType == 2)
        {
            int RroomType;

            RroomType = Random.Range(0, CRooms.Length);

            cRoom = Instantiate(CRooms[RroomType], transform.position, transform.rotation);
            CreatedCRooms.Add(cRoom.transform.position);

            if (i > maxRooms*2 + 1)
            {
                float movX = 0;
                float movY = 0;

                if (CreatedCRooms[i - (1 + maxRooms*2)].x < CreatedCRooms[i - (2 + maxRooms*2)].x)
                {
                    movX = -roomSize * 0.5f;
                }
                if (CreatedCRooms[i - (1 + maxRooms*2)].x > CreatedCRooms[i - (2 + maxRooms*2)].x)
                {
                    movX = roomSize * 0.5f;
                }

                if (CreatedCRooms[i - (1 + maxRooms*2)].y < CreatedCRooms[i - (2 + maxRooms*2)].y)
                {
                    movY = -roomSize * 0.5f;
                }
                if (CreatedCRooms[i - (1 + maxRooms*2)].y > CreatedCRooms[i - (2 + maxRooms*2)].y)
                {
                    movY = roomSize * 0.5f;
                }

                iJoint = Instantiate(Jointpf, new Vector3(CreatedCRooms[i - (2 + maxRooms*2)].x + movX, CreatedCRooms[i - (2 + maxRooms*2)].y + movY, 0), transform.rotation);
                iJoint.transform.parent = cRoom.transform;
            }
            cRoom.name = "C Room #" + i;
            cRoom.transform.parent = gCRooms.transform;
        }

        else if (roomType == 3)
        {
            int RroomType;

            RroomType = Random.Range(0, DRooms.Length);

            cRoom = Instantiate(DRooms[RroomType], transform.position, transform.rotation);
            CreatedDRooms.Add(cRoom.transform.position);

            if (i > maxRooms * 3 + 1)
            {
                float movX = 0;
                float movY = 0;

                if (CreatedDRooms[i - (1 + maxRooms * 3)].x < CreatedDRooms[i - (2 + maxRooms * 3)].x)
                {
                    movX = -roomSize * 0.5f;
                }
                if (CreatedDRooms[i - (1 + maxRooms * 3)].x > CreatedDRooms[i - (2 + maxRooms * 3)].x)
                {
                    movX = roomSize * 0.5f;
                }

                if (CreatedDRooms[i - (1 + maxRooms * 3)].y < CreatedDRooms[i - (2 + maxRooms * 3)].y)
                {
                    movY = -roomSize * 0.5f;
                }
                if (CreatedDRooms[i - (1 + maxRooms * 3)].y > CreatedDRooms[i - (2 + maxRooms * 3)].y)
                {
                    movY = roomSize * 0.5f;
                }

                iJoint = Instantiate(Jointpf, new Vector3(CreatedDRooms[i - (2 + maxRooms * 3)].x + movX, CreatedDRooms[i - (2 + maxRooms * 3)].y + movY, 0), transform.rotation);
                iJoint.transform.parent = cRoom.transform;
            }
            cRoom.name = "D Room #" + i;
            cRoom.transform.parent = gDRooms.transform;
        }

        else
        {
            cRoom = Instantiate(oRoom, transform.position, transform.rotation);

            cRoom.name = "Origin Room";
            CreatedORoom.Add(cRoom.transform.position);
            Instantiate(Jointpf, new Vector3(transform.position.x + roomSize * 0.5f, transform.position.y, 0), transform.rotation);
            Instantiate(Jointpf, new Vector3(transform.position.x, transform.position.y - roomSize * 0.5f, 0), transform.rotation);
            Instantiate(Jointpf, new Vector3(transform.position.x - roomSize * 0.5f, transform.position.y, 0), transform.rotation);
            Instantiate(Jointpf, new Vector3(transform.position.x, transform.position.y + roomSize * 0.5f, 0), transform.rotation);
            cRoom.name = "Room #" + i;
        }
    }

    void eCreation(int i, int roomType)
    {
        if (roomType == 0)
        {
            int reRooms = Random.Range(0, eRooms.Length);

            GameObject cRoom = Instantiate(eRooms[reRooms], transform.position, transform.rotation);
            CreatedExtraARooms.Add(cRoom.transform.position);
            cRoom.name = "Extra Room A #" + i;
            cRoom.transform.parent = gARooms.transform;
        }
        if (roomType == 1)
        {
            int reRooms = Random.Range(0, eRooms.Length);

            GameObject cRoom = Instantiate(eRooms[reRooms], transform.position, transform.rotation);
            CreatedExtraBRooms.Add(cRoom.transform.position);
            cRoom.name = "Extra Room B #" + i;
            cRoom.transform.parent = gBRooms.transform;
        }
        if (roomType == 2)
        {
            int reRooms = Random.Range(0, eRooms.Length);

            GameObject cRoom = Instantiate(eRooms[reRooms], transform.position, transform.rotation);
            CreatedExtraCRooms.Add(cRoom.transform.position);
            cRoom.name = "Extra Room C #" + i;
            cRoom.transform.parent = gCRooms.transform;
        }
        if (roomType == 3)
        {
            int reRooms = Random.Range(0, eRooms.Length);

            GameObject cRoom = Instantiate(eRooms[reRooms], transform.position, transform.rotation);
            CreatedExtraDRooms.Add(cRoom.transform.position);
            cRoom.name = "Extra Room D #" + i;
            cRoom.transform.parent = gDRooms.transform;
        }

    }
}
