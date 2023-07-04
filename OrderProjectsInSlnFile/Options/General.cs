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
        [Category("General")]
        [DisplayName("Do not show message box")]
        [Description("Determines whether the message box about sorting the .sln file will be displayed.")]
        [DefaultValue(false)]
        public bool DoNotShowMesssageAnymore { get; set; }

        [Category("General")]
        [DisplayName("Sort .sln file always without asking")]
        [Description("Determines whether the sorting the .sln file will always start without asking first.")]
        [DefaultValue(false)]
        public bool SortAlwaysWithoutAsking { get; set; }

        [Category("General")]
        [DisplayName("Never sort .sln file after closing solution")]
        [Description("Determines whether the sorting can happen after closing solution.")]
        [DefaultValue(false)]
        public bool NeverSortAfterClosingSolution { get; set; }
    }
}
