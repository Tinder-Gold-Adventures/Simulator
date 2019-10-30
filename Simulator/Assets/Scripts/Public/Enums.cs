//Possible spawn locations
public enum Location
{
    North,
    East,
    South,
    West
}

//All types of traffic
public enum TrafficType
{
    Car,
    Train,
    Boat,
    Bicycle,
    Passenger
}

//All types of traffic lanes
public enum LaneTypes
{
    motorised,
    cycle,
    foot,
    vessel,
    track
}

//All types of components, controllable from the Controller
public enum ComponentTypes
{
    traffic_light,
    warning_light,
    sensor,
    barrier
}

//Different states a traffic light can be in
public enum TrafficLightState
{
    Red = 0,
    Yellow = 1,
    Green = 2,
    Disabled = 3
}

//Different states a warning light can be in
public enum WarningLightState
{
    Off = 0,
    On = 1
}

//Different states a barrier can be in
public enum BarrierState
{
    Open = 0,
    Closed = 1
}
