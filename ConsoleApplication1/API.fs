module API 

open System

/// Events that Windows sends us when keyboard is pressed, relayed to handlers

type ModifierKeys = {
    shift: bool; 
    control: bool; 
    meta: bool; 
    alt: bool; 
}

type KeyPressData = { 
    currentlyPressedKeys : list<int>;
    key: int
}

type KeyReleaseData = { 
    currentlyPressedKeys : list<int>;
    key: int
}

type Event = 
    | KeyPressEvent of KeyPressData 
    | KeyReleaseEvent of KeyReleaseData


/// Stuff that handlers do in response to events

type KeyPressAction = {
    holdKeys: list<int>;
    pressKeys: list<int>;
    blockPropagation: bool
}

type HandlerAction =
    | KeyPressAction of KeyPressAction
    | NoAction


type EventHandler = Event -> HandlerAction

let desktopSwitchHandler : EventHandler = 
    fun event ->
        match event with
        | KeyPressEvent(e) -> 
            Console.WriteLine("Doing nothing to key press")
            NoAction
        | KeyReleaseEvent(e) -> 
            Console.WriteLine("Doing nothing to key release")
            NoAction

