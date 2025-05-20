using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Behavioral.Template.Model
{
    /// <summary>
    /// partner model
    /// </summary>
    public class Partner
    {
        public Partner()
        {
        }

        public string? Id { get; set; }

        public int? CompanyId { get; set; }

        public string? CompanyIdentifier { get; set; }

        public string? CompanyName { get; set; }

        public string? FriendlyName { get; set; }

        public string? MainAddress { get; set; }

        [StringLength(18, MinimumLength = 18, ErrorMessage = "CRM Account ID must be 18 characters in length.")]
        public string? CrmAccountId { get; set; }

        public long Updated { get; set; }

        public long Created { get; set; }

        public JObject? Custom { get; set; }

        public string? CreatedByClient { get; set; }

        public string? UpdatedByClient { get; set; }
    }
}
