
module NativeBidnings.WindowManagement

open System
open System.Text
open System.Runtime.InteropServices
open NativeBindings
open System.Diagnostics
open Microsoft.FSharp.NativeInterop



module private NativeMethods =
    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern IntPtr GetForegroundWindow()

    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern uint32 GetWindowThreadProcessId(IntPtr hWnd, nativeint lpdwProcessId);


// TODO This will ahve to be changed. It doesn't work for a lot of windows things.
// Just returns ApplicationFrameHost
// https://stackoverflow.com/questions/39702704/connecting-uwp-apps-hosted-by-applicationframehost-to-their-real-processes
let getFocusedWindow (_: unit) =
    let windowPtr = NativeMethods.GetForegroundWindow()
    let pidPtr = NativePtr.stackalloc<int> 1
    NativeMethods.GetWindowThreadProcessId(windowPtr, NativePtr.toNativeInt pidPtr) 
    let pid = NativePtr.read(pidPtr)
    let process = Process.GetProcessById(pid)
    process.ProcessName

