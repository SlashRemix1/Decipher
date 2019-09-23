using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akorade
{
    public static class Extensions
    {
        public static void SynchronizedInvoke(this ISynchronizeInvoke sync, Action action)
        {
            // If the invoke is not required, then invoke here and get out.
            if (!sync.InvokeRequired)
            {
                // Execute action.
                action();

                // Get out.
                return;
            }

            // Marshal to the required context.
            sync.Invoke(action, new object[] { });
        }

        public static void ThreadSafeInvoke(this Control control, Action action)
        {
            try
            {
                if (!control.InvokeRequired)
                {
                    action();
                }
                else
                {
                    control.Invoke(action);
                }
            }
            catch
            {
            }
        }
    }
}
