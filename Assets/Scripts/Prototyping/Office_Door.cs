using amogus;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;

namespace amogus
{
    public class Office_Door : MonoBehaviour
    {
        [SerializeField] private Door Door;
        public GameObject door;
        private bool open = false;

        void Start()
        {
           
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O) && open != true)
            {
                Door.Open();
                open = true;
            }
        }
    }
}
