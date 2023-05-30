using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OrderProjectsInSlnFile
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "OrderProjectsInSlnFile", "General", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("Message box")]
        [DisplayName("Do not show message box")]
        [Description("Determines whether the message box about sorting the .sln file will be displayed.")]
        [DefaultValue(false)]
        public bool DoNotShowMesssageAnymore { get; set; }
    }
}
