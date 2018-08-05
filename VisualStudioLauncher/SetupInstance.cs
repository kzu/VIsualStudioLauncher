// Main types generated via https://app.quicktype.io/ by pasting the output of 
// vswhere.exe -format json -prerelease

// To force generation of the Catalog as a class instead of a Dictionary<string, string>, 
// add a an entry to any catalog in the json like "_": 0,. This forces the generator 
// to consider at least one property is not a string and therefore creates a type for it.

namespace VisualStudioLauncher
{
    public partial class SetupInstance
    {
        public bool IsLaunchable { get; set; }
    }
}
