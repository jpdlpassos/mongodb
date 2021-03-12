using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace mongodb
{
	internal class Program
	{
		private static readonly FacilityAttributeStringValue Country = new()
			{Id = Guid.NewGuid(), AttributeId = Guid.NewGuid(), Value = "Spain"};

		private static async Task Main(string[] args)
		{
			BsonClassMap.RegisterClassMap<FacilityAttributeDateTimeValue>();
			BsonClassMap.RegisterClassMap<FacilityAttributeStringValue>();
			BsonClassMap.RegisterClassMap<FacilityAttributeDecimalValue>();
			BsonClassMap.RegisterClassMap<MonitoredDataSourceId>();
			BsonClassMap.RegisterClassMap<CustomDataSourceId>();

			var client =
				new MongoClient(
					"mongodb://localhost:27017/?readPreference=primary&appname=mongodb-vscode%200.4.1&ssl=false");

			await client.DropDatabaseAsync("mongo-benchmark");
			var db = client.GetDatabase("mongo-benchmark");

			var typedCollection = db.GetCollection<Facility>("facilities");
			//Insert 5000 facilities with 150 parameter and 500 attributes each
			await typedCollection.InsertManyAsync(GetFacilities());

			var rawCollection = db.GetCollection<RawBsonDocument>("facilities");
			var sw = Stopwatch.StartNew();
			var rawValues = await rawCollection
				.Find(new BsonDocumentFilterDefinition<RawBsonDocument>(new BsonDocument())).ToListAsync();
			sw.Stop();
			Console.WriteLine(rawValues.Count + " raw values took " + sw.ElapsedMilliseconds);

			//Typed

			sw.Restart();
			var typedValues = await typedCollection.Find(new BsonDocument()).ToListAsync();
			sw.Stop();
			Console.WriteLine(typedValues.Count + " typed values took " + sw.ElapsedMilliseconds);
		}

		private static IEnumerable<Facility> GetFacilities()
		{
			var parameter = Enumerable.Range(0, 150).Select(_ => Guid.NewGuid()).ToList();
			var attributes = Enumerable.Range(0, 500).Select(_ => Guid.NewGuid()).ToList();

			for (var i = 0; i < 5000; i++)
				yield return new Facility
				{
					Id = Guid.NewGuid(),
					ParametersValues = parameter.Select(p => new ParameterValue
					{
						Id = Guid.NewGuid(),
						ParameterId = p,
						DataSourceId = new CustomDataSourceId {Id = Guid.NewGuid()}
					}).ToList(),
					AttributesValues =
						i % 3 == 0
							? new List<AttributeValue>(attributes.Take(499).Select(a =>
								new FacilityAttributeStringValue
									{Id = Guid.NewGuid(), AttributeId = a, Value = a.ToString()}).Append(Country))
							: new List<AttributeValue>(attributes.Select(a =>
								new FacilityAttributeStringValue
									{Id = Guid.NewGuid(), AttributeId = a, Value = a.ToString()}))
				};
		}
	}
}