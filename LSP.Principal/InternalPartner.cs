using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Principal
{
    public class InternalPartner : Partner
    {
        public InternalPartner(string name, string id)
            : base(name, id, new Dictionary<string, string>(), false) // Internal partners are always internal
        {
            // Internal partners might have internal metadata, but for external display, we want to restrict it.
        }

        // LSP Violation: This method changes the expected behavior of DisplayPartnerInfo.
        // The base method promises to display all available metadata. By overriding it
        // and explicitly *not* displaying metadata, we break the contract.
        public override void DisplayPartnerInfo()
        {
            Console.WriteLine($"Internal Partner Name: {Name}, ID: {ID}");
            Console.WriteLine("  (Metadata is intentionally suppressed for internal partners in this view.)");
            // We've actively chosen not to call base.DisplayPartnerInfo() or
            // display the Metadata property, violating the expectation.
        }

        public void AccessInternalData()
        {
            Console.WriteLine($"Accessing internal data for {Name}...");
        }
    }

    public class BrokenLSPExample
    {
        public static void Run()
        {
            Console.WriteLine("--- Breaking LSP Demo ---");

            var internalPartner = new InternalPartner(
                "Internal Accounts Dept.",
                "INT001"
            );
            internalPartner.Metadata.Add("Department", "Finance"); // Add some metadata internally

            // The ProcessPartner method expects any Partner to display ALL its metadata.
            // But when InternalPartner is passed, it explicitly suppresses this.
            InitialLSPExample.ProcessPartner(internalPartner); // LSP is violated here
        }
    }
}
