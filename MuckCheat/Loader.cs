using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace MuckCheat
{
    public class Loader
    {
        public static GameObject Load;
        public static void Init()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<Cheats>();
            //Loader.Load.AddComponent<DropDown>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

    }
}