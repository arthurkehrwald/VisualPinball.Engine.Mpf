namespace FutureBoxSystems.MpfMediaController.Messages.Monitor
{
    public enum MonitoringCategory
    {
        [StringValue(null)]
        None,
        [StringValue("events")]
        Events,
        [StringValue("devices")]
        Devices,
        [StringValue("machine_vars")]
        MachineVars,
        [StringValue("player_vars")]
        PlayerVars,
        [StringValue("switches")]
        Switches,
        [StringValue("modes")]
        Modes,
        [StringValue("core_events")]
        CoreEvents
    }
}