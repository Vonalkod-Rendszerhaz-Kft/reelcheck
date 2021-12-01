using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterventionEmulator
{
    public enum LabelDInputs
    {
        Enable,
        StickingDone,
        StatusReset,
        HardReset,
    }

    public enum LabelDOutputs
    {
        Ready,
        LabelPrinted,
        AllStationFinished,
        WorkpieceOk,
        WorkPieceNok,
        Empty,
    }

    public enum Switch
    {
        On,
        Off,
    }
}
