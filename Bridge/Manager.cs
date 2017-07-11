using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Bridge
{
    /// <summary>
    /// Data structure between unreal and manager
    /// </summary>
    public struct UnrealOutData         // Unreal --> Manager (Recving)
    {
        ///
        public float objectPositionX;
        public float objectPositionY;
        public float objectPositionZ;
    }

    public struct UnrealInData         // Manager --> Unreal (Sending)
    {
        ///
        public float currentHIPX;
        public float currentHIPY;
        public float currentHIPZ;
    }

    /// <summary>
    /// Data structure between Chai3D and manager
    /// </summary>
    public struct CHAI3DOutData         // CHAI3D --> Manager (Recving)
    {
        ///
        public float currentHIPX;
        public float currentHIPY;
        public float currentHIPZ;
    }

    public struct CHAI3DInData         // Manager --> CHAI3D (Sending)
    {
        ///
        public float objectPositionX;
        public float objectPositionY;
        public float objectPositionZ;
    }

    public class Manager
    {
        /* // for Unreal
        CHAI3DInData uinData;
        IntPtr uinFileMap;
        IntPtr uinMapAddress;

        CHAI3DOutData uoutData;
        IntPtr uoutFileMap;
        IntPtr uoutMapAddress;
        */

        // for CHAI3D
        CHAI3DInData cinData;
        IntPtr cinFileMap;
        IntPtr cinMapAddress;

        CHAI3DOutData coutData;
        IntPtr coutFileMap;
        IntPtr coutMapAddress;

        private bool CHAI3Drunning = true;
        private Thread CHAI3Dthread;

        // ERROR
        public event EventHandler updateTextEvent;
        public string errMsg = "";
        int i = 0;

        public void init ()
        {
            /// CHAI3D mapping create
            // outmap
            coutFileMap = PipeNative.CreateFileMapping(IntPtr.Subtract(IntPtr.Zero, 1),
                                IntPtr.Zero,
                                FileMapProtection.PageReadWrite | FileMapProtection.SectionCommit,
                                0, 2048, "chai3d2manager");
            if (coutFileMap == IntPtr.Zero)
            {
                errMsg = "outFile (Recving) CreateFileMapping() Error.";
            }
            else
            {
                coutMapAddress = PipeNative.MapViewOfFile(coutFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, 0, UIntPtr.Zero);
                if (coutMapAddress == IntPtr.Zero)
                {
                    errMsg = "outFile (Recving) MapViewOfFile() Error.";
                    // tell it's an error
                }
                else
                {
                    errMsg = "Recving No Problem!";

                    //initialAngles = gimbalModule.getAngle();

                    coutData.currentHIPX = 0.0f;
                    coutData.currentHIPY = 0.0f;
                    coutData.currentHIPZ = 0.0f;
                    
                    Marshal.StructureToPtr(coutData, coutMapAddress, false);
                }
            }

            // inmap
            cinFileMap = PipeNative.CreateFileMapping(IntPtr.Subtract(IntPtr.Zero, 1),
                                IntPtr.Zero,
                                FileMapProtection.PageReadWrite | FileMapProtection.SectionCommit,
                                0, 2048, "manager2chai3d");
            if (cinFileMap == IntPtr.Zero)
            {
                errMsg = "inFile (Sending) CreateFileMapping() Error.";
            }
            else
            {
                cinMapAddress = PipeNative.MapViewOfFile(cinFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, 0, UIntPtr.Zero);
                if (cinMapAddress == IntPtr.Zero)
                {
                    errMsg = "inFile (Sending) MapViewOfFile() Error.";
                    // tell it's an error
                }
                else
                {
                    errMsg = "Sending No Problem!";
                    // TODO: Sending Information Initialization
                    cinData.objectPositionX = 0.0f;
                    cinData.objectPositionX = 0.0f;
                    cinData.objectPositionX = 0.0f;

                    Marshal.StructureToPtr(cinData, cinMapAddress, false);
                }
            }

            CHAI3Dthread = new Thread(CHAI3dThread);
            CHAI3Dthread.Start();
        }

        public void CHAI3dThread()
        {
            i = 0;
            while (CHAI3Drunning)
            {
                i++;
                coutData = (CHAI3DOutData)Marshal.PtrToStructure(coutMapAddress, typeof(CHAI3DOutData));
                updateTextEvent(this, EventArgs.Empty);
                //if (i++ > 500000)
                //    stopCHAI3D();
            }
        }

        public void stopCHAI3D()
        {
            CHAI3Drunning = false;
        }

        /// <summary>
        /// trivial functions
        /// </summary>
        /// <returns></returns>
        public string getErrorMsg()
        {
            return errMsg;
        }

        public string getHIPposition()
        {
            return coutData.currentHIPX.ToString("0.000") + ", "
                + coutData.currentHIPY.ToString("0.000") + ", "
                + coutData.currentHIPZ.ToString("0.000")
                + "\n" + i.ToString();
        }
    }
}
