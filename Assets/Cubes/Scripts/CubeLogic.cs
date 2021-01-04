﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubes
{
    public class Cube
    {
        public Vector3Int id;

        public Vector3Int position;

        public Vector3Int shifr;

        public Color Color;

        public Trap trap;
    }

    public class ICubeLogic
    {

    }


    public class CubeLogic : MonoBehaviour
    {
        protected Cube cube;

        public Light lightt;
        public List<GameObject> walls;

        private int CountPlayers = 0;
        private float TimeWait = 10;
        private float TimeLost = 0;

        public void IntCube(object cubee) // Cube
        {
            cube = (Cube)cubee;
            int index = 0;
            foreach (var item in walls)
            {
                Wall wall = new Wall();
                wall.id = index;
                wall.number = cube.id;
                item.SendMessage("IntWall", (object)wall);
                index++;
            }

            lightt.color = cube.Color;
        }

        public void OpenDoor(object indexDoor) // int
        {
            walls[(int)indexDoor].SendMessage("ToOpenDoor");
        }

        public void CloseDoor(object indexDoor) // int
        {
            walls[(int)indexDoor].SendMessage("ToCloseDoor");
        }

        public void CloseAllDoor()
        {
            foreach (var item in walls)
            {
                item.SendMessage("ToCloseDoor");
            }
        }

        public void EventOpenedDoor(object indexwall) // int
        {
            var argument = new Vector4();
            argument.x = cube.position.x;
            argument.y = cube.position.y;
            argument.z = cube.position.z;
            argument.w = (int)indexwall;
            transform.parent.gameObject.SendMessage("EventOpenedDoor", (object)argument);
        }

        public void EventClosedDoor(object indexwall) // int
        {
            var argument = new Vector4();
            argument.x = cube.position.x;
            argument.y = cube.position.y;
            argument.z = cube.position.z;
            argument.w = (int)indexwall;

            if (CountPlayers == 0)
            {
                transform.parent.gameObject.SendMessage("DeactivateCube", (object)argument);
                CloseAllDoor();
                gameObject.SetActive(false);
                TimeLost = 0;
            }
            else
            {
                transform.parent.gameObject.SendMessage("EventClosedDoor", (object)argument);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                ++CountPlayers;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                --CountPlayers;
            }
        }

        public void Update()
        {
            if (CountPlayers == 0)
            {
                TimeLost += Time.deltaTime;
                if (TimeWait <= TimeLost)
                {
                    var argument = new Vector3Int();
                    argument.x = cube.position.x;
                    argument.y = cube.position.y;
                    argument.z = cube.position.z;
                    transform.parent.gameObject.SendMessage("DeactivateCube", (object)argument);
                    CloseAllDoor();
                    gameObject.SetActive(false);
                    TimeLost = 0;
                }
            }
        }
    }
}