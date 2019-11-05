using System;
using System.Runtime.InteropServices;

namespace LabProg
{

    class PCO_SDK_LibWrapper
    {
        [DllImport("sc2_cam.dll", EntryPoint = "PCO_OpenCamera",
       ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_OpenCamera(ref IntPtr pHandle, UInt16 wCamNum);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_OpenCameraEx",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_OpenCameraEx(ref IntPtr pHandle, PCO_OpenStruct strOpen);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_CloseCamera",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_CloseCamera(IntPtr pHandle);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_ResetLib",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_ResetLib();

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraDescription",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraDescription(IntPtr pHandle, ref PCO_Description strDescription);

        // In C# it is hard to deal with pointer to structures with different sizes.
        // Thus it is easier to setup a similar function call for each available structure.
        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraDescriptionEx",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraDescriptionEx(IntPtr pHandle, ref PCO_Description strDescription, UInt16 wType);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraDescriptionEx",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraDescriptionEx2(IntPtr pHandle, ref PCO_Description2 strDescription, UInt16 wType);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraHealthStatus",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraHealthStatus(IntPtr pHandle, ref UInt32 dwWarn, ref UInt32 dwError, ref UInt32 dwStatus);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_AllocateBuffer",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_AllocateBuffer(IntPtr pHandle, ref short sBufNr, int size, ref UIntPtr wBuf, ref IntPtr hEvent);
        //HANDLE ph,SHORT* sBufNr,DWORD size,WORD** wBuf,HANDLE *hEvent

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetBuffer",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetBuffer(IntPtr pHandle, short sBufNr, ref UIntPtr wBuf, ref IntPtr hEvent);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_FreeBuffer",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_FreeBuffer(IntPtr pHandle, short sBufNr);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_ArmCamera",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_ArmCamera(IntPtr pHandle);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_CamLinkSetImageParameters",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_CamLinkSetImageParameters(IntPtr pHandle, UInt16 wXRes, UInt16 wYRes);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_SetRecordingState",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_SetRecordingState(IntPtr pHandle, UInt16 wRecState);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetRecordingState",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetRecordingState(IntPtr pHandle, ref UInt16 wRecState);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_CancelImages",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_CancelImages(IntPtr pHandle);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_AddBuffer",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_AddBuffer(IntPtr pHandle, UInt32 dwFirstImage, UInt32 dwLastImage, short sBufNr);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_AddBufferEx",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_AddBufferEx(IntPtr pHandle, UInt32 dwFirstImage, UInt32 dwLastImage, short sBufNr, UInt16 wXRes, UInt16 wYRes, UInt16 wBitPerPixel);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetBufferStatus",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetBufferStatus(IntPtr pHandle, short sBufNr, ref UInt32 dwStatusDll, ref UInt32 dwStatusDrv);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetStorageStruct",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetStorageStruct(IntPtr pHandle, ref PCO_Storage strStorage);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetImageStruct",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetImageStruct(IntPtr pHandle, ref PCO_Image strImage);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraType",
           ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraType(IntPtr pHandle, ref PcoCameraType strCameraType);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetCameraName",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetCameraName(IntPtr pHandle, byte[] szCameraName, ushort wSZCameraNameLen);

        [DllImport("sc2_cam.dll", EntryPoint = "PCO_SetTriggerMode",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_SetTriggerMode(IntPtr pHandle, ushort wTriggerMode);
        [DllImport("sc2_cam.dll", EntryPoint = "PCO_GetSizes",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_GetSizes(IntPtr hCamDialog,
                                  ref UInt16 wXResAct, // Actual X Resolution
                                  ref UInt16 wYResAct, // Actual Y Resolution
                                  ref UInt16 wXResMax, // Maximum X Resolution
                                  ref UInt16 wYResMax); // Maximum Y Resolution

        [DllImport("sc2_Dlg.dll", EntryPoint = "PCO_OpenDialogCam",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_OpenDialogCam(ref IntPtr hCamDialog, IntPtr pHandle, IntPtr parent, UInt32 uiFlags, UInt32 uiMsgArm, UInt32 uiMsgCtrl, int xpos, int ypos, [MarshalAs(UnmanagedType.LPStr)]string title);

        [DllImport("sc2_Dlg.dll", EntryPoint = "PCO_CloseDialogCam",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_CloseDialogCam(IntPtr hCamDialog);

        [DllImport("sc2_Dlg.dll", EntryPoint = "PCO_EnableDialogCam",
            ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int PCO_EnableDialogCam(IntPtr hCamDialog, bool bEnable);
    }
}
