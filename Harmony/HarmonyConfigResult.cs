using System.Collections.Generic;

namespace Harmony
{
    public class HarmonyConfigResult
    {
        public List<Activity> activity { get; set; }
        public List<Device> device { get; set; }
        public List<object> sequence { get; set; }
        public Content content { get; set; }
        public Global global { get; set; }
    }

    public class Activity
    {
        public string suggestedDisplay { get; set; }
        public string label { get; set; }
        public string id { get; set; }
        public string activityTypeDisplayName { get; set; }
        public List<object> controlGroup { get; set; }
        public List<object> sequences { get; set; }
        public int activityOrder { get; set; }
        public bool isTuningDefault { get; set; }
        public Fixit fixit { get; set; }
        public string type { get; set; }
        public string icon { get; set; }
        public string baseImageUri { get; set; }
    }

    public class Device
    {
        public string type { get; set; }
        public string deviceTypeDisplayName { get; set; }
        public string label { get; set; }
        public string id { get; set; }
        public int Transport { get; set; }
        public List<object> controlGroup { get; set; }
        public int ControlPort { get; set; }
        public string suggestedDisplay { get; set; }
        public string model { get; set; }
        public string deviceProfileUri { get; set; }
        public string manufacturer { get; set; }
        public string icon { get; set; }
        public string isManualPower { get; set; }
    }

    public class Content
    {
        public string contentUserHost { get; set; }
        public string contentDeviceHost { get; set; }
        public string contentServiceHost { get; set; }
        public string contentImageHost { get; set; }
        public string householdUserProfileUri { get; set; }
    }

    public class Global
    {
        public string timeStampHash { get; set; }
    }



    public class Fixit
    {
        public __invalid_type__14766261 __invalid_name__14766261 { get; set; }
        public __invalid_type__14766257 __invalid_name__14766257 { get; set; }
        public __invalid_type__14766263 __invalid_name__14766263 { get; set; }
        public __invalid_type__14766260 __invalid_name__14766260 { get; set; }
        public __invalid_type__14766271 __invalid_name__14766271 { get; set; }
        public __invalid_type__14766278 __invalid_name__14766278 { get; set; }
    }

    public class __invalid_type__14766261
    {
        public string id { get; set; }
        public string Power { get; set; }
    }

    public class __invalid_type__14766257
    {
        public string id { get; set; }
        public bool isRelativePower { get; set; }
        public string Input { get; set; }
        public string Power { get; set; }
    }

    public class __invalid_type__14766263
    {
        public string id { get; set; }
        public bool isManualPower { get; set; }
    }

    public class __invalid_type__14766260
    {
        public string id { get; set; }
        public string Input { get; set; }
        public string Power { get; set; }
    }

    public class __invalid_type__14766271
    {
        public string id { get; set; }
        public bool isRelativePower { get; set; }
        public string Power { get; set; }
        public string Input { get; set; }
    }

    public class __invalid_type__14766278
    {
        public string id { get; set; }
        public string Power { get; set; }
    }
}
