using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace LibSM64
{
    internal static class Interop
    {
        public const float SCALE_FACTOR = 50;

        public const int SM64_TEXTURE_WIDTH  = 64 * 11;
        public const int SM64_TEXTURE_HEIGHT = 64;
        public const int SM64_GEO_MAX_TRIANGLES = 4096;

        public const int SM64_MAX_HEALTH = 8;

        private static readonly object _lock = new();

        private static Vector3 UnityWorldToMario(this Vector3 pos)
        {
            return SCALE_FACTOR * Vector3.Scale(pos, new Vector3(-1, 1, 1));
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SM64Surface
        {
            public short type;
            public short force;
            public ushort terrain;
            public int v0x, v0y, v0z;
            public int v1x, v1y, v1z;
            public int v2x, v2y, v2z;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SM64MarioInputs
        {
            public float camLookX, camLookZ;
            public float stickX, stickY;
            public byte buttonA, buttonB, buttonZ;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SM64MarioState
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] velocity;
            public float faceAngle;
            public float forwardVelocity;
            public short health;
            public uint action;
            public int animID;
            public short animFrame;
            public uint flags;
            public uint particleFlags;
            public short invincTimer;

            public Vector3 unityPosition
            {
                get { return position != null ? new Vector3( -position[0], position[1], position[2] ) / SCALE_FACTOR : Vector3.zero; }
            }
            
            public Quaternion unityRotation
            {
                get { return Quaternion.Euler(0.0f, Mathf.Repeat((-Mathf.Rad2Deg * faceAngle) + 180.0f, 360.0f) - 180.0f, 0.0f); }
            }
            
            public bool hasWingCap
            {
                get { return (flags & (uint)SM64CapType.Wing) != 0; }
            }
            
            public bool hasMetalCap
            {
                get { return (flags & (uint)SM64CapType.Metal) != 0; }
            }
			
			// action flags
            
            public bool isAttacking
            {
                get { return (action & (uint)SM64ActionFlag.ACT_FLAG_ATTACKING) != 0; }
            }
            
            public bool isAir
            {
                get { return (action & (uint)SM64ActionFlag.ACT_FLAG_AIR) != 0; }
            }
			
			// action types
            
            public bool isLongJumping
            {
                get { return action == (uint)SM64ActionType.ACT_LONG_JUMP; }
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        struct SM64MarioGeometryBuffers
        {
            public IntPtr position;
            public IntPtr normal;
            public IntPtr color;
            public IntPtr uv;
            public ushort numTrianglesUsed;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct SM64ObjectTransform
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            float[] position;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            float[] eulerRotation;

            static public SM64ObjectTransform FromUnityWorld( Vector3 position, Quaternion rotation )
            {
                float[] vecToArr( Vector3 v )
                {
                    return new float[] { v.x, v.y, v.z };
                }

                float fmod( float a, float b )
                {
                    return a - b * Mathf.Floor( a / b );
                }
                
                float fixAngle( float a )
                {
                    return fmod( a + 180.0f, 360.0f ) - 180.0f;
                }

                var pos = SCALE_FACTOR * Vector3.Scale( position, new Vector3( -1, 1, 1 ));
                var rot = Vector3.Scale( rotation.eulerAngles, new Vector3( -1, 1, 1 ));

                rot.x = fixAngle( rot.x );
                rot.y = fixAngle( rot.y );
                rot.z = fixAngle( rot.z );

                return new SM64ObjectTransform {
                    position = vecToArr( pos ),
                    eulerRotation = vecToArr( rot )
                };
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        struct SM64SurfaceObject
        {
            public SM64ObjectTransform transform;
            public uint surfaceCount;
            public IntPtr surfaces;
        }

        /* GLOBAL */
        [DllImport("sm64")]
        static extern void sm64_global_init( IntPtr rom, IntPtr outTexture );
        [DllImport("sm64")]
        static extern void sm64_global_terminate();

        [DllImport("sm64")]
        static extern void sm64_register_debug_print_function( IntPtr debugPrintFunctionPtr );

        /* SURFACES */

        [DllImport("sm64")]
        static extern uint sm64_surface_object_create(ref SM64SurfaceObject surfaceObject);
        [DllImport("sm64")]
        static extern void sm64_surface_object_move(uint objectId, ref SM64ObjectTransform transform);
        [DllImport("sm64")]
        static extern void sm64_surface_object_delete(uint objectId);

        [DllImport("sm64")]
        static extern void sm64_static_surfaces_load( SM64Surface[] surfaces, ulong numSurfaces );

        /* AUDIO */

        [DllImport("sm64")]
        static extern void sm64_audio_init( IntPtr rom );
        [DllImport("sm64")]
        static extern uint sm64_audio_tick( uint numQueuedSamples, uint numDesiredSamples, IntPtr audio_buffer );

        /* MR RETRO HIMSELF */

        [DllImport("sm64")]
        static extern uint sm64_mario_create( float marioX, float marioY, float marioZ );
        [DllImport("sm64")]
        static extern void sm64_mario_tick( uint marioId, ref SM64MarioInputs inputs, ref SM64MarioState outState, ref SM64MarioGeometryBuffers outBuffers );
        [DllImport("sm64")]
        static extern void sm64_mario_delete( uint marioId );
        [DllImport("sm64")]
        static extern void sm64_set_mario_position( uint marioId, float x, float y, float z );
        [DllImport("sm64")]
        static extern void sm64_set_mario_velocity( uint marioId, float x, float y, float z );
        [DllImport("sm64")]
        static extern void sm64_set_mario_forward_velocity( uint marioId, float vel );

        /* ACTIONS */

        [DllImport("sm64")]
        static extern void sm64_set_mario_action( uint marioId, uint action );
        
        /* CAPS */

        [DllImport("sm64")]
        static extern void sm64_mario_interact_cap( uint marioId, uint capFlag, ushort capTime, byte playMusic );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DebugPrintFuncDelegate(string str);

        static public Texture2D marioTexture { get; private set; }
        static public bool isGlobalInit { get; private set; }

        /*static void debugPrintCallback(string str)
        {
            Debug.Log("libsm64: " + str);
        }*/

        public static void GlobalInit( byte[] rom )
        {
            //var callbackDelegate = new DebugPrintFuncDelegate( debugPrintCallback );
            var romHandle = GCHandle.Alloc( rom, GCHandleType.Pinned );
            var textureData = new byte[ 4 * SM64_TEXTURE_WIDTH * SM64_TEXTURE_HEIGHT ];
            var textureDataHandle = GCHandle.Alloc( textureData, GCHandleType.Pinned );

            sm64_global_init( romHandle.AddrOfPinnedObject(), textureDataHandle.AddrOfPinnedObject());
            // Spams and lags because of audio
            //sm64_register_debug_print_function( Marshal.GetFunctionPointerForDelegate( callbackDelegate ));
            sm64_audio_init( romHandle.AddrOfPinnedObject() );

            Color32[] cols = new Color32[ SM64_TEXTURE_WIDTH * SM64_TEXTURE_HEIGHT ];
            marioTexture = new Texture2D( SM64_TEXTURE_WIDTH, SM64_TEXTURE_HEIGHT );
            for( int ix = 0; ix < SM64_TEXTURE_WIDTH; ix++)
            for( int iy = 0; iy < SM64_TEXTURE_HEIGHT; iy++)
            {
                cols[ix + SM64_TEXTURE_WIDTH*iy] = new Color32(
                    textureData[4*(ix + SM64_TEXTURE_WIDTH*iy)+0],
                    textureData[4*(ix + SM64_TEXTURE_WIDTH*iy)+1],
                    textureData[4*(ix + SM64_TEXTURE_WIDTH*iy)+2],
                    textureData[4*(ix + SM64_TEXTURE_WIDTH*iy)+3]
                );
            }
            marioTexture.SetPixels32( cols );
            marioTexture.Apply();

            romHandle.Free();
            textureDataHandle.Free();

            isGlobalInit = true;
        }

        public static void GlobalTerminate()
        {
            lock (_lock)
            {
                sm64_global_terminate();
                marioTexture = null;
                isGlobalInit = false;
            }
        }

        public static void StaticSurfacesLoad( SM64Surface[] surfaces )
        {
            sm64_static_surfaces_load( surfaces, (ulong)surfaces.Length );
        }

        public static uint MarioCreate( Vector3 marioPos )
        {
            return sm64_mario_create( marioPos.x, marioPos.y, marioPos.z );
        }

        public static SM64MarioState MarioTick( uint marioId, SM64MarioInputs inputs, Vector3[] positionBuffer, Vector3[] normalBuffer, Vector3[] colorBuffer, Vector2[] uvBuffer )
        {
            SM64MarioState outState = new SM64MarioState();

            var posHandle = GCHandle.Alloc( positionBuffer, GCHandleType.Pinned );
            var normHandle = GCHandle.Alloc( normalBuffer, GCHandleType.Pinned );
            var colorHandle = GCHandle.Alloc( colorBuffer, GCHandleType.Pinned );
            var uvHandle = GCHandle.Alloc( uvBuffer, GCHandleType.Pinned );

            SM64MarioGeometryBuffers buff = new SM64MarioGeometryBuffers
            {
                position = posHandle.AddrOfPinnedObject(),
                normal = normHandle.AddrOfPinnedObject(),
                color = colorHandle.AddrOfPinnedObject(),
                uv = uvHandle.AddrOfPinnedObject()
            };

            lock (_lock)
                sm64_mario_tick(marioId, ref inputs, ref outState, ref buff);

            posHandle.Free();
            normHandle.Free();
            colorHandle.Free();
            uvHandle.Free();

            return outState;
        }

        public static void MarioDelete( uint marioId )
        {
            sm64_mario_delete( marioId );
        }

        public static uint AudioTick(short[] audioBuffer, uint numDesiredSamples, uint numQueuedSamples = 0)
        {
            lock (_lock)
            {
                GCHandle audioBufferPointer = GCHandle.Alloc(audioBuffer, GCHandleType.Pinned);
                var samples = sm64_audio_tick(numQueuedSamples, numDesiredSamples, audioBufferPointer.AddrOfPinnedObject());
                audioBufferPointer.Free();
                return samples;
            }
        }

        public static uint SurfaceObjectCreate( Vector3 position, Quaternion rotation, SM64Surface[] surfaces )
        {
            var surfListHandle = GCHandle.Alloc( surfaces, GCHandleType.Pinned );
            var t = SM64ObjectTransform.FromUnityWorld( position, rotation );

            SM64SurfaceObject surfObj = new SM64SurfaceObject
            {
                transform = t,
                surfaceCount = (uint)surfaces.Length,
                surfaces = surfListHandle.AddrOfPinnedObject()
            };

            uint result = sm64_surface_object_create( ref surfObj );

            surfListHandle.Free();

            return result;
        }

        public static void SurfaceObjectMove( uint id, Vector3 position, Quaternion rotation )
        {
            var t = SM64ObjectTransform.FromUnityWorld( position, rotation );
            sm64_surface_object_move( id, ref t );
        }

        public static void SurfaceObjectDelete( uint id )
        {
            sm64_surface_object_delete( id );
        }

        public static void SetMarioAction( uint id, SM64ActionType action )
        {
            sm64_set_mario_action( id, (uint)action );
        }

        public static void SetMarioPosition( uint id, Vector3 position )
        {
            Vector3 corrected = position.UnityWorldToMario();
            sm64_set_mario_position( id, corrected.x, corrected.y, corrected.z );
        }

        public static void SetMarioVelocity( uint id, Vector3 velocity )
        {
            Vector3 corrected = velocity.UnityWorldToMario();
            sm64_set_mario_velocity( id, corrected.x, corrected.y, corrected.z );
        }

        public static void SetMarioForwardVelocity( uint id, float velocity )
        {
            sm64_set_mario_forward_velocity( id, velocity );
        }

        public static void InteractCap( uint marioId, SM64CapType capFlag, ushort capTime, bool playMusic )
        {
            lock (_lock)
            {
                sm64_mario_interact_cap( marioId, (uint)capFlag, capTime, playMusic ? (byte)1 : (byte)0 );
            }
        }
    }
}
