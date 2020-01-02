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

