using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ServerApp.Models.GosRegistries
{
    public class KgdDutyReqPars
    {
        [JsonProperty("captcha-id")]
        public string CaptchaId { get; set; }

        [JsonProperty("captcha-user-value")]
        public string CaptchaUserValue { get; set; }

        [JsonProperty("iinBin")]
        public string IinBin { get; set; }

    }
}