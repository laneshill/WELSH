using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*********************
 * AliasMode
 * 
 * The mode of the central command in the alias file.  
 * This class is deprecated and is not actually used for anything, but is kept because of compability reasons. 
 * Will be removed in Welsh V.2
 * 
 * Enumeration:
 * path = 0 (the mode of the command is an external file)
 * c_central = 1 (the mode of the command is a built in command in WELSH).
 *
 */
namespace Wash.Components
{
    enum AliasMode 
    {
        path, c_central
    }
}
