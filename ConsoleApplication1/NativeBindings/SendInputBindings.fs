module NativeBindings.SendInputBindings

open System.Runtime.InteropServices

[<StructLayout(LayoutKind.Sequential)>]
type private MOUSEINPUT = struct
    val dx: int32
    val dy:int32
    val mouseData:uint32
    val dwFlags: uint32
    val time: uint32
    val dwExtraInfo: int
    new(_dx, _dy, _mouseData, _dwFlags, _time, _dwExtraInfo) = {dx=_dx; dy=_dy; mouseData=_mouseData; dwFlags=_dwFlags; time=_time; dwExtraInfo=_dwExtraInfo}
end

[<StructLayout(LayoutKind.Sequential)>]
type private KEYBDINPUT = struct
    val wVk: uint16
    val wScan: uint16
    val dwFlags: uint32
    val time: uint32
    val dwExtraInfo:int
    new(_wVk, _wScan, _dwFlags, _time, _dwExtraInfo) = {wVk =_wVk; wScan = _wScan; dwFlags = _dwFlags; time = _time; dwExtraInfo = _dwExtraInfo}
end

[<StructLayout(LayoutKind.Sequential)>]
type private HARDWAREINPUT = struct
    val uMsg: uint32
    val wParamL: uint16
    val wParamH: uint16
    new(_uMsg, _wParamL, _wParamH) = {uMsg = _uMsg; wParamL = _wParamL; wParamH = _wParamH}
end

[<StructLayout(LayoutKind.Explicit)>]
type private LPINPUT  = struct
    [<FieldOffset(0)>]
    val mutable ``type``:int // 1 is keyboard

    [<FieldOffset(4)>]
    val mutable mi : MOUSEINPUT

    [<FieldOffset(4)>]
    val mutable ki : KEYBDINPUT

    [<FieldOffset(4)>]
    val mutable hi : HARDWAREINPUT 
end

module private InputModes =
    let INPUT_MOUSE = 0;
    let INPUT_KEYBOARD = 1;
    let INPUT_HARDWARE = 2;

module private Dwords = 
    let KEYEVENTF_KEYDOWN = uint32 0x0000
    let KEYEVENTF_EXTENDEDKEY = uint32 0x0001
    let KEYEVENTF_KEYUP = uint32 0x0002
    let KEYEVENTF_UNICODE = uint32 0x0004
    let KEYEVENTF_SCANCODE = uint32 0x0008

module KeyCodes =
    let VK_CONTROL = 162
    let VK_TAB = 9
    let VK_LWIN = 91
    let VK_RIGHT = 39
    let VK_UP = 38
    let VK_DOWN = 40
    let VK_LEFT = 37
    let VK_SHIFT = 160
    let VK_ESCAPE = 27

    // These are left and right alt
    let VK_LMENU = 164
    let VK_RMENU = 165

    let KEY_A = 65
    let KEY_H = 72
    let KEY_J = 74
    let KEY_K = 75
    let KEY_L = 76

module private NativeMethods =
    [<DllImport("user32.dll", SetLastError=true)>]
    extern uint32 SendInput(uint32 nInputs, LPINPUT* pInputs, int cbSize)

let appSignature = 0xA8969

let private createPressInput (code: int) =
    let mutable input = LPINPUT()
    input.``type`` <- InputModes.INPUT_KEYBOARD
    input.ki <- KEYBDINPUT(uint16  code, uint16 0, Dwords.KEYEVENTF_KEYDOWN, uint32 0, appSignature)
    input

let private createReleaseInput (code: int) =
    let mutable input = LPINPUT()
    input.``type`` <- InputModes.INPUT_KEYBOARD
    input.ki <- KEYBDINPUT(uint16  code, uint16  0, Dwords.KEYEVENTF_KEYUP, uint32 0, appSignature)
    input

let pressKey (code: int) =
    let input = createPressInput code
    NativeMethods.SendInput(uint32 1, &&input, Marshal.SizeOf(input)) |> ignore

let releaseKey (code: int) =
    let input = createReleaseInput code
    NativeMethods.SendInput(uint32 1, &&input, Marshal.SizeOf(input)) |> ignore

