module NativeBindings.Hooks

open System
open System.Diagnostics
open System.Runtime.InteropServices
open NativeBindings

// Docs and SO claim that the second argument is also a IntPtr, but it looks like
// it's definitely an int.
// TODO maybe this is why my serialization is messed up?
type LowLevelKeyboardProc = delegate of int * int * IntPtr -> IntPtr

module private NativeMethods =
    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint32 dwThreadId);

    [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern IntPtr GetModuleHandle(string lpModuleName);

    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

    [<DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern bool UnhookWindowsHookEx(IntPtr hhk)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern IntPtr GetMessageExtraInfo()

module private IdHook =
    let WH_KEYBOARD_LL = 13

module private WParams =
    let WM_KEYDOWN = 0x0100
    let WM_KEYUP = 0x0101
    let WM_SYSKEYDOWN = 0x0104
    let WM_SYSKEYUP = 0x0105


let private setKeyListener (fn: LowLevelKeyboardProc) =
    let handle = NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName)
    NativeMethods.SetWindowsHookEx(IdHook.WH_KEYBOARD_LL, fn, handle, uint32 0)

[<StructLayout(LayoutKind.Sequential)>]
type KBDLLHOOKSTRUCT = struct
    val vkCode: int
    val scanCode: int
    val flags: int
    val time: int
    val dwExtraInfo: int 
end

let private ConsumeKeycode = IntPtr(1)

type LowLevelKeyState =
    | KeyPress
    | KeyRelease

type LowLevelKeyboardEvent = {
    rawStruct: KBDLLHOOKSTRUCT
    keyState: LowLevelKeyState
}

type LowLevelEventHandler = LowLevelKeyboardEvent -> bool

let mutable private hookId = IntPtr.Zero // api fail
let private keyListener handler nCode (wParam: int) lParam =
    if nCode < 0 then
        // Docs say we have to call CallNextHookEx in this case
        NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
    else
        let keyState = if wParam = WParams.WM_KEYDOWN then KeyPress else KeyRelease 
        let keyboardStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam)
        let keyboardEvent = { rawStruct = keyboardStruct; keyState = keyState }
        Console.WriteLine("Got keycode " + keyboardStruct.vkCode.ToString())

        if keyboardStruct.dwExtraInfo = SendInputBindings.appSignature then
            // Don't send events that we generate to our handlers
            NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
        elif handler(keyboardEvent) then
            ConsumeKeycode
        else 
            NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);

let removeKeyListener =
    NativeMethods.UnhookWindowsHookEx(hookId)

// Without these, our handler gets garbage collected eventually and C++ throws
type UnGarbageCollectableHandler = Uninitialized | Initilaized of LowLevelKeyboardProc  
let mutable dontGarbageCollectHandler: UnGarbageCollectableHandler = Uninitialized
    
let setupKeyListener (handler: LowLevelEventHandler)  = 
    let proc = new LowLevelKeyboardProc(keyListener handler)
    dontGarbageCollectHandler <- Initilaized <| proc
    hookId <- setKeyListener(proc)
