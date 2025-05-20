using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP.Principal
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

        // Common behavior for all partners
        public virtual void DisplayPartnerInfo()
        {
            Console.WriteLine($"Partner Name: {Name}, ID: {ID}, External: {IsExternal}");
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
                Console.WriteLine("  No metadata available.");
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

        // Overrides base behavior but still fulfills its contract
        public override void DisplayPartnerInfo()
        {
            base.DisplayPartnerInfo(); // Calls the base implementation first
            Console.WriteLine($"Provision Type: {ProvisionType}");
        }

        public void HandleProvisions()
        {
            Console.WriteLine($"Handling provisions for {Name} (Type: {ProvisionType})...");
        }
    }

    public class InitialLSPExample
    {
        public static void ProcessPartner(Partner partner)
        {
            Console.WriteLine("--- Processing Partner ---");
            partner.DisplayPartnerInfo();
            Console.WriteLine("--------------------------\n");
        }

        public static void Run()
        {
            Console.WriteLine("--- Initial LSP Adherence Demo ---");

            var provisionPartner = new ProvisionPartner(
                "Global Services Co.",
                "R002",
                new Dictionary<string, string> { { "Contract", "Tier 1" } },
                false,
                "Service Agreement"
            );

            var genericPartner = new Partner(
                "Basic Partner Ltd.",
                "B003",
                new Dictionary<string, string> { { "Status", "Active" } },
                true
            );

            ProcessPartner(provisionPartner);
            ProcessPartner(genericPartner);
        }
    }
}