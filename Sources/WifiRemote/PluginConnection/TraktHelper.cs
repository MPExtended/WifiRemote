using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraktPlugin.GUI;

namespace WifiRemote
{
    class TraktHelper
    {
        /// <summary>
        /// Check if a dialog is a trakt rating dialog
        /// </summary>
        /// <param name="dialog">Dialog</param>
        /// <returns>true/false ;)</returns>
        internal static bool IsTraktRatingDialog(MediaPortal.Dialogs.GUIDialogWindow dialog)
        {
            if (dialog.GetType().Equals(typeof(TraktPlugin.GUI.GUIRateDialog)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
