using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VwSyncSever
{
    class Settings
    {

        internal static string
            /* local */   strLocalFolder = @"c:\\___\",
             /* remote */ strRemoteFolder =
        @"\\CJ-PC\Users\Default\AppData";
        //@"\\10.10.10.47\video\gi test\demo\";
        //@"c:\_ToDo\TestHik\TestHik\bin\x86\Debug\DbgMessages\";


        internal static string
            dirForMetadata = "\\___meta___",
            dirForTemporaryFiles = "\\___temp___",
            dirForConflictedFiles = "\\___conf___";


        //extensii care nu vor fi sincronizate
        internal static string[] syncExcludeExtensions = 
            new string[] { "*.tmp", "*.lnk", "*.pst" };
        //const string             displayExcludeDirExtension = ; // 


    }
}
