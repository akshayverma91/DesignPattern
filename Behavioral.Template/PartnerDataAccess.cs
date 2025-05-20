using Behavioral.Template.Model;
using Newtonsoft.Json;
using System.Data.Common;

namespace Behavioral.Template
{
    public interface IPartnerDataAccess : IDataAccess<Partner>
    {

    }

    public class PartnerDataAccess : DataAccess<Partner>, IPartnerDataAccess
    {
        private readonly IDatabase _db;

        public PartnerDataAccess(IDatabase db, ICommandProvider cmdProvider) : base(db, cmdProvider)
        {
            _db = db;
        }

        public new async Task<Partner> CreateAsync(Partner item)
        {
            item.Id = null;
            item.CreatedByClient = "test";
            item.UpdatedByClient = item.CreatedByClient;
            return await base.CreateAsync(item);
        }

        public new async Task<Partner> UpdateAsync(Partner item)
        {
            item.UpdatedByClient = "test";
            return await base.UpdateAsync(item);
        }

        public new async Task<Partner> GetAsync(string id, bool useWriteConnection = false)
        {
            var item = await base.GetAsync(id, useWriteConnection);
            return item;
        }

        public override string GetIdColumn()
        {
            return "id";
        }

        public override string GetSelect()
        {
            return @"SELECT id, company_id, company_identifier, company_name,
                            main_address, updated, created, custom, friendly_name, encryption_iv,
                            created_by_client, updated_by_client";
        }

        public override string GetFrom()
        {
            return @"FROM partners";
        }

        public override string GetTableName()
        {
            return "partners";
        }


        public override Task<Partner> GetItemFromReaderAsync(DbDataReader reader)
        {
            var item = new Partner
            {
                Id = reader.GetString(0),
                CompanyId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                CompanyIdentifier = reader.IsDBNull(2) ? (string)null : reader.GetString(2),
                CompanyName = reader.IsDBNull(3) ? (string)null : reader.GetString(3),
                MainAddress = reader.IsDBNull(4) ? null : reader.GetString(4),
                Updated = reader.GetInt64(5),
                Created = reader.GetInt64(6),
                CreatedByClient = reader.GetString(10),
                UpdatedByClient = reader.GetString(11),
            };
            return Task.FromResult(item);
        }

        public override Dictionary<string, MapAndType> GetMappings()
        {
            return new Dictionary<string, MapAndType>
            {
                {"id", new MapAndType{Map = "id", Type = FindConditionsType.Text}},
                {"companyId", new MapAndType{Map = "company_id", Type = FindConditionsType.Numeric}},
                {"companyIdentifier", new MapAndType{Map = "company_identifier", Type = FindConditionsType.Text}},
                {"companyName", new MapAndType{Map = "company_name", Type = FindConditionsType.Text}},
                {"updated", new MapAndType{Map = "updated", Type = FindConditionsType.Numeric}},
                {"created", new MapAndType{Map = "created", Type = FindConditionsType.Numeric}},
                {"custom/*", new MapAndType{Map = "custom", Type = FindConditionsType.JSON}},
                {"mainAddress/*", new MapAndType{Map = "main_address", Type = FindConditionsType.JSON}},
                {"createdByClient", new MapAndType{Map = "created_by_client", Type = FindConditionsType.Text}},
                {"updatedByClient", new MapAndType{Map = "updated_by_client", Type = FindConditionsType.Text}},
            };
        }


        public override Task<Dictionary<string, object>> GetFieldValuesAsync(Partner item)
        {
            var mainAddress = item.MainAddress == null ? (object)DBNull.Value : JsonConvert.SerializeObject(item.MainAddress, ApiHelper.DefaultJsonSettings);
            var values = new Dictionary<string, object>{
                {"id", item.Id},
                {"company_id", item.CompanyId.HasValue ? item.CompanyId.Value : (object)DBNull.Value},
                {"company_identifier", item.CompanyIdentifier},
                {"company_name", item.CompanyName},
                {"friendly_name", item.FriendlyName},
                {"main_address", mainAddress},
                {"salesforce_account_id", item.CrmAccountId},
                {"updated", item.Updated},
                {"created", item.Created},
                {"updated_by_client", item.UpdatedByClient},
            };
           
            var custom = item.Custom == null ? (object)DBNull.Value : item.Custom.ToString(Newtonsoft.Json.Formatting.None);
            values.Add("custom", custom);
           
            return Task.FromResult(values);
        }

    }

    public class MapAndType
    {
        public string Map { get; set; } = string.Empty;
        public FindConditionsType Type { get; set; }
    }

    public enum FindConditionsType
    {
        Text,
        Numeric,
        JSON
    }

}
