using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SC.BL.Domain
{
    [DataContract]
    public class HardwareTicket : Ticket
    {
        [DataMember]
        public string DeviceName { get; set; }
    }
}
