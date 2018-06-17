public enum CompanionState {
    Following, //companion follows the player around
    Returning, //companion returns to the player and delivers the vacuum gun
    Staying, //companion stays where he is for a certain amount of time
    Traveling, //companion travels to a task
    Roaming, //companion roams around the areas and draws attention
    Waiting, //companion waits for the player do so something and provides reinforcement
    Instructing, //compansion tells the player about something
    GettingVacuum, //companion catches or picks up the vacuum
    HandingVacuum //companion hands over the vacuum to the player
}

public enum ObjectiveStatus {
    Incomplete, //not complete yet
    Active, //currently active
    Complete //complete objective
}

public enum ObjectiveTask {
    Talk, //talk about something, no actual objective
    Cleanup, //clean trash
    Choose, //choose the location for the minigame
    Place, //place turbine
    PlugIn, //plug in the cable into the turbine
    PowerOn, //power on turbine
    Assemble, //pick up parts from the turbine and reassemble them
    NextLevel, //enable level transition
    Event //points of interest, maybe more specification (side objecitves)
}

public enum AudioSourceType {
    Effects, //general companion sounds
    Voice //voiceovers
}

public enum ObjectiveType {
    Main, //Main objective
    Side //Side objective
}

public enum VacuumState {
    CompanionBack, //companion carries vacuum on his back
    CompanionHand, //companion hold vacuum in his left hand
    Player, //player carries vacuum
    Free //not held or used by anything
}