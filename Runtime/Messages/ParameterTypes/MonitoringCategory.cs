namespace FutureBoxSystems.MpfMediaController
{
    public enum MonitoringCategory
    {
        [StringValue(null)]
        None,
        [StringValue("events")]
        Events,
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