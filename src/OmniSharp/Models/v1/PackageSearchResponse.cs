using System.Collections.Generic;

namespace OmniSharp.Models
{
    public class PackageSearchResponse
    {
        public IEnumerable<PackageSearchItem> Packages { get; set; } = new List<PackageSearchItem>();
        public IEnumerable<string> Sources { get; set; } = new List<string>();
    }
}
