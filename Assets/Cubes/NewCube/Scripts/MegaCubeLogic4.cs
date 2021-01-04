﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Cubes.CubeOne
{
    public class MegaCubeLogic4 : MonoBehaviour
    {
        public int _Size = 10;
        public float _WidhtCube = 2.6f;
        public GameObject prefabcube;

        public List<GameObject> Traps;
        public List<GameObject> Frames;

        private GameObject[,,] gamecubes;
        private Cube[,,] cubes;
        private GameObject CubeBridge;
        private GameObject player;

        void Start()
        {
            _Size = Cookie.room.Size;
            gamecubes = new GameObject[_Size, _Size, _Size];
            cubes = new Cube[_Size, _Size, _Size];

            //SetFrames();
            SetCubes();
            IntCubes();
            SetPlayers();

            //Set Cube of Bridght
            //SetCube(_Size+1, 5, 5);
        }

        #region Int
        public void IntPlayer(object player2)
        {
            player = (GameObject)player2;
        }

        private void SetFrames()
        {
            var value = ((_Size / 2) + 1) * _WidhtCube;
            Frames[0].transform.localPosition = new Vector3(0, -value, 0); // top
            Frames[1].transform.localPosition = new Vector3(0, 0, value); // left
            Frames[2].transform.localPosition = new Vector3(value, 0, 0);
            Frames[3].transform.localPosition = new Vector3(0, 0, -value); // right
            Frames[4].transform.localPosition = new Vector3(-value, 0, 0);
            Frames[5].transform.localPosition = new Vector3(0, value, 0); // down 
        }

        private void SetCubes()
        {
            for (int i = 0; i < _Size; ++i)
            {
                for (int j = 0; j < _Size; ++j)
                {
                    for (int l = 0; l < _Size; ++l)
                    {
                        var cubee = new Cube();
                        cubee.Color = Color.white;
                        cubee.id = new Vector3Int();
                        cubee.id.x = (int)Random.Range(300, 700);
                        cubee.id.y = (int)Random.Range(300, 700);
                        cubee.id.z = (int)Random.Range(300, 700);
                        cubee.position = new Vector3Int(i, j, l);
                        cubee.trap = SetTrap(cubee);

                        cubes[i, j, l] = cubee;
                    }
                }
            }
        }

        private void IntCubes()
        {
            for (int i = 0; i < _Size; ++i)
            {
                for (int j = 0; j < _Size; ++j)
                {
                    for (int l = 0; l < _Size; ++l)
                    {
                        gamecubes[i, j, l] = IntCube(cubes[i, j, l]);
                        if (cubes[i, j, l].trap != null)
                        {
                            Instantiate(Traps[cubes[i, j, l].trap.id], gamecubes[i, j, l].transform);
                        }
                    }
                }
            }
        }

        private GameObject IntCube(Cube cubedto)
        {
            var newcube = Instantiate(prefabcube, transform);
            newcube.transform.position = new Vector3(cubedto.position.x * _WidhtCube, cubedto.position.y * _WidhtCube, cubedto.position.z * _WidhtCube);
            newcube.SendMessage("IntCube", (object)cubedto);
            newcube.SetActive(false);
            return newcube;
        }

        private Trap SetTrap(Cube cubedto)
        {
            if (MathCube.IsSimpleNumber(cubedto.id.x) || MathCube.IsSimpleNumber(cubedto.id.y) || MathCube.IsSimpleNumber(cubedto.id.z))
            {
                int index = Random.Range(0, Traps.Count - 1);
                return new Trap() { id = index, name = Traps[index].name };
            }
            return null;
        }

        private void SetPlayers()
        {
            foreach (var item in Cookie.players)
            {
                int x = 0, y = 0, z = 0;
                do
                {
                    x = Random.Range(0, _Size);
                    y = Random.Range(0, _Size);
                    z = Random.Range(0, _Size);
                }
                while (cubes[x, y, z].trap != null);
                player.transform.localPosition = gamecubes[x, y, z].transform.localPosition;
                gamecubes[x, y, z].SetActive(true);
            }
        }

        #endregion

        #region Events
        public bool ActivateCube(Vector3Int oldposition, int oldwall, Vector3Int position, int wallnumber)
        {
            try
            {
                var actCube = gamecubes[position.x, position.y, position.z];
                actCube.SetActive(true);
                actCube.SendMessage("OpenDoor", wallnumber, SendMessageOptions.DontRequireReceiver);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void EventOpenedDoor(object vector4) // vector4 = position + indexwall
        {
            var result = (Vector4)vector4;
            var position = new Vector3Int((int)result.x, (int)result.y, (int)result.z);
            int indexwall = (int)result.w;
            var newposition = new Vector3Int(position.x, position.y, position.z);
            int newdoor = 0;

            newposition = GetCube(indexwall, position, out newdoor);
            ActivateCube(position, indexwall, newposition, newdoor);
        }

        private Vector3Int GetCube(int indexwall, Vector3Int position, out int newdoor)
        {
            Vector3Int newposition = new Vector3Int();
            newdoor = 0;
            if (indexwall == 0) { newposition = position + new Vector3Int(0, -1, 0); newdoor = 5; }
            else if (indexwall == 1) { newposition = position + new Vector3Int(0, 0, 1); newdoor = 3; }
            else if (indexwall == 2) { newposition = position + new Vector3Int(1, 0, 0); newdoor = 4; }
            else if (indexwall == 3) { newposition = position + new Vector3Int(0, 0, -1); newdoor = 1; }
            else if (indexwall == 4) { newposition = position + new Vector3Int(-1, 0, 0); newdoor = 2; }
            else if (indexwall == 5) { newposition = position + new Vector3Int(0, 1, 0); newdoor = 0; }
            return newposition;
        }

        public void DeactivateCube(object vector3) // vector3int
        {
            var position = (Vector3Int)vector3;
            Vector3Int newposition;
            int newdoor = 0;
            for (int i = 0; i < 6; ++i)
            {
                newposition = GetCube(i, position, out newdoor);
                CloseDoor(newposition, newdoor);
            }
        }

        private bool CloseDoor(Vector3Int position, int wallnumber)
        {
            try
            {
                var actCube = gamecubes[position.x, position.y, position.z];
                actCube.SendMessage("CloseDoor", wallnumber, SendMessageOptions.DontRequireReceiver);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}