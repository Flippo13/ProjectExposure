public enum CompanionState {
    Inactive, //companion is turned off and has to be turned on
    Following, //companion follows the player around
    Traveling, //companion travels to a task
    Roaming, //companion roams around the areas and draws attention
    Waiting, //companion waits for the player do so something and provides reinforcement
    Instructing, //compansion tells the player about something
    Transforming, //companion is transformed into the vacuum gun
    Useable, //companion can be picked up
    Grabbed //companion is picked up
}

public enum TransformationState {
    None, //not transforming 
    Vacuum, //transforming into the vacuum
    Robot //trasnforming into the robot
}

public enum ObjectiveStatus {
    Incomplete, //not complete yet
    Active, //currently active
    Complete //complete objective
}

public enum ObjectiveTask {
    Cleanup, //clean trash
    Place, //place turbine
    PowerOn, //power on turbine
    SideTask //points of interest
    //more
}

public enum AudioSourceType {
    Effects, //general companion sounds
    Voice //voiceovers
}

public enum ObjectiveType {
    Main, //Main objective
    Side //Side objective
}