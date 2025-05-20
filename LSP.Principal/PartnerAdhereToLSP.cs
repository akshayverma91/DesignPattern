using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.PrincipalAdhereToLSP
{
    public class Partner
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public bool IsExternal { get; set; }

        public Partner(string name, string id, Dictionary<string, string> metadata, bool isExternal)
        {
            Name = name;
            ID = id;
            Metadata = metadata ?? new Dictionary<string, string>();
            IsExternal = isExternal;
        }

        // This method now focuses on displaying the *core* partner identity, which is universal.
        public virtual void DisplayCorePartnerInfo()
        {
            Console.WriteLine($"Partner Name: {Name}, ID: {ID}, External: {IsExternal}");
        }

        // New: A separate, explicit method for displaying metadata.
        // This allows subclasses to control or prevent public metadata display without violating LSP
        // for the core info.
        public virtual void DisplayPublicMetadata()
        {
            Console.WriteLine("Metadata:");
            if (Metadata.Any())
            {
                foreach (var entry in Metadata)
                {
                    Console.WriteLine($"  {entry.Key}: {entry.Value}");
                }
            }
            else
            {
                Console.WriteLine("  No public metadata available.");
            }
        }
    }

    public class ProvisionPartner : Partner
    {
        public string ProvisionType { get; set; }

        public ProvisionPartner(string name, string id, Dictionary<string, string> metadata, bool isExternal, string provisionType)
            : base(name, id, metadata, isExternal)
        {
            ProvisionType = provisionType;
        }

        // Overrides core info display, extending it.
        public override void DisplayCorePartnerInfo()
        {
            base.DisplayCorePartnerInfo();
            Console.WriteLine($"Provision Type: {ProvisionType}");
        }

        // ProvisionPartner still wants to display its public metadata via the general method.
    }

    // Fixed InternalPartner - now adheres to LSP
    public class InternalPartner : Partner
    {
        public InternalPartner(string name, string id, Dictionary<string, string> internalMetadata)
            : base(name, id, internalMetadata, false)
        {
            // Internal partners might have metadata, but it's not meant for public display.
        }

        // Adheres to LSP: Overrides DisplayCorePartnerInfo without removing expected behavior.
        public override void DisplayCorePartnerInfo()
        {
            Console.WriteLine($"Internal Partner Name: {Name}, ID: {ID}");
            // We're not calling base.DisplayCorePartnerInfo() directly here, but we are
            // providing a consistent display of core info for this type.
            // If the base method included 'IsExternal', we'd ensure that was also reflected,
            // or call base.DisplayCorePartnerInfo().
        }

        // LSP Adherence: We explicitly override DisplayPublicMetadata to indicate that
        // internal partners do not expose their metadata publicly via this method.
        public override void DisplayPublicMetadata()
        {
            Console.WriteLine("  (Public metadata display is suppressed for internal partners.)");
        }

        public void AccessInternalData()
        {
            Console.WriteLine($"Accessing internal data for {Name}...");
            Console.WriteLine("  Internal Metadata:");
            foreach (var entry in Metadata) // Can still access its own internal metadata
            {
                Console.WriteLine($"    {entry.Key}: {entry.Value}");
            }
        }
    }

    public class FixedLSPExample
    {
        public static void ProcessPartnerFixed(Partner partner)
        {
            Console.WriteLine("--- Processing Partner (Fixed LSP) ---");
            partner.DisplayCorePartnerInfo(); // Always displays core info
            partner.DisplayPublicMetadata();  // Always attempts to display public metadata (subclass controls what that means)
            Console.WriteLine("--------------------------------------\n");
        }

        public static void Run()
        {
            Console.WriteLine("--- Fixed LSP Demo ---");

            var provisionPartner = new ProvisionPartner(
                "Global Services Co. (Fixed)",
                "R002F",
                new Dictionary<string, string> { { "Contract", "Tier 1" } },
                false,
                "Service Agreement"
            );

            var internalPartnerFixed = new InternalPartner(
                "Internal Accounts Dept. (Fixed)",
                "INT001F"
            );
            internalPartnerFixed.Metadata.Add("Department", "Finance"); // Internal metadata

            ProcessPartnerFixed(provisionPartner);
            ProcessPartnerFixed(internalPartnerFixed);

            // We can still access internal data for internal partners directly if needed
            internalPartnerFixed.AccessInternalData();
            Console.WriteLine();
        }
    }
}
