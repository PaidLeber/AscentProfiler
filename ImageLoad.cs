﻿using System;
using UnityEngine;
using KSP.IO;



namespace AscentProfiler
{


        class ImageLoader
        {



            private ImageLoader()
            {



            }



            public static Texture2D GetTexture(String pathInGameData)
            {

                Debug.Log("get texture " + pathInGameData);

                Texture2D texture = GameDatabase.Instance.GetTexture(pathInGameData, false);

                if (texture != null)
                {

                    return texture;

                }

                else
                {

                    Debug.Log("texture " + pathInGameData + " not found");

                    return null;

                }

            }



        }

}