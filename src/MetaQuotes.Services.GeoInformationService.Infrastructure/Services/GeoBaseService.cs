using System.Text;
using MetaQuotes.Services.GeoInformationService.Domain.Models;

namespace MetaQuotes.Services.GeoInformationService.Infrastructure.Services
{
    public class GeoBaseService
    {
        private Header header = new Header();
        private List<IpRangeRecord> ipRangeRecords = new List<IpRangeRecord>();
        private List<LocationRecord> locationRecords = new List<LocationRecord>();

        Dictionary<string, List<int>> cityIndex = new Dictionary<string, List<int>>();

        public void LoadData(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                header.Version = reader.ReadInt32();
                header.Name = Encoding.ASCII.GetString(reader.ReadBytes(32)).TrimEnd('\0');
                header.Timestamp = reader.ReadUInt64();
                header.Records = reader.ReadInt32();
                header.OffsetRanges = reader.ReadUInt32();
                header.OffsetCities = reader.ReadUInt32();
                header.OffsetLocations = reader.ReadUInt32();

                fs.Seek(header.OffsetRanges, SeekOrigin.Begin);
                for (int i = 0; i < header.Records; i++)
                {
                    IpRangeRecord ipRangeRecord = new IpRangeRecord
                    {
                        IpFrom = reader.ReadUInt32(),
                        IpTo = reader.ReadUInt32(),
                        LocationIndex = reader.ReadUInt32()
                    };
                    ipRangeRecords.Add(ipRangeRecord);
                }

                fs.Seek(header.OffsetLocations, SeekOrigin.Begin);
                for (int i = 0; i < header.Records; i++)
                {
                    LocationRecord locationRecord = new LocationRecord
                    {
                        Country = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0'),
                        Region = Encoding.ASCII.GetString(reader.ReadBytes(12)).TrimEnd('\0'),
                        Postal = Encoding.ASCII.GetString(reader.ReadBytes(12)).TrimEnd('\0'),
                        City = Encoding.ASCII.GetString(reader.ReadBytes(24)).TrimEnd('\0'),
                        Organization = Encoding.ASCII.GetString(reader.ReadBytes(32)).TrimEnd('\0'),
                        Latitude = reader.ReadSingle(),
                        Longitude = reader.ReadSingle()
                    };
                    locationRecords.Add(locationRecord);
                }

                for (int i = 0; i < locationRecords.Count; i++)
                {
                    var city = locationRecords[i].City;
                    if (!cityIndex.ContainsKey(city))
                    {
                        cityIndex[city] = new List<int>();
                    }
                    cityIndex[city].Add(i);
                }
            }
        }


        public LocationRecord? FindLocationByIp(string ip)
        {
            uint numericIp = ConvertIpToUint(ip);
            int low = 0, high = ipRangeRecords.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                IpRangeRecord midRecord = ipRangeRecords[mid];

                if (numericIp >= midRecord.IpFrom && numericIp <= midRecord.IpTo)
                {
                    return locationRecords[(int)midRecord.LocationIndex];
                }

                if (numericIp < midRecord.IpFrom)
                {
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            }

            return null;
        }

        public IEnumerable<LocationRecord> FindLocationsByCity(string cityName)
        {
            if (cityIndex.TryGetValue(cityName, out var locationIndexes))
            {
                return locationIndexes.Select(index => locationRecords[index]);
            }

            return Enumerable.Empty<LocationRecord>();
        }

        private uint ConvertIpToUint(string ip)
        {
            var ipParts = ip.Split('.');
            uint numericIp = 0;

            foreach (var part in ipParts)
            {
                numericIp = numericIp << 8;
                numericIp += uint.Parse(part);
            }

            return numericIp;
        }
    }
}
