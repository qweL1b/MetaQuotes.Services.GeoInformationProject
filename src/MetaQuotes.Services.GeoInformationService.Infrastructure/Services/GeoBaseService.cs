﻿using System.Text;
using MetaQuotes.Services.GeoInformationService.Application.Services;
using MetaQuotes.Services.GeoInformationService.Domain.Models;

namespace MetaQuotes.Services.GeoInformationService.Infrastructure.Services
{
    public class GeoBaseService : IGeoSearching, ILoadDatabase
    {
        private Header header = new Header(); //database headers
        private List<IpRangeRecord> ipRangeRecords = new List<IpRangeRecord>();  //IP address records
        private List<LocationRecord> locationRecords = new List<LocationRecord>(); //Geolocation records

        Dictionary<string, List<int>> cityIndex = new Dictionary<string, List<int>>(); //Index for searching city names

        /// <summary>
        /// Reading a binary database and creating indexes
        /// </summary>
        /// <param name="filePath">Path to binary file</param>
        /// <returns>Database loading result</returns>
        public bool LoadData(string filePath)
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

            return header != null && ipRangeRecords.Count > 0;
        }

        /// <summary>
        /// Function for quick search in the loaded database by IP address
        /// </summary>
        /// <param name="ip">IP address</param>
        /// <returns>Return geolocation record</returns>
        public async Task<LocationRecord?> FindLocationByIpAsync(string ip)
        {
            return await Task.Run(() =>
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
            });
        }

        /// <summary>
        /// Search for records using city name
        /// </summary>
        /// <param name="cityName">Name of City</param>
        /// <returns>city data</returns>
        public async Task<IEnumerable<LocationRecord>> FindLocationsByCityAsync(string cityName)
        {
            return await Task.Run(() =>
            {
                if (cityIndex.TryGetValue(cityName, out var locationIndexes))
                {
                    return locationIndexes.Select(index => locationRecords[index]);
                }

                return Enumerable.Empty<LocationRecord>();
            });
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
