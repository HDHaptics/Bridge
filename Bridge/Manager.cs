using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
//using System.IO.MemoryMappedFiles;

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
    public struct HIPData         // CHAI3D --> Manager (Recving)
    {
        ///
        public float currentHIPX;
        public float currentHIPY;
        public float currentHIPZ;
    }

    public struct InfoView
    {
        public int sysGran;
        public float HIPx;
        public float HIPy;
        public float HIPz;
        public int numberOfView;
        //public int[] sizeOfView;
    }

    public struct ObjectNum
    {
        public int numberOfObject;
    }

    public struct ObjectView         // Manager --> CHAI3D (Sending)
    {
        public float objectPosX;
        public float objectPosY;
        public float objectPosZ;
        public float objectRotX;
        public float objectRotY;
        public float objectRotZ;
        //public float objectPositionX;
        //public float[] objectPositionY;
        //public float[] objectPositionZ;
    }

    public class Manager
    {
        /* // for Unreal
        HIPData uinData;
        IntPtr uinFileMap;
        IntPtr uinMapAddress;

        ObjData uoutData;
        IntPtr uoutFileMap;
        IntPtr uoutMapAddress;
        */

        // for CHAI3D
        ObjectView cinData;
        ObjectView cinData2;
        InfoView cDataInfo;
        IntPtr cinFileMap;
        IntPtr cinMapAddress;
        IntPtr cinMapAddressData;
        IntPtr cinMapAddressData2;

        HIPData coutData;
        IntPtr coutFileMap;
        IntPtr coutMapAddress;

        private bool CHAI3Drunning = true;
        private Thread CHAI3Dthread;

        // ERROR
        public event EventHandler updateTextEvent;
        public string errMsg = "";
        int i = 0;

        uint sizeObjInfo;
        uint sizeObjData;
        uint NUMBER_OF_MAP_VIEW;
        IntPtr numberOfMapView;

        public void init ()
        {
            SYSTEM_INFO sys_info = new SYSTEM_INFO();
            PipeNative.GetSystemInfo(out sys_info);

            sizeObjInfo = (uint)Marshal.SizeOf(cDataInfo);
            sizeObjData = (uint)Marshal.SizeOf(cinData);

            NUMBER_OF_MAP_VIEW = 5;
            uint FILE_MAP_START = 65536;
            uint sysGran = sys_info.allocationGranularity;
            uint fileMapStart = (FILE_MAP_START / sysGran) * sysGran;
            uint mapViewSize1 = (FILE_MAP_START % sysGran) + sizeObjInfo;
            uint mapViewSize = (FILE_MAP_START % sysGran) + sizeObjData*2;
            uint fileMapSize = FILE_MAP_START + sizeObjData;
            int iViewDelta = (int) (FILE_MAP_START - fileMapStart);

            /// CHAI3D mapping create
            // outmap
            coutFileMap = PipeNative.CreateFileMapping(IntPtr.Subtract(IntPtr.Zero, 1),
                                IntPtr.Zero,
                                FileMapProtection.PageReadWrite | FileMapProtection.SectionCommit,
                                0, 139264, "chai3d2manager");
            if (coutFileMap == IntPtr.Zero)
            {
                errMsg = "outFile (Recving) CreateFileMapping() Error.";
            }
            else
            {
                coutMapAddress = PipeNative.MapViewOfFile(coutFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, 0, 0);
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
                                0, sysGran * NUMBER_OF_MAP_VIEW, "HDhaptics");
            if (cinFileMap == IntPtr.Zero)
            {
                errMsg = "inFile (Sending) CreateFileMapping() Error.";
            }
            else
            {
                
                //uint sizeObjNum = (uint) Marshal.SizeOf(cinDataNum);
                //uint sizeObjData = (uint) Marshal.SizeOf(cinData);

                cinMapAddress = PipeNative.MapViewOfFile(cinFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, 0, sysGran);
                if (cinMapAddress == IntPtr.Zero)
                {
                    errMsg = "inFile (Sending) MapViewOfFile() Error.";
                    // tell it's an error
                }
                else
                {
                     
                    errMsg = "No Sending Problem!";
                    // TODO: Sending Information Initialization
                    //cinData.numberOfObject = 10;
                    cDataInfo.sysGran = (int) sysGran;
                    cDataInfo.HIPx = 0f;
                    cDataInfo.HIPy = 0f;
                    cDataInfo.HIPz = 0f;
                    cDataInfo.numberOfView = (int) NUMBER_OF_MAP_VIEW - 1;
                    //cDataInfo.sizeOfView = new int[4] { (int)sysGran, (int)sysGran, (int)sysGran, (int)sysGran };
                    //cinData.objectPositionX = new float[10] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };
                    //cinData.objectPositionY = new float[10] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };
                    //cinData.objectPositionZ = new float[10] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };

                    Marshal.StructureToPtr(cDataInfo, cinMapAddress, false);
                    //Marshal.StructureToPtr()
                    int[] sizeOfViews = new int[4] { (int)sysGran, (int)sysGran, (int)sysGran, (int)sysGran };
                    //Marshal.Copy(sizeOfViews, 0, cinMapAddress + (int)sizeObjInfo, sizeOfViews.Length);
                    for (int i = 0; i < sizeOfViews.Length; i++)
                    {
                        Marshal.WriteInt32(cinMapAddress + (int)sizeObjInfo + i * 4, sizeOfViews[i]);
                    }                    
                    //numberOfMapView = Marshal.AllocHGlobal((int) NUMBER_OF_MAP_VIEW * 4);
                    
                }

                ////////
                cinMapAddressData = PipeNative.MapViewOfFile(cinFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, fileMapStart, sysGran);
                if (cinMapAddressData == IntPtr.Zero)
                {
                    errMsg = PipeNative.GetLastError() + " // inFile (Sending) MapViewOfFile() Error.";
                    // tell it's an error
                }
                else
                {
                    
                    errMsg = "No Sending Problem!";
                    // TODO: Sending Information Initialization
                    //cinData.numberOfObject = 10;
                    ObjectNum objectNum;
                    objectNum.numberOfObject = 2;
                    Marshal.StructureToPtr(objectNum, cinMapAddressData, false);
                    int sizeObjNum = Marshal.SizeOf(objectNum);

                    cinData.objectPosX = 1f;
                    cinData.objectPosY = 1f;
                    cinData.objectPosZ = 1f;
                    cinData.objectRotX = 0f;
                    cinData.objectRotY = 0f;
                    cinData.objectRotZ = 0f;

                    Marshal.StructureToPtr(cinData, cinMapAddressData + sizeObjNum, false);
                    //Marshal.StructureToPtr()
                    cinData2.objectPosX = 2f;
                    cinData2.objectPosY = 2f;
                    cinData2.objectPosZ = 2f;
                    cinData2.objectRotX = 0f;
                    cinData2.objectRotY = 0f;
                    cinData2.objectRotZ = 0f;
                    
                    Marshal.StructureToPtr(cinData2, cinMapAddressData + (int) sizeObjData + sizeObjNum, false);
                }

                ////////
                /*
                cinMapAddressData2 = PipeNative.MapViewOfFile(cinFileMap, FileMapAccess.FILE_MAP_ALL_ACCESS, 0, fileMapStart, sizeObjData);
                if (cinMapAddressData2 == IntPtr.Zero)
                {
                    errMsg = PipeNative.GetLastError() + " // inFile (Sending) MapViewOfFile() Error.";
                    // tell it's an error
                }
                else
                {

                    errMsg = "No Sending Problem!";
                    // TODO: Sending Information Initialization
                    cinData2.objectPosX = 2f;
                    cinData2.objectPosY = 2f;
                    cinData2.objectPosZ = 2f;
                    cinData2.objectRotX = 0f;
                    cinData2.objectRotY = 0f;
                    cinData2.objectRotZ = 0f;

                    Marshal.StructureToPtr(cinData2, cinMapAddressData2, false);
                    //Marshal.StructureToPtr()
                }
                */
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
                cDataInfo = (InfoView)Marshal.PtrToStructure(cinMapAddress, typeof(InfoView));
                ObjectNum cinDataNum = (ObjectNum)Marshal.PtrToStructure(cinMapAddressData, typeof(ObjectNum));
                cinDataNum.numberOfObject = 2;
                Marshal.StructureToPtr(cinDataNum, cinMapAddressData, false);
                cinData = (ObjectView)Marshal.PtrToStructure(cinMapAddressData, typeof(ObjectView));
                cinData.objectPosX = cDataInfo.HIPx;
                cinData.objectPosY = cDataInfo.HIPy;
                cinData.objectPosZ = cDataInfo.HIPz;
                Marshal.StructureToPtr(cinData, cinMapAddressData + 4, false);

                cinData2 = (ObjectView)Marshal.PtrToStructure(cinMapAddressData + (int)sizeObjData, typeof(ObjectView));
                cinData2.objectPosX = cDataInfo.HIPx + 1;
                cinData2.objectPosY = cDataInfo.HIPy + 1;
                cinData2.objectPosZ = cDataInfo.HIPz + 1;
                Marshal.StructureToPtr(cinData2, cinMapAddressData + (int)sizeObjData + 4, false);

                //updateTextEvent(this, EventArgs.Empty);
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
            string answer = cDataInfo.HIPx.ToString("0.000") + ", "
                + cDataInfo.HIPy.ToString("0.000") + ", "
                + cDataInfo.HIPz.ToString("0.000");
            return answer; 
                ;//+ "\n" + Marshal.SizeOf(cDataInfo).ToString();
        }
    }
}
