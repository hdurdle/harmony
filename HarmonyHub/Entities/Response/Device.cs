using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    ///     HarmonyHub Device
    /// </summary>
    [DataContract]
    public class Device : IComparable<Device>
    {
        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "Capabilities")]
        public IList<int> Capabilities { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "controlGroup")]
        public IList<ControlGroup> ControlGroups { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "ControlPort")]
        public int ControlPort { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "deviceProfileUri")]
        public string DeviceProfileUri { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "deviceTypeDisplayName")]
        public string DeviceTypeDisplayName { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "DongleRFID")]
        public int DongleRfid { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Id of the device
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "IsKeyboardAssociated")]
        public bool IsKeyboardAssociated { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "isManualPower")]
        public string IsManualPower { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Manufacturer of the device
        /// </summary>
        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Device model
        /// </summary>
        [DataMember(Name = "model")]
        public string Model { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "suggestedDisplay")]
        public string SuggestedDisplay { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "Transport")]
        public int Transport { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Used to order devices
        /// </summary>
        /// <param name="other">Device to compare to</param>
        /// <returns></returns>
        public int CompareTo(Device other)
        {
            return string.Compare(Label, other?.Label, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// The to string will return the label, type and Id
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Label} : {Type} ({Id})";
        }
    }
}