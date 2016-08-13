using System.Diagnostics.CodeAnalysis;

namespace HarmonyHub.Internals
{
    /// <summary>
    ///     The supported commands which can be send to the harmony hub.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum HarmonyCommands
    {
        /// <summary>
        ///     The command will have the harmony hub return the configuration
        /// </summary>
        config,

        /// <summary>
        ///     The command will have the harmony hub return the current activity
        /// </summary>
        getCurrentActivity,

        /// <summary>
        ///     The command will make the harmony hub send a command to a device.
        /// </summary>
        holdAction,

        /// <summary>
        ///     The command will have the harmony hub start a specified activity
        /// </summary>
        startactivity
    }
}