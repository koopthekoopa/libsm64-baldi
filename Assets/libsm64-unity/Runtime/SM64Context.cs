using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LibSM64
{
    public class SM64Context : MonoBehaviour
    {
        static SM64Context s_instance = null;

        List<SM64Mario> _marios = new List<SM64Mario>();
        List<SM64DynamicTerrain> _surfaceObjects = new List<SM64DynamicTerrain>();

        private const int bufferSize = 544 * 2 * 2;
        private int bufferPosition = bufferSize;
        private readonly short[] sm64AudioBuffer = new short[bufferSize];
        private readonly float[] audioSourceBuffer = new float[bufferSize];
        private static byte[] romData;
        
        static string romPref = "SM64BaseRom";

        public static bool CheckRom()
        {
            return PlayerPrefs.HasKey(romPref) && File.Exists(PlayerPrefs.GetString(romPref));
        }
        
        public static void OpenRom()
        {
            if (!CheckRom() || romData != null)
            {
                return;
            }
            romData = File.ReadAllBytes(PlayerPrefs.GetString(romPref));
        }

        public static void SetNewRom(string newFile)
        {
            PlayerPrefs.SetString(romPref, newFile);
            //OpenRom();
        }

        void Awake()
        {
            OpenRom();

            Interop.GlobalInit( romData );
            //Interop.GlobalInit( File.ReadAllBytes( PlayerPrefs.GetString( "Baserom" ) ));
            RefreshStaticTerrain();

            // Setup audio source
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f;

            audioSource.pitch = 0.665f;

            audioSource.loop = true;
            audioSource.Play();
        }

        void Update()
        {
            foreach( var o in _surfaceObjects )
                o.contextUpdate();

            foreach( var o in _marios )
                o.contextUpdate();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            int remaining = data.Length;
            while (remaining > 0)
            {
                // Play the data!
                int copyLength = Math.Min(remaining, bufferSize - bufferPosition);
                Array.Copy(audioSourceBuffer, bufferPosition, data, data.Length - remaining, copyLength);

                // Next!
                bufferPosition += copyLength;
                remaining -= copyLength;

                // Write SM64 audio data to audio source data.
                if (bufferPosition >= bufferSize)
                {
                    Interop.AudioTick(sm64AudioBuffer, bufferSize);
                    for (int i = 0; i < bufferSize; i++)
                    {
                        audioSourceBuffer[i] = Math.Min((float)sm64AudioBuffer[i] / short.MaxValue, 1f);
                    }
                    bufferPosition = 0;
                }
            }
        }

        void FixedUpdate()
        {
            foreach( var o in _surfaceObjects )
                o.contextFixedUpdate();

            foreach( var o in _marios )
                o.contextFixedUpdate();
        }

        void OnApplicationQuit()
        {
            Interop.GlobalTerminate();
            s_instance = null;
        }

        static void ensureInstanceExists()
        {
            if( s_instance == null )
            {
                var contextGo = new GameObject( "SM64_CONTEXT" );
                contextGo.hideFlags |= HideFlags.HideInHierarchy;
                s_instance = contextGo.AddComponent<SM64Context>();
            }
        }

        static public void RefreshStaticTerrain()
        {
            Interop.StaticSurfacesLoad( Utils.GetAllStaticSurfaces());
            
            foreach( var obj in GameObject.FindObjectsOfType<SM64StaticTerrain>())
            {
                if (obj.GetComponent<SM64StaticTerrain>().ExclusiveToMario)
                {
                    obj.GetComponent<MeshCollider>().enabled = false;
                }
            }
        }

        static public void RegisterMario( SM64Mario mario )
        {
            ensureInstanceExists();

            if( !s_instance._marios.Contains( mario ))
                s_instance._marios.Add( mario );
        }

        static public void UnregisterMario( SM64Mario mario )
        {
            if( s_instance != null && s_instance._marios.Contains( mario ))
                s_instance._marios.Remove( mario );
        }

        static public void RegisterSurfaceObject( SM64DynamicTerrain surfaceObject )
        {
            ensureInstanceExists();

            if( !s_instance._surfaceObjects.Contains( surfaceObject ))
                s_instance._surfaceObjects.Add( surfaceObject );
        }

        static public void UnregisterSurfaceObject( SM64DynamicTerrain surfaceObject )
        {
            if( s_instance != null && s_instance._surfaceObjects.Contains( surfaceObject ))
                s_instance._surfaceObjects.Remove( surfaceObject );
        }
    }
}