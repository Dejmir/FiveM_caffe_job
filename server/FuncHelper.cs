using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace fivem_caffe_job
{
    public class FuncHelper : BaseScript
    {
        public static void ChatMessage(string msg)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 255, 255 },
                args = new[] { $"{msg}" }
            });
        }
        public static void ChatMessage(string msg, Array color)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = color, //color = new[] { 0, 0, 0 },
                args = new[] { $"{msg}" }
            });
        }
    }
}
