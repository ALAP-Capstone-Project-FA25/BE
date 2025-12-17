using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ALAP.Entity.Models.Enums
{
    public enum EventStatusEnum
    {
        [Description("Wait For Approval")]
        WAIT_FOR_APPROVAL = 1,

        [Description("Wait For Payment")]
        WAIT_FOR_PAYMENT = 2,

        [Description("Preparing")]
        PREPARING = 3,

        [Description("In Progress")]
        IN_PROGRESS = 4,

        [Description("Completed")]
        COMPLETED = 5
    }
}
